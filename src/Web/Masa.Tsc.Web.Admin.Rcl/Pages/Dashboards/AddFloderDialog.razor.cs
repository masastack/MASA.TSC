// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards;

public partial class AddFloderDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    public MForm? Form { get; set; }

    private AddFolderDto Folder { get; set; } = new();

    private async Task UpdateVisible(bool visible)
    {
        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(visible);
        }
        else
        {
            Visible = false;
        }
    }

    protected override void OnParametersSet()
    {
        if (Visible is true)
        {
            Folder = new();
        }
    }

    public async Task AddFolderAsync()
    {
        var success = Form!.Validate();
        if (success)
        {
            await ApiCaller.DirectoryService.AddAsync(Folder);
            OpenSuccessMessage(I18n.Dashboard( "Add folder data success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
        }
    }
}
