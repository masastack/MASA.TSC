// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards;

public partial class FolderSelect
{
    [Parameter]
    public Guid Value { get; set; }

    [Parameter]
    public EventCallback<Guid> ValueChanged { get; set; }

    [Parameter]
    public Expression<Func<Guid>>? ValueExpression { get; set; }

    [Parameter]
    public bool Readonly { get; set; }

    protected List<FolderDto> Folders { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        var data = await ApiCaller.DirectoryService.GetListAsync(1, 999, isIncludeInstrument: false);
        if (data.Result != null && data.Result.Any())
            Folders = data.Result;

        await base.OnInitializedAsync();
    }
}
