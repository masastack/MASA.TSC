// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscTraceDetail
{
    [Parameter]
    public string TraceId { get; set; }

    private bool _isLoading = true;

    private string _lastTraceId;
    private List<TraceResponseTree> treeData = new();
    private StringNumber tabValue;

    private List<string>? actives;
    private TraceResponseTree? _activeTreeItem;

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (_lastTraceId != TraceId)
        {
            _lastTraceId = TraceId;

            await QueryTraceDetailAndToTree(TraceId);
        }
    }

    private async Task QueryTraceDetailAndToTree(string traceId)
    {
        _isLoading = true;
        var data = await ApiCaller.TraceService.GetAsync(traceId);
        treeData = ToTree(data, null);
        if (treeData.Any())
        {
            _activeTreeItem = treeData.First();
            actives = new List<string>() { _activeTreeItem.SpanId };
        }

        _isLoading = false;
    }

    private void OnActiveUpdate(List<TraceResponseTree> items)
    {
        if (items.Count == 0)
        {
            return;
        }

        _activeTreeItem = items.First();
    }

    private List<TraceResponseTree> ToTree(IEnumerable<TraceResponseDto> items, TraceResponseTree? parent, long parentLeft = 0)
    {
        int parentLevel = parent?.Level ?? 0;
        string? parentSpanId = parent?.SpanId;

        var currentLevel = parentLevel + 1;
        var found = items.Where(item => item.ParentSpanId == parentSpanId);
        var nodes = found.Select(item => new TraceResponseTree(item, currentLevel)).ToList();

        foreach (var node in nodes)
        {
            var items2 = items.Except(found).ToList();
            node.Children ??= new();

            long internalParentLeft = parentLeft;
            if (parent is not null)
            {
                internalParentLeft += (long)Math.Ceiling((node.Timestamp - parent.Timestamp).TotalMilliseconds);
            }

            var children = ToTree(items2, node, internalParentLeft).OrderBy(u => u.Timestamp).ToList();

            if (children.Any())
            {
                node.Children.AddRange(children);

                if (internalParentLeft > 0)
                {
                    Console.Out.WriteLine("internalParentLeft = {0}", internalParentLeft);
                    var marginLeft = internalParentLeft / (double)parent.Duration;

                    var duration = (long)Math.Floor((node.EndTimestamp - node.Timestamp).TotalMilliseconds);
                    var durationPercent = (duration / (double)parent.Duration);
                    node.Timelines.Add(new TraceResponseTimeline(true, durationPercent, marginLeft));
                }

                var lastTimestamp = node.Timestamp;
                foreach (var child in children)
                {
                    var duration = (long)Math.Floor((child.Timestamp - lastTimestamp).TotalMilliseconds);
                    if (duration < 0)
                    {
                        continue;
                    }

                    node.Timelines.Add(new TraceResponseTimeline(true, duration / (double)node.Duration));

                    var childDuration = child.Duration;
                    node.Timelines.Add(new TraceResponseTimeline(false, childDuration / (double)node.Duration));

                    lastTimestamp = child.EndTimestamp;
                }

                if (lastTimestamp < node.EndTimestamp)
                {
                    var duration = (long)Math.Floor((node.EndTimestamp - lastTimestamp).TotalMilliseconds);
                    node.Timelines.Add(new TraceResponseTimeline(true, duration / (double)node.Duration));
                }
            }
            else if (parent is not null)
            {
                Console.Out.WriteLine("internalParentLeft = {0}", internalParentLeft);
                var marginLeft = internalParentLeft / (double)parent.Duration; // TODO:use top root duration!

                var duration = (long)Math.Floor((node.EndTimestamp - node.Timestamp).TotalMilliseconds);
                var durationPercent = (duration / (double)parent.Duration);
                node.Timelines.Add(new TraceResponseTimeline(true, durationPercent, marginLeft));
            }
            else
            {
                node.Timelines.Add(new TraceResponseTimeline(true, 1));
            }
        }

        return nodes;
    }

    private static string FormatDuration(long ms)
    {
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
