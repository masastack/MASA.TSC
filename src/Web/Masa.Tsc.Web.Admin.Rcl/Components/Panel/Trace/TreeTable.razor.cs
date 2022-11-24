// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TreeTable
{
    [Parameter]
    public bool Expand { get; set; } = true;

    [Parameter]
    public Func<object, string> KeyFunc { get; set; }

    [Parameter]
    public Func<object, string> ParentFunc { get; set; }

    
    [Parameter]
    public IEnumerable<TraceResponseDto> Items
    {
        get
        {
            return _items;
        }
        set
        {
            _items = value;
        }
    }

    [Parameter]
    public Func<TraceResponseDto, Task> OnRowClick { get; set; }

    [Parameter]
    public Func<TraceOverviewModel, Task> OnOverviewUpdate { get; set; }

    [Parameter]
    public TraceOverviewModel OverViewData { get; set; }

    private IEnumerable<TraceResponseDto> _items;
    private List<TraceTimeUsModel> _timeLines = new();
    private bool _isLoading = true;

    protected override async Task OnParametersSetAsync()
    {
        _isLoading = true;
        SetTimeLine();
        SetTreeLine();
        if (OnOverviewUpdate != null)
        {
            await OnOverviewUpdate(OverViewData);
        }
        _isLoading = false;
        await base.OnParametersSetAsync();
    }    

    private void SetTreeLine()
    {
        DateTime start = OverViewData.Start;
        long total = OverViewData.TimeUs;
        TraceTableLineModel? last = null;
        long t_total = 0;
        double left, width, right;
        foreach (var item in OverViewData.SpansDeeps)
        {
            if (item.Value.IsTransaction)
            {
                if (last != null)
                    t_total += last.TimeUs;
                last = item.Value;
            }

            if (item.Value.TimeUs - total == 0)
            {
                left = 0;
                width = 1;
                right = 0;
            }
            else
            {
                var t1 = last != null ? Math.Round((item.Value.Time - last.Time).TotalMilliseconds * 1000, 0) + t_total : 0;
                left = Math.Round(t1 * 1.0 / total, 4);
                width = Math.Round(item.Value.TimeUs * 1.0 / total, 4);
                right = 1 - left - width;
            }

            item.Value.Left = left.ToString("0.####%");
            item.Value.Width = width.ToString("0.####%");
            item.Value.Right = right.ToString("0.####%");
        }
    }    

    private string GetPadding(TraceResponseDto item)
    {
        string id = item.SpanId, parentId = item.ParentSpanId ?? string.Empty;
        int deep = OverViewData.SpansDeeps[id].Deep;
        bool hasChild = OverViewData.SpanChildren.ContainsKey(id);

        if (hasChild)//floder
        {
            if (deep == 0)//根
            {

            }
            else //子
            {

            }
        }

        return $"padding-left:{deep * 20}px";
    }

    private async Task OnRowClickAync(TraceResponseDto item)
    {
        if (OnRowClick != null)
            await OnRowClick(item);
    }

    private void SetTimeLine()
    {
        var total = OverViewData.TimeUs;
        _timeLines.Clear();
        var last = new TraceTimeUsModel(1)
        {
            TimeUs = total,
        };

        var item = total / 6;
        int count = 5;
        do
        {
            _timeLines.Add(new TraceTimeUsModel(1) { TimeUs = item, FloorLength = 0 });
            item += item;
        }
        while (_timeLines.Count - count < 0);
        _timeLines.Add(last);
    }
}