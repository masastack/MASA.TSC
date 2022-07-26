// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Admin.Rcl.Pages.Components;

public partial class TreeTable
{
    [Parameter]
    public bool Expand { get; set; } = true;

    [Parameter]
    public Func<object, string> KeyFunc { get; set; }

    [Parameter]
    public Func<object, string> ParentFunc { get; set; }

    [Parameter]
    public IEnumerable<object> Items
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
    public Func<object, Task> OnRowClick { get; set; }

    [Parameter]
    public Func<TraceOverViewModel, Task> OnOverViewUpdate { get; set; }

    private Func<object, long> TimeUsFunc = obj =>
    {
        var dic = (Dictionary<string, object>)obj;
        dic = (Dictionary<string, object>)(dic.ContainsKey("transaction") ? dic["transaction"] : dic["span"]);
        return Convert.ToInt64(((Dictionary<string, object>)dic["duration"])["us"]);

    };
    private Func<object, DateTime> TimeFunc = obj =>
    {
        var dic = (Dictionary<string, object>)obj;
        return DateTime.Parse(dic["@timestamp"].ToString()!);
    };
    private IEnumerable<object> _items;
    private Dictionary<string, TraceTableLineModel> _keyDeeps = new Dictionary<string, TraceTableLineModel>();
    private Dictionary<string, List<string>> _dicChild = new Dictionary<string, List<string>>();
    private TraceOverViewModel _overView = new TraceOverViewModel();
    private bool _isLoading = true;

    protected override async Task OnParametersSetAsync()
    {
        _isLoading = true;
        SetDeeep();
        var parentIds = _keyDeeps.Keys;
        var data = _keyDeeps.Where(item => item.Value.IsTransaction && !_keyDeeps.Keys.Contains(item.Value.ParentId)).ToList();
        DateTime start = data.Min(item => item.Value.Time);
        long total = data.Sum(item => item.Value.Ms);

        SetOverView();
        if (OnOverViewUpdate != null)
        {
            await OnOverViewUpdate(_overView);
        }
        SetTreeLine();
        _isLoading = false;
        await base.OnParametersSetAsync();
    }

    private void SetTreeLine()
    {
        DateTime start = _overView.Start;
        long total = _overView.TimeUs;
        TraceTableLineModel? last = null;
        long t_total = 0;
        double left, width, right;
        foreach (var item in _keyDeeps)
        {
            if (item.Value.IsTransaction)
            {
                if (last != null)
                    t_total += last.Ms;
                last = item.Value;
            }

            if (item.Value.Ms - total == 0)
            {
                left = 0;
                width = 1;
                right = 0;
            }
            else
            {
                var t1 = last != null ? Math.Round((item.Value.Time - last.Time).TotalMilliseconds * 1000, 0) + t_total : 0;
                left = Math.Round(t1 * 1.0 / total, 4);
                width = Math.Round(item.Value.Ms * 1.0 / total, 4);
                right = 1 - left - width;
            }

            item.Value.Left = left.ToString("0.####%");
            item.Value.Width = width.ToString("0.####%");
            item.Value.Right = right.ToString("0.####%");
        }
    }

    private void SetDeeep()
    {
        if (Items == null || !Items.Any())
            return;
        _keyDeeps.Clear();
        _dicChild.Clear();

        var list = new List<object>();
        foreach (var item in Items)
        {
            string id = KeyFunc(item), parentId = ParentFunc(item);
            long us = TimeUsFunc(item);
            DateTime time = TimeFunc(item);

            if (_keyDeeps.ContainsKey(id))
                continue;
            list.Add(item);
            int deep = 0;
            bool isTransaction = ((Dictionary<string, object>)item).ContainsKey("transaction");
            string name = GetDictionaryValue(item, "service.name").ToString()!;

            var add = new TraceTableLineModel
            {
                Id = id,
                ParentId = parentId,
                Time = time,
                Deep = deep,
                IsTransaction = isTransaction,
                ServiceName = name,
                Ms = us
            };

            if (string.IsNullOrEmpty(parentId))
                _keyDeeps.Add(id, add);
            else
            {
                if (_keyDeeps.ContainsKey(parentId))
                    deep = _keyDeeps[parentId].Deep;

                deep++;

                add.Deep = deep;
                _keyDeeps.Add(id, add);

                if (_dicChild.ContainsKey(parentId))
                    _dicChild[parentId].Add(id);
                else
                    _dicChild.Add(parentId, new List<string> { id });
            }
        }

        var sortIds = _keyDeeps.Values.OrderBy(item => item.Time).ThenBy(item => item.Deep).Select(item => item.Id).ToList();
        _items = list.OrderBy(item => sortIds.IndexOf(KeyFunc(item)));
    }

    private void SetOverView()
    {
        _overView.Total = _items.Count();
        var data = _keyDeeps.Where(item => item.Value.IsTransaction && !_keyDeeps.ContainsKey(item.Value.ParentId)).ToList();
        DateTime start = data.Min(item => item.Value.Time);
        long total = data.Sum(item => item.Value.Ms);
        _overView.Start = start;
        _overView.TimeUs = total;
        _overView.Name = GetDictionaryValue(_items.First(), "transaction.name").ToString()!;
        _overView.Services = _keyDeeps.Values.Select(item => item.ServiceName).Distinct().Select(item => new TraceOverViewServiceModel
        {
            Name = item,
            Color = "green"
        }).ToList();
    }

    private string GetPadding(object item)
    {
        string id = KeyFunc(item), parentId = ParentFunc(item);
        int deep = _keyDeeps[id].Deep;
        bool hasChild = _dicChild.ContainsKey(id);

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

    private async Task OnRowClickAync(object item)
    {
        if (OnRowClick != null)
            await OnRowClick(item);
    }
}