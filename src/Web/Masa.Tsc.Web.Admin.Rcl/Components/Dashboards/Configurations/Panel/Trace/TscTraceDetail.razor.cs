// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Google.Protobuf.WellKnownTypes;
using static Nest.JoinField;

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
    private List<List<TraceResponseTimeline>> _timelinesView = new();
    private List<LogModel> _logs = new();
    private bool _loadLogs = false;
    private string? lastQuerySpanId;
    private MVirtualScroll<TraceResponseTree> _vs;

    internal async Task OpenAsync(string traceId)
    {
        _tabValue = 0;
        _dialogValue = true;
        StateHasChanged();

        await QueryTraceDetailAndToTree(traceId);
        StateHasChanged();
    }

    private void DialogValueChanged(bool val)
    {
        _dialogValue = val;

        if (!val)
        {
            _activeTreeItem = null;
            _rootTreeItem = null;
            _treeData.Clear();
            _timelinesView.Clear();
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
        _timelinesView.Clear();
        if (_treeData.Any())
        {
            _rootTreeItem = _treeData.First();
            _activeTreeItem = _treeData.First();
            _actives = new List<string>() { _activeTreeItem.SpanId };

            _timelinesView.Add(_rootTreeItem.Timelines);

            if (_rootTreeItem.Children is not null)
            {
                Test(_rootTreeItem.Children, 2);
            }

            void Test(List<TraceResponseTree> trees, int level)
            {
                if (_timelinesView.Count < level)
                {
                    _timelinesView.Add(new List<TraceResponseTimeline>());
                }

                var timelines = _timelinesView[level - 1];

                foreach (var tree in trees)
                {
                    timelines.AddRange(tree.Timelines);

                    if (tree.Children is null || tree.Children.Count == 0)
                    {
                        continue;
                    }

                    Test(tree.Children, level + 1);
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

    private List<TraceResponseTree> ToTree(IEnumerable<TraceResponseDto> items, TraceResponseTree? parent, TraceResponseTree? root = null,
        double parentLeft = 0)
    {
        int parentLevel = parent?.Level ?? 0;
        string? parentSpanId = parent?.SpanId;

        if (parent == null && items != null && items.Any())
        {
            if (!items.Any(item => string.IsNullOrEmpty(item.ParentSpanId)))
            {
                var first = items.First();
                parentSpanId = first.ParentSpanId;
                root = new TraceResponseTree(new TraceResponseDto
                {
                    SpanId = parentSpanId,
                    TraceId = first.TraceId,
                    Timestamp = first.Timestamp,
                    EndTimestamp = items.Last().EndTimestamp
                }, 0);
            }
        }

        var currentLevel = parentLevel + 1;
        var found = items.Where(item => item.ParentSpanId == parentSpanId);
        var nodes = found.Select(item => new TraceResponseTree(item, currentLevel)).ToList();

        foreach (var node in nodes)
        {
            var restItems = items.Except(found).ToList();
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
                //if (durationPercent - 1 > 0)
                //    durationPercent = 0;
                node.Timelines.Add(new TraceResponseTimeline(true, durationPercent, marginLeft));
            }
            else
            {
                node.Timelines.Add(new TraceResponseTimeline(true, 1));
            }
        }

        return nodes;
    }

    private async Task LoadSpanLogsAsync(string? spanId)
    {
        if (_loadLogs || lastQuerySpanId == spanId)
            return;
        _loadLogs = true;
        if (string.IsNullOrEmpty(spanId))
        {

            _logs.Clear();
            _loadLogs = false;
            return;
        }

        int pageSize = 9999;
        Loading = true;
        var query = new LogPageQueryDto
        {
            PageSize = pageSize,
            Page = 1,
            SpanId = spanId
        };
        var response = await ApiCaller.LogService.GetDynamicPageAsync(query);

        lastQuerySpanId = spanId;
        _logs = response.Result.Select(item => new LogModel(item.Timestamp, item.ExtensionData.ToDictionary(item => item.Key, item => new LogTree(item.Value)))).ToList();
        Loading = false;
        _loadLogs = false;
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
