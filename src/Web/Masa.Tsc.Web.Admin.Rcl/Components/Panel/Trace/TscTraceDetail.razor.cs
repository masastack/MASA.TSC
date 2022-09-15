﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscTraceDetail
{
    [Parameter]
    public string TraceId { get { return _traceId; } set { _traceId = value; _tabIndex = "attr"; } }

    private StringNumber _tabIndex = "attr";
    private IEnumerable<object> _items = new List<object>();
    private bool _isLoading = true;
    private string _traceId = default!;
    private TraceDetailModel _selectItem;
    private TraceOverviewModel _overView = new TraceOverviewModel();

    private Func<object, long> _timeUsFunc = obj =>
    {
        var dic = (Dictionary<string, object>)obj;
        dic = (Dictionary<string, object>)(dic.ContainsKey("transaction") ? dic["transaction"] : dic["span"]);
        return Convert.ToInt64(((Dictionary<string, object>)dic["duration"])["us"]);

    };
    private Func<object, DateTime> _timeFunc = obj =>
    {
        var dic = (Dictionary<string, object>)obj;
        return DateTime.Parse(dic["@timestamp"].ToString()!);
    };
    private Func<object, string> _keyFunc { get; set; } = item =>
    {
        var dic = (Dictionary<string, object>)item;
        return (dic.ContainsKey("transaction") ? GetDictionaryValue(dic, "transaction.id") : GetDictionaryValue(dic, "span.id")).ToString();
    };
    private Func<object, string> _parentFunc { get; set; } = item => GetDictionaryValue((Dictionary<string, object>)item, "parent.id")?.ToString() ?? string.Empty;     

    private List<DataTableHeader<Dictionary<string, object>>> _headers = new()
    {
        new("Service", item => ((Dictionary<string, object>)item["service"])["name"])
        {
            Align = "start",
            Sortable = false
        },
        new("Endpoint", item => ((Dictionary<string, object>)item["transaction"])["name"])
        {
            Align = "start",
            Sortable = false
        },
        new("Duration (ms)", item => ((Dictionary<string, object>)((Dictionary<string, object>)item["transaction"])["duration"])["us"])
        {
            Align = "start",
            Sortable = false
        },
        new("Timestamp", item => item["@timestamp"])
        {
            Align = "start",
            Sortable = false
        },
        new("EndTime", item => ((Dictionary<string, object>)item["transaction"])["name"])
        {
            Align = "start",
            Sortable = false
        }
    };

    protected override async Task OnParametersSetAsync()
    {
        _isLoading = true;
        var data = await ApiCaller.TraceService.GetAsync(TraceId);
        if (data == null)
            return;

        _items = data.Select(item => (object)((JsonElement)item).ToKeyValuePairs()!);
        SetDeeep();
        SetOverview();
        await OnChangeRecordAsync(_items.FirstOrDefault()!);
        await base.OnParametersSetAsync();
        _isLoading = false;
    }

    private async Task OnChangeRecordAsync(object item)
    {
        _selectItem = new TraceDetailModel((Dictionary<string, object>)item);
        StateHasChanged();
        await Task.CompletedTask;
    }

    private static string CreateColor()
    {
        int length = 6;
        var bytes = new int[length];
        var index = 0;
        do
        {
            bytes[index] = Random.Shared.Next(0, 16);
            index++;
        } while (length - index > 0);
        return $"#{string.Join("", bytes.Select(b => b.ToString("X")))}";
    }

    private void SetDeeep()
    {
        if (_items == null || !_items.Any())
            return;
        _overView.SpansDeeps.Clear();
        _overView.SpanChildren.Clear();

        var list = new List<object>();
        foreach (var item in _items)
        {
            string id = _keyFunc(item), parentId = _parentFunc(item);
            long us = _timeUsFunc(item);
            DateTime time = _timeFunc(item);

            if (_overView.SpansDeeps.ContainsKey(id))
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
                TimeUs = us
            };

            if (string.IsNullOrEmpty(parentId))
                _overView.SpansDeeps.Add(id, add);
            else
            {
                if (_overView.SpansDeeps.ContainsKey(parentId))
                    deep = _overView.SpansDeeps[parentId].Deep;

                deep++;

                add.Deep = deep;
                _overView.SpansDeeps.Add(id, add);

                if (_overView.SpanChildren.ContainsKey(parentId))
                    _overView.SpanChildren[parentId].Add(id);
                else
                    _overView.SpanChildren.Add(parentId, new List<string> { id });
            }
        }

        var sortIds = _overView.SpansDeeps.Values.OrderBy(item => item.Time).ThenBy(item => item.Deep).Select(item => item.Id).ToList();
        _items = list.OrderBy(item => sortIds.IndexOf(_keyFunc(item)));
    }

    private void SetOverview()
    {
        _overView.Total = _items.Count();
        _overView.SearchName = default!;
        var data = _overView.SpansDeeps.Where(item => item.Value.IsTransaction && !_overView.SpansDeeps.ContainsKey(item.Value.ParentId)).ToList();
        DateTime start = data.Min(item => item.Value.Time);
        long total = data.Sum(item => item.Value.TimeUs);
        _overView.Start = start;
        _overView.TimeUs = total;
        _overView.Name = GetDictionaryValue(_items.First(), "transaction.name").ToString()!;        
        _overView.Services = _overView.SpansDeeps.Values.Select(item => item.ServiceName).Distinct().Select(item => new TraceOverviewServiceModel
        {
            Name = item,
            Color = CreateColor()
        }).ToList();
    }
}
