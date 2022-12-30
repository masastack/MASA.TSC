// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Apps;

public partial class AppAutoComplete
{
    [Parameter]
    public string Value { get; set; }

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public bool FillBackground { get; set; } = true;

    [Inject]
    public IPmClient PmClient { get; set; }

    List<AppDetailModel> Apps { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Apps = await PmClient.AppService.GetListAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (string.IsNullOrEmpty(Value) && Apps.Any())
        {
            var value = Apps.First().Name;
            await ValueChanged.InvokeAsync(value);
        }
    }
}
