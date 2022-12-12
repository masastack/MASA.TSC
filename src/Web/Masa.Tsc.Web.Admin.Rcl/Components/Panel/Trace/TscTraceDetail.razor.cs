// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

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

    private async Task QueryTraceDetailAndToTree(string traceId)
    {
        var data = await ApiCaller.TraceService.GetAsync(traceId);

        _count = data.Count() - 1;
        if (_count == -1)
        {
            _count = 0;
        }

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

    private void OnActiveUpdate(List<TraceResponseTree> items)
    {
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

                    if (index == 0 && internalParentLeft > 0)
                    {
                        var marginLeft = internalParentLeft / root.DoubleDuration;
                        node.Timelines.Add(new TraceResponseTimeline(true, duration / root.DoubleDuration, marginLeft));
                    }
                    else
                    {
                        node.Timelines.Add(new TraceResponseTimeline(true, duration / root.DoubleDuration));
                    }

                    var childDuration = child.DoubleDuration;
                    node.Timelines.Add(new TraceResponseTimeline(false, childDuration / root.DoubleDuration));

                    lastTimestamp = child.EndTimestamp;
                }

                if (lastTimestamp < node.EndTimestamp)
                {
                    var duration = (node.EndTimestamp - lastTimestamp).TotalMilliseconds;
                    node.Timelines.Add(new TraceResponseTimeline(true, duration / root.DoubleDuration));
                }
            }
            else if (parent is not null)
            {
                var marginLeft = internalParentLeft / root.DoubleDuration;

                var durationPercent = (node.DoubleDuration / root.DoubleDuration);
                node.Timelines.Add(new TraceResponseTimeline(true, durationPercent, marginLeft));
            }
            else
            {
                node.Timelines.Add(new TraceResponseTimeline(true, 1));
            }
        }

        return nodes;
    }

    private static string FormatDuration(double duration)
    {
        var ms = (long)Math.Ceiling(duration);

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
