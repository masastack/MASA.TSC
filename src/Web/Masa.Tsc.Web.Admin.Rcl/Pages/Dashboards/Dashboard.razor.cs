// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards;

public partial class Dashboard
{
    string? _search;
    int _page = 1;
    int _pageSize = 10;
    bool _expandAll = true;

    string Search
    {
        get { return _search ?? ""; }
        set
        {
            _search = value;
            _page = 1;
            GetFoldersAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
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

    bool ExpandAll
    {
        get => Folders.Any(folder => folder.ISActive);
        set 
        {
            Folders.ForEach(folder => folder.ISActive = value);
            _expandAll = value; 
        }
    }

    long Total { get; set; }

    IEnumerable<FolderDto> Folders { get; set; }

    IEnumerable<DashboardDto> Dashboards => Folders.SelectMany(folder => 
    {
        folder.Dashboards.ForEach(dashboard => dashboard.Folder = folder);
        return folder.Dashboards;
    });

    Modes Mode { get; set; } = Modes.Folder;

    bool AddFolderDialogVisible { get; set; }

    bool UpdateFolderDialogVisible { get; set; }

    bool AddDashboardDialogVisible { get; set; }

    bool UpdateDashboardDialogVisible { get; set; }

    Guid CurrentDashboardId { get; set; }

    Guid CurrentFolderId { get; set; }

    protected override string? PageName { get; set; } = "DashboardBlock";

    protected override async Task OnInitializedAsync()
    {
        await GetFoldersAsync();
    }

    async Task GetFoldersAsync()
    {
        Loading = true;
        //todo search 
        Folders = new List<FolderDto> 
        {
            new FolderDto
            {
                Id = Guid.NewGuid(),
                Name ="订单",
                Dashboards = new List<DashboardDto>
                {
                    new DashboardDto
                    {
                        Id = Guid.NewGuid(),
                        Name = "历史订单",
                        IsRoot = true,
                        Layer = LayerTypes.K8s,
                        Model = ModelTypes.Service
                    },
                    new DashboardDto
                    {
                        Id = Guid.NewGuid(),
                        Name = "当日订单",
                        IsRoot = true,
                        Layer = LayerTypes.Mesh,
                        Model = ModelTypes.ProcessRelation
                    },
                    new DashboardDto
                    {
                        Id = Guid.NewGuid(),
                        Name = "亏损订单",
                        IsRoot = true,
                        Layer = LayerTypes.Mysql,
                        Model = ModelTypes.Endpoint
                    }
                }
            },
           new FolderDto
            {
                Id = Guid.NewGuid(),
                Name ="用户",
                Dashboards = new List<DashboardDto>
                {
                    new DashboardDto
                    {
                        Id = Guid.NewGuid(),
                        Name = "所有用户",
                        IsRoot = false,
                        Layer = LayerTypes.VirtualGateway,
                        Model = ModelTypes.All
                    },
                    new DashboardDto
                    {
                        Id = Guid.NewGuid(),
                        Name = "员工",
                        IsRoot = false,
                        Layer = LayerTypes.Mq,
                        Model = ModelTypes.EndpointRelation
                    },
                    new DashboardDto
                    {
                        Id = Guid.NewGuid(),
                        Name = "第三方用户",
                        IsRoot = true,
                        Layer = LayerTypes.Mysql,
                        Model = ModelTypes.Endpoint
                    }
                }
            }
        };
        Total = 6;
        await Task.CompletedTask;
        Loading = false;
    }

    List<DataTableHeader<DashboardDto>> GetHeaders() => new()
    {
        new() { Text = T(nameof(DashboardDto.Name)), Value = nameof(DashboardDto.Name), Sortable = false },
        new() { Text = T(nameof(DashboardDto.Folder)), Value = nameof(DashboardDto.Folder), Sortable = false },
        new() { Text = T(nameof(DashboardDto.Layer)), Value = nameof(DashboardDto.Layer), Sortable = false },
        new() { Text = T(nameof(DashboardDto.Model)), Value = nameof(DashboardDto.Model), Sortable = false },
        new() { Text = T(nameof(DashboardDto.IsRoot)), Value = nameof(DashboardDto.IsRoot), Sortable = false },
        new() { Text = T("Action"), Value = "Action", Sortable = false, Align = DataTableHeaderAlign.Center, Width="105px" },
    };

    async Task RefreshAsync()
    {
        Loading = true;
        await GetFoldersAsync();
        OpenSuccessMessage(T("Refresh data success"));
        Loading = false;
    }


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

    async Task SwitchRootAsync(DashboardDto dashboard)
    {
        Loading = true;
        dashboard.IsRoot = !dashboard.IsRoot;
        await Task.CompletedTask;
        OpenSuccessMessage(T("Action success"));
        await GetFoldersAsync();
        Loading = false;
    }

    async Task OpenRemoveDashboardDialogAsync(DashboardDto dashboard)
    {
        var confirm = await OpenConfirmDialog(T("Delete Dashboard"), T("Are you sure delete dashboard \"{0}\"?", dashboard.Name));
        if (confirm) await RemoveDashboardAsync(dashboard.Id);
    }

    async Task RemoveDashboardAsync(Guid dashboardId)
    {
        Loading = true;
        await Task.CompletedTask;
        OpenSuccessMessage(T("Delete dashboard data success"));
        await GetFoldersAsync();
        Loading = false;
    }

    async Task OpenRemoveFolderDialogAsync(FolderDto folder)
    {
        var confirm = await OpenConfirmDialog(T("Delete Folder"), T("Are you sure delete folder \"{0}\"?", folder.Name));
        if (confirm) await RemoveDashboardAsync(folder.Id);
    }

    async Task RemoveFolderAsync(Guid folderId)
    {
        Loading = true;
        await Task.CompletedTask;
        OpenSuccessMessage(T("Delete folder data success"));
        await GetFoldersAsync();
        Loading = false;
    }
}
