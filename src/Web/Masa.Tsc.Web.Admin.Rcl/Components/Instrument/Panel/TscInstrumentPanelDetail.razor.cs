// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscInstrumentPanelDetail
{
    [Parameter]
    public PanelTypes Type { get; set; }

    [Parameter]
    public Guid InstrumentId { get; set; }

    [Parameter]
    public Guid ParentId { get; set; }

    [Parameter]
    public int Index { get; set; }

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

    private async Task OnSubmitAsync()
    {
        var item = _widget.Value;
        item.Sort = Index;
        await ApiCaller.PanelService.AddAsync(item);
        if (item.Type == PanelTypes.Tabs)
        {
            var tabs = ((TabsPanelDto)item).Tabs;
            int sort = 1;
            foreach (var tab in tabs)
            {
                tab.Sort = sort++;
                tab.ParentId = item.Id;
                tab.Type = PanelTypes.TabItem;
                await ApiCaller.PanelService.AddAsync(tab);
            }
        }

        _panel.Id = Guid.NewGuid();
        await OnCallParent(OperateCommand.Success, item);
    }
}
