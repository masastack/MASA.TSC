// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscInstrumentPannelView
{
    [Inject]
    public AddInstrumentsDto _model { get; set; }

    [Parameter]
    public AddPanelDto Item { get; set; }

    [Parameter]
    public bool Readonly { get; set; }

    private TscWidgetBase _widget = default!;

    private async Task SaveAsync()
    { 
        var item=_widget.ToPannel();
        item.Id = Item.Id;
        await CallParent("save", item);
    }
}
