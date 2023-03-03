﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Apps;

public partial class AppAutoComplete
{
    [Inject]
    public IPmClient PmClient { get; set; }

    [Parameter]
    public string Value { get; set; }

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public bool FillBackground { get; set; } = true;

    [Parameter]
    public bool Metric { get; set; }

    [Parameter]
    public List<AppDetailModel> Apps { get; set; } = new();

    bool _hasLoadData;

    public AppDetailModel? CurrentApp => Apps.FirstOrDefault(app => app.Identity == Value);

    protected override async Task OnInitializedAsync()
    {
        if (!_hasLoadData)
        {
            if (Metric)
            {
                var data = await ApiCaller.MetricService.GetValues(new RequestMetricListDto { Type = MetricValueTypes.Service });
                if (data != null && data.Any())
                {
                    Apps = data.Select(item => new AppDetailModel
                    {
                        Identity = item,
                        Name = item
                    }).ToList();
                }
            }
            else
            {
                var data = await PmClient.AppService.GetListAsync();
                if (data != null && data.Any())
                    Apps = data;
            }
            _hasLoadData = true;
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (string.IsNullOrEmpty(Value) && Apps?.Any() is true)
        {
            var value = Apps.First().Identity;
            await ValueChanged.InvokeAsync(value);
        }
    }
}
