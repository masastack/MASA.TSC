// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscInstrumentPanelView
{
    [Parameter]
    public PanelDto Value { get; set; }

    [Parameter]
    public EventCallback<PanelDto> ValueChanged { get; set; }

    [Parameter]
    public bool Readonly { get; set; }

    private TscWidgetBase _widget = default!;

    private async Task SaveAsync(bool isSave)
    {
        var item = _widget.Value;
        var sendData = new UpdatePanelDto
        {
            Id = item.Id,
            Description = item.Description ?? string.Empty,
            Height = item.Height,
            Width = item.Width,
            InstrumentId = item.InstrumentId,
            Name = item.Title,
            Sort = item.Sort
        };

        if (item.Type == InstrumentTypes.Chart)
        {
            //sendData.Metrics =;
        }
        else if (item.Type == InstrumentTypes.Tabs)
        {
            //await ApiCaller.PanelService.AddAsync

        }
        if (isSave)
            await ApiCaller.PanelService.UpdateAsync(sendData);
        Value = item;
        await CallParent(OperateCommand.Success, Value);
    }
}
