// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using BlazorComponent.Web;

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscPanelEdit : IDisposable
{
    [Inject]
    public Window Window { get; set; }

    [Inject]
    public Document Document { get; set; }

    [Parameter]
    public PanelDto Value { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }

    [Parameter]
    public EventCallback<PanelDto> ValueChanged { get; set; }

    private ElementReference _div;

    private string _style = "";

    protected override async Task OnInitializedAsync()
    {
        base.OnInitialized();
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        //if (firstRender)
        //{
        //    var html = Document.GetElementByReference(_div);
        //    //await Window.AddEventListenerAsync("resize", html.Selector, OnResize, false);
        //    StateHasChanged();
        //}
        await base.OnAfterRenderAsync(firstRender);
    }

    protected override void OnWatcherInitialized()
    {
        base.OnWatcherInitialized();
        //Watcher
        //    .Watch<bool>(nameof(Value.Height), ReCalculateInputHeight)
        //     .Watch<bool>(nameof(Value.Width), ReCalculateInputHeight)
    }


    protected override void OnParametersSet()
    {
        StringBuilder text = new();
        if (!ReadOnly)
            text.Append("resize:both;");
        if (Value != null)
        {
            if (!string.IsNullOrEmpty(Value.Height))
                text.Append($"height:{Value.Height}");

            if (!string.IsNullOrEmpty(Value.Width))
                text.Append($"width:{Value.Width}");
        }
        _style = text.ToString();
        base.OnParametersSet();
    }

    private async Task OnResize()
    {
        var html = Document.GetElementByReference(_div);
        var height = await html.GetClientHeightAsync();
        var width = await html.GetClientWidthAsync();

        //await ApiCaller.PanelService.UpdateWidthHeightAsync(Value.Id, CurrentUserId, $"{width}px", $"{height}px");
    }

    private async Task OnDelete()
    {
        if (!await PopupService.ConfirmAsync("删除", "确认要移除该panel吗"))
            return;
        await ApiCaller.PanelService.DeleteAsync(Value.InstrumentId, Value.Id);
        await OnCallParent(OperateCommand.Remove, Value);
    }

    private async Task OnEdit()
    {
        await OnCallParent(OperateCommand.Update, Value);
    }

    public new void Dispose()
    {
        Window.OnResize -= OnResize;
    }
}
