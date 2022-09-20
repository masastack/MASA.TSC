// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscInstrumentAdd
{
    [Parameter]
    public EventCallback<AddInstrumentsDto> OnSuccess { get; set; }

    [Inject]
    public AddInstrumentsDto _model { get; set; }

    protected override void OnInitialized()
    {
        _model.UserId = CurrentUserId;
        _model.DirectoryId = Guid.NewGuid();
        base.OnInitialized();
    }

    private async Task OnSubmitAsync()
    {
        if (OnSuccess.HasDelegate)
            await OnSuccess.InvokeAsync(_model);
        //await PopupService.AlertAsync("Success", AlertTypes.Success);
    }
}
