﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards;

public partial class Dashboard
{
    string? _search;
    int _page = 1;
    int _pageSize = 10;

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    string Search
    {
        get { return _search ?? ""; }
        set
        {
            _search = value;
        }
    }

    int Page
    {
        get { return _page; }
        set
        {
            _page = value;
            GetFoldersAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    int PageSize
    {
        get { return _pageSize; }
        set
        {
            _page = 1;
            _pageSize = value;
            GetFoldersAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    bool ExpandAll { get; set; } = true;

    long Total { get; set; }

    List<FolderDto> Folders { get; set; } = new List<FolderDto>();

    IEnumerable<DashboardDto> Dashboards => Folders.SelectMany(folder =>
    {
        folder.Dashboards.ForEach(dashboard => dashboard.Folder = folder);
        return folder.Dashboards;
    });

    ModeTypes Mode { get; set; } = ModeTypes.Folder;

    bool AddFolderDialogVisible { get; set; }

    bool UpdateFolderDialogVisible { get; set; }

    bool AddDashboardDialogVisible { get; set; }

    bool UpdateDashboardDialogVisible { get; set; }

    Guid CurrentDashboardId { get; set; }

    Guid CurrentFolderId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetFoldersAsync();
        Folders.Where(forder => forder.Dashboards.Any()).ToList().ForEach(folder => folder.IsActive = true);
    }

    async Task GetFoldersAsync()
    {
        Loading = true;
        var result = await ApiCaller.DirectoryService.GetListAsync(Page, PageSize, Search);
        var folders = result.Result ?? new();
        folders.ForEach(folder =>
        {
            folder.IsActive = ExpandAll || (Folders.FirstOrDefault(item => item.Id == folder.Id)?.IsActive ?? false);
        });
        Folders = folders;
        Total = result.Total;
        Loading = false;
    }

    async Task OnAddDashboardSuccessAsync(AddDashboardDto dashboard)
    {
        await GetFoldersAsync();
        Folders.First(folder => folder.Id == dashboard.Folder).IsActive = true;
    }

    async Task OnUpdateDashboardSuccessAsync(UpdateDashboardDto dashboard)
    {
        await GetFoldersAsync();
        Folders.First(folder => folder.Id == dashboard.Folder).IsActive = true;
    }

    List<DataTableHeader<DashboardDto>> GetHeaders() => new()
    {
        new() { Text = I18n.T(nameof(DashboardDto.Name)), Value = nameof(DashboardDto.Name), Sortable = false },
        new() { Text = I18n.Dashboard(nameof(DashboardDto.Folder)), Value = nameof(DashboardDto.Folder), Sortable = false },
        new() { Text = I18n.Dashboard(nameof(DashboardDto.Layer)), Value = nameof(DashboardDto.Layer), Sortable = false },
        new() { Text = I18n.Dashboard(nameof(DashboardDto.Model)), Value = nameof(DashboardDto.Model), Sortable = false },
        new() { Text = I18n.T("Action"), Value = "Action", Sortable = false, Align = DataTableHeaderAlign.Center, Width = "105px" },
    };

    void OpenAddFolderDialog()
    {
        AddFolderDialogVisible = true;
    }

    void OpenUpdateFolderDialog(FolderDto folder)
    {
        UpdateFolderDialogVisible = true;
        CurrentFolderId = folder.Id;
    }

    void OpenAddDashboardDialog()
    {
        AddDashboardDialogVisible = true;
    }

    void OpenUpdateDashboardDialog(DashboardDto dashboard)
    {
        UpdateDashboardDialogVisible = true;
        CurrentDashboardId = dashboard.Id;
    }

    async Task OpenRemoveDashboardDialogAsync(DashboardDto dashboard)
    {
        var confirm = await OpenConfirmDialog(I18n.Dashboard("Delete Dashboard"), string.Format(I18n.Dashboard("Are you sure delete dashboard \"{0}\"?"), dashboard.Name));
        if (confirm) await RemoveDashboardAsync(dashboard.Id);
    }

    async Task RemoveDashboardAsync(Guid dashboardId)
    {
        Loading = true;
        await ApiCaller.InstrumentService.DeleteAsync(dashboardId);
        OpenSuccessMessage(I18n.Dashboard("Delete dashboard data success"));
        await GetFoldersAsync();
        Loading = false;
    }

    async Task OpenRemoveFolderDialogAsync(FolderDto folder)
    {
        var confirm = await OpenConfirmDialog(I18n.Dashboard("Delete Folder"), string.Format(I18n.Dashboard("Are you sure delete folder \"{0}\"?"), folder.Name));
        if (confirm) await RemoveFolderAsync(folder.Id);
    }

    async Task RemoveFolderAsync(Guid folderId)
    {
        Loading = true;
        await ApiCaller.DirectoryService.DeleteAsync(folderId);
        OpenSuccessMessage(I18n.Dashboard("Delete folder data success"));
        await GetFoldersAsync();
        Loading = false;
    }

    void NavigateToConfiguration(DashboardDto dashboard)
    {
        NavigationManager.NavigateToDashboardConfiguration(dashboard.Id.ToString(), default!);
    }

    async Task OnSearchAsync()
    {
        _page = 1;
        ExpandAll = true;

        await GetFoldersAsync();
    }

    void ExpandOrCloseAll(bool expand = true)
    {
        ExpandAll = !expand;
        Folders.ForEach(folder =>
        {
            folder.IsActive = ExpandAll;
        });
    }
}
