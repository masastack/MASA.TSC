﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using BlazorComponent;
using Masa.Blazor;
using Nest;
using Newtonsoft.Json.Linq;
using OneOf.Types;
using System.Collections.Generic;
using System;

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscTraceDetail
{
    private bool _dialogValue;
    private StringNumber _tabValue;

    private List<TraceResponseTree> _treeData = new();
    private List<string>? _actives;
    private TraceResponseTree? _activeTreeItem;
    private TraceResponseTree? _rootTreeItem;
    private int _count;
    private readonly List<List<TraceResponseTimeline>> _timelineView = new();
    private List<LogModel> _logs = new();
    private bool _loadLogs = false;
    private string? _lastQuerySpanId;
    private SSheetDialog.MProgressInfo _loading = new();
    private MVirtualScroll<TraceResponseTree> _vs;
    private int? _logsCount;
    
    internal async Task OpenAsync(string traceId)
    {
        _tabValue = 0;
        _dialogValue = true;
        _loading.Loading = true;
        _logsCount = null;
        StateHasChanged();
        await QueryTraceDetailAndToTree(traceId);
        _loading.Loading = false;
        StateHasChanged();
    }

    internal async Task OpenAsync(ConfigurationRecord configurationRecord, string url)
    {
        _tabValue = 0;
        _dialogValue = true;
        _loading.Loading = true;
        _logsCount = null;
        StateHasChanged();
        var traceId = await ApiCaller.TraceService.GetTraceIdByMetricAsync(configurationRecord.Service!, url, configurationRecord.StartTime.UtcDateTime, configurationRecord.EndTime.UtcDateTime);
        await QueryTraceDetailAndToTree(traceId);
        _loading.Loading = false;
        StateHasChanged();
    }

    private void DialogValueChanged(bool val)
    {
        _dialogValue = val;
        if (!val)
        {
            _activeTreeItem = null;
            _rootTreeItem = null;
            _logsCount = null;
            _treeData.Clear();
            _timelineView.Clear();
        }
    }

    private List<double> NumberSplit(double number)
    {
        List<double> sections = new();
        number = Math.Round(number, 0);

        var steps = 5d;
        var step = Math.Ceiling(number / steps);

        for (int i = 1; i < steps; i++)
        {
            var curNumber = step * i;

            if (curNumber >= number)
                break;

            sections.Add(step * i);
        }

        return sections;
    }

    private async Task QueryTraceDetailAndToTree(string traceId)
    {
        var data = await ApiCaller.TraceService.GetAsync(traceId);
        data = data.DistinctBy(span => span.SpanId).ToList();
        _count = data.Count();

        _treeData = ToTree(data.OrderBy(item => item.Timestamp), null);
        _timelineView.Clear();
        if (_treeData.Any())
        {
            _rootTreeItem = _treeData.First();
            _activeTreeItem = _treeData.First();
            _actives = new List<string>() { _activeTreeItem.SpanId };

            CalcTraceTimeConsuming(_treeData, _rootTreeItem.Timestamp, _rootTreeItem.EndTimestamp);
            CalculateOverlapDisplayDlank();
            _timelineView.Reverse();
        }
    }
    
    /*Serial is on one line, while parallel requires multiple lines*/
    /*Prioritize finding serial, not within a time range*/
    private void CalcTraceTimeConsuming(List<TraceResponseTree> treeData, DateTime rootTimestamp, DateTime rootEndTimestamp)
    {
        List<TraceResponseTimeline> timelineViews = new();
        //double sumLeft = 0;
        double rootTimespan = Math.Round((rootEndTimestamp - rootTimestamp).TotalMilliseconds, 4);
        foreach (var item in treeData)
        {
            double percent = Math.Round((item.EndTimestamp - item.Timestamp).TotalMilliseconds / rootTimespan, 4);
            double left = Math.Round((item.Timestamp - rootTimestamp).TotalMilliseconds / rootTimespan, 4);
            percent = percent > 1 ? 1 : percent;
            left = left < 0 ? 0 : left;
            var traceView = new TraceResponseTimeline(true, percent, left);
            traceView.Id = item.SpanId;
            traceView.ParentId = item.ParentSpanId;
            timelineViews.Add(traceView);

            if (item.Children is not null && item.Children.Any())
            {
                CalcTraceTimeConsuming(item.Children, rootTimestamp, rootEndTimestamp);
            }
        }
        _timelineView.Add(timelineViews);
    }

    /*Calculate Overlap Display Blank Above*/
    private void CalculateOverlapDisplayDlank()
    {
        /*Ignore root*/
        var timelinesViewCopy = _timelineView.DeepClone();
        for (int i = 0; i < timelinesViewCopy.Count - 1; i++)
        {
            var timeLines = timelinesViewCopy[i];
            List<TraceResponseTimeline> parentLineList = new List<TraceResponseTimeline>();
            for (int j = 0; j < timeLines.Count; j++)
            {
                //next line
                TraceResponseTimeline? nextTimeLine = j + 1 < timeLines.Count ? timeLines[j + 1] : null;
                SplitLines(timeLines[j], nextTimeLine, timelinesViewCopy, ref parentLineList);
            }
            if (parentLineList.Any())
            {
                for (int k = 0; k < timelinesViewCopy.Count; k++)
                {
                    if (_timelineView[k].Any(x => x.Id == parentLineList[0].Id))
                    {
                        _timelineView[k].Clear();
                        _timelineView[k] = parentLineList;
                    }
                }
            }
        }
    }

    private void SplitLines(TraceResponseTimeline timeline, TraceResponseTimeline? nextTimeLine, List<List<TraceResponseTimeline>> timelinesViewCopy, ref List<TraceResponseTimeline> parentLineList)
    {
        for (var i = 0; i < timelinesViewCopy.Count - 1; i++)
        {
            var parentLines = timelinesViewCopy[i].Where(x => x.Id == timeline.ParentId).ToList();
            foreach (var parentLine in parentLines.ToArray())
            {
                //parent is not root
                if (!string.IsNullOrEmpty(parentLine.ParentId))
                {
                    //split left              
                    var leftParentLine = parentLine.DeepClone();
                    leftParentLine.marginLeft = parentLine.marginLeft;
                    if (!parentLineList.Any())
                    {
                        leftParentLine.Percent = Math.Round(timeline.marginLeft - leftParentLine.marginLeft, 4);
                        if (leftParentLine.Percent > 0)
                            parentLineList.Add(leftParentLine);
                    }
                    //split right
                    var rightParentLine = parentLine.DeepClone();
                    rightParentLine.marginLeft = Math.Round(timeline.marginLeft + timeline.Percent, 4);
                    rightParentLine.Percent = Math.Round(nextTimeLine is not null ? nextTimeLine.marginLeft - rightParentLine.marginLeft : parentLine.Percent - rightParentLine.marginLeft, 4);
                    if (rightParentLine.Percent > 0)
                        parentLineList.Add(rightParentLine);
                }
            }
        }
    }

    private async Task OnTabValueChange(StringNumber value)
    {
        _tabValue = value;
        if (_tabValue.AsT1 == 3)
            await LoadSpanLogsAsync(_activeTreeItem?.SpanId);
    }

    private async Task OnActiveUpdate(List<TraceResponseTree> items)
    {
        _loadLogs = false;
        _logsCount = null;
        if (_tabValue.AsT1 == 3)
        {
            await LoadSpanLogsAsync(items.FirstOrDefault()?.SpanId);
        }

        if (items.Count == 0)
        {
            return;
        }
        _activeTreeItem = items.First();
    }

    private List<TraceResponseTree> ToTree(IEnumerable<TraceResponseDto>? items, TraceResponseTree? parent, TraceResponseTree? root = null,
        double parentLeft = 0)
    {
        int parentLevel = parent?.Level ?? 0;
        string? parentSpanId = parent?.SpanId;

        if (parent == null && items != null && items.Any())
        {
            var first = GetRoots(items).FirstOrDefault();
            parentSpanId = first.ParentSpanId;
            root = new TraceResponseTree(new TraceResponseDto
            {
                SpanId = parentSpanId,
                TraceId = first.TraceId,
                Timestamp = first.Timestamp,
                EndTimestamp = items.Last().EndTimestamp
            }, 0);
        }

        var currentLevel = parentLevel + 1;
        var found = items.Where(item => item.ParentSpanId == parentSpanId);
        var nodes = found.Select(item => new TraceResponseTree(item, currentLevel)).ToList();
        var restItems = items.Except(found).ToList();

        foreach (var node in nodes)
        {
            node.Children ??= new();

            double internalParentLeft = parentLeft;
            if (parent is not null)
            {
                internalParentLeft += (node.Timestamp - parent.Timestamp).TotalMilliseconds;
            }

            root ??= node;

            if (restItems.Any())
            {
                var children = ToTree(restItems, node, root, internalParentLeft).OrderBy(u => u.Timestamp).ToList();

                node.Children.AddRange(children);

                var lastTimestamp = node.Timestamp;
                foreach (var (child, index) in children.Select((child, index) => (child, index)))
                {
                    var duration = (child.Timestamp - lastTimestamp).TotalMilliseconds;
                    var percent = Math.Round(duration / root.DoubleDuration, 4, MidpointRounding.ToPositiveInfinity);
                    if (percent - 1 > 0)
                        percent = 0;

                    if (index == 0 && internalParentLeft > 0)
                    {
                        var marginLeft = internalParentLeft / root.DoubleDuration;
                        node.Timelines.Add(new TraceResponseTimeline(true, percent, marginLeft));
                    }
                    else
                    {
                        node.Timelines.Add(new TraceResponseTimeline(true, percent));
                    }

                    var childDuration = child.DoubleDuration;
                    node.Timelines.Add(new TraceResponseTimeline(false, percent));

                    lastTimestamp = child.EndTimestamp;
                }

                if (lastTimestamp < node.EndTimestamp)
                {
                    var duration = (node.EndTimestamp - lastTimestamp).TotalMilliseconds;
                    var percent = Math.Round(duration / root.DoubleDuration, 4, MidpointRounding.ToPositiveInfinity);
                    node.Timelines.Add(new TraceResponseTimeline(true, percent));
                }
            }
            else if (parent is not null)
            {
                var marginLeft = internalParentLeft / root.DoubleDuration;

                var durationPercent = Math.Round(node.DoubleDuration / root.DoubleDuration, 4, MidpointRounding.ToPositiveInfinity);
                node.Timelines.Add(new TraceResponseTimeline(true, durationPercent, marginLeft));
            }
            else
            {
                node.Timelines.Add(new TraceResponseTimeline(true, 1));
            }
        }

        return nodes;
    }

    private IEnumerable<TraceResponseDto> GetRoots(IEnumerable<TraceResponseDto> items)
    {
        var roots = items.Where(item => string.IsNullOrEmpty(item.ParentSpanId));
        if (roots.Any())
            return roots.ToList();

        var spanIds = roots.Select(item => item.SpanId).Distinct().ToList();
        var parentIds = roots.Select(item => item.ParentSpanId).Distinct().ToList();

        spanIds.Sort();
        parentIds.Sort();

        parentIds.RemoveAll(spanId => spanIds.Contains(spanId));
        if (!parentIds.Any())
        {
            //没有root
            return items;
        }
        return items.Where(item => parentIds.Contains(item.ParentSpanId)).ToList();
    }

    private async Task LoadSpanLogsAsync(string? spanId)
    {
        if (_loadLogs || _lastQuerySpanId == spanId)
            return;
        _loadLogs = true;
        int pageSize = 9999;
        if (string.IsNullOrEmpty(spanId))
        {
            _logs.Clear();
            _loadLogs = false;
            return;
        }
        await InvokeStateHasChangedAsync();
        var query = new LogPageQueryDto
        {
            PageSize = pageSize,
            Page = 1,
            SpanId = spanId
        };
        var response = await ApiCaller.LogService.GetDynamicPageAsync(query);

        _lastQuerySpanId = spanId;
        _logs = response.Result.Select(item =>
                new LogModel(item.Timestamp, item.ExtensionData.ToDictionary(i => i.Key, i => new LogTree(i.Value))))
            .ToList();
        _logsCount = _logs.Count;
        _loadLogs = false;
        await InvokeStateHasChangedAsync();
    }

    private static string FormatDuration(double duration)
    {
        if (duration < 1)
            return "< 1ms";

        var ms = (long)Math.Round(duration, 0);

        if (ms < 1000)
        {
            return $"{ms}ms";
        }

        if (ms < 60000)
        {
            return $"{(ms / 1000d):F}s";
        }

        return $"{(ms / 60000d):F}m";
    }
}
