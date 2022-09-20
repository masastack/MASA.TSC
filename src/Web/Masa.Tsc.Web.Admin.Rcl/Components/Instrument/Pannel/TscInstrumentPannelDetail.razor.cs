// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscInstrumentPannelDetail
{
    [Parameter]
    public InstrumentTypes Type { get; set; }

    [Inject]
    public AddInstrumentsDto _model { get; set; }

    private TscWidgetBase _widget = default!;

    protected override Task OnParametersSetAsync()
    {
        return base.OnParametersSetAsync();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
    }

    private async Task OnSubmitAsync()
    {
        await PopupService.AlertAsync("Success", AlertTypes.Success);
    }
}
