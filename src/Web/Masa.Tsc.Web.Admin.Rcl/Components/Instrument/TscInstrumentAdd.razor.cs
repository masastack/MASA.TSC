// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscInstrumentAdd
{
    [Parameter]
    public EventCallback<AddInstrumentDto> OnSuccess { get; set; }

    [Inject]
    public AddInstrumentDto Item { get; set; }

    protected override void OnInitialized()
    {
        Item.DirectoryId = Guid.NewGuid();
        base.OnInitialized();
    }

    private async Task OnSubmitAsync()
    {
        if (OnSuccess.HasDelegate)
            await OnSuccess.InvokeAsync(Item);
        //await PopupService.AlertAsync("Success", AlertTypes.Success);
    }
}
