// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscInstrumentDetail
{
    [Parameter]
    public string Id { get; set; }

    [Inject]
    public AddInstrumentsDto Data { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    private bool _showDialog = false;


    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrEmpty(Id) && Data == null)
        {
            await PopupService.AlertAsync("Id or Data must set", AlertTypes.Error);
            return;
        }

        //todo load data

        if (Data == null && !string.IsNullOrEmpty(Id))
        {
            await PopupService.AlertAsync("Id or Data must set", AlertTypes.Error);
            return;
        }

        await base.OnInitializedAsync();
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {

        }

        return base.OnAfterRenderAsync(firstRender);
    }

    private async Task OnCloseAsync()
    {
        if (OnClose.HasDelegate)
            await OnClose.InvokeAsync();
    }
}