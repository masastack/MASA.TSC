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

    private AddPanelDto _panel { get; set; } = new();

    private TscWidgetBase _widget = default!;

    protected override Task OnInitializedAsync()
    {
        if (_panel.InstrumentId != InstrumentId || _panel.ParentId != ParentId)
        {
            _panel = new AddPanelDto
            {
                InstrumentId = InstrumentId,
                ParentId = ParentId,
                Id = Guid.NewGuid(),
            };
        }
        return base.OnInitializedAsync();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
    }

    private async Task OnSubmitAsync()
    {
        var item = _widget.Item;
        if (item.Type == InstrumentTypes.Widget)
        {
            //sendData.Metrics =;
        }
        await ApiCaller.PanelService.AddAsync(item);
        await CallParent("save", item);
    }
}
