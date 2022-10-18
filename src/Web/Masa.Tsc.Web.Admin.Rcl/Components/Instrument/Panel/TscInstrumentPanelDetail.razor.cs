// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscInstrumentPanelDetail
{
    [Parameter]
    public InstrumentTypes Type { get; set; }

    [Parameter]
    public Guid InstrumentId { get; set; }

    [Parameter]
    public Guid ParentId { get; set; }

    private PanelDto _panel { get; set; } = new();

    private TscWidgetBase _widget = default!;

    protected override void OnParametersSet()
    {
        if (_panel.InstrumentId != InstrumentId || _panel.ParentId != ParentId)
        {
            _panel = new PanelDto
            {
                InstrumentId = InstrumentId,
                ParentId = ParentId,
                Id = Guid.NewGuid(),
            };
        }
        base.OnParametersSet();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
    }

    private async Task OnSubmitAsync()
    {
        var item = _widget.Value;
        await ApiCaller.PanelService.AddAsync(item);
        if (item.Type == InstrumentTypes.Tabs)
        {
            var tabs = ((TabsPanelDto)item).Tabs;
            foreach (var tab in tabs)
            {
                tab.ParentId = item.Id;
                tab.Type = InstrumentTypes.TabItem;
                await ApiCaller.PanelService.AddAsync(tab);
            }
        }

        _panel.Id = Guid.NewGuid();
        await CallParent(OperateCommand.Success, item);
    }
}
