// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscInstrumentAdd
{    
    [Parameter]
    public AddInstrumentDto Item { get; set; }

    [Parameter]
    public string DirectoryName { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    private async Task OnSubmitAsync()
    {
        await ApiCaller.InstrumentService.AddAsync(Item);
        await OnCallParent(OperateCommand.Add, "instrument",Item);        
    }
}
