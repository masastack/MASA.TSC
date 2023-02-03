// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class Example2
{
    private List<ProjectOverviewDto> _projects = default!;
    private TeamSearchModel? _teamSearchModel = null;
    private int _error, _warn, _monitor, _normal;
    private bool _isLoad = true;

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
    }

    protected override async void OnAfterRender(bool firstRender)
    {
        if (_isLoad)
        {
            await LoadData();
            StateHasChanged();
            _isLoad = false;
        }
        base.OnAfterRender(firstRender);
    }

    private async Task LoadData()
    {
        DateTime start = DateTime.MinValue, end = DateTime.MinValue;
        if (_teamSearchModel != null)
        {
            if (_teamSearchModel.Start.HasValue)
                start = _teamSearchModel.Start.Value;
            if (_teamSearchModel.End.HasValue)
                end = _teamSearchModel.End.Value;
        }
        var data = await ApiCaller.ProjectService.OverviewAsync(new RequestTeamMonitorDto
        {
            EndTime = end,
            StartTime = start,
            Keyword = _teamSearchModel?.Keyword ?? default!,
            ProjectId = _teamSearchModel?.AppId ?? default!,
            UserId = CurrentUserId
        });


        if (data != null)
        {
            _error = data.Monitor.ServiceError;
            _warn = data.Monitor.ServiceWarn;
            _monitor = data.Monitor.ServiceTotal;
            _normal = data.Monitor.Normal;
            _projects = data.Projects;
        }
    }

    private async Task<IEnumerable<ProjectDto>> LoadProjectAsync()
    {
        if (_projects == null)
        {
            await LoadData();
        }

        if (_projects == null || !_projects.Any())
            return default!;
        return _projects.Select(x => new ProjectDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Identity = x.Identity,
            LabelName = x.LabelName,
            TeamId = x.TeamId,
            Apps = x.Apps,
        });
    }

    private async Task OnSearch(TeamSearchModel query)
    {
        _teamSearchModel = query;
        await LoadData();
        StateHasChanged();
    }


    private List<HexagonalMeshViewModel> Data = new();

    protected override async Task OnInitializedAsync()
    {
        FillData();
        await base.OnInitializedAsync();
    }

    public void FillData()
    {
        var items = new List<AppDto>
        {
            new AppDto{
                Name="masa-auth-web-admin",
                Status = MonitorStatuses.Normal
            },
            new AppDto{
                Name="masa-auth-service...",
                Status = MonitorStatuses.Normal
            },
            new AppDto{
                Name="masa-auth-web-admin",
                Status = MonitorStatuses.Normal
            }
        };
        Data = new List<HexagonalMeshViewModel>
        {
            new HexagonalMeshViewModel
            {
                Key="0-0",
                Q=0,
                R=0,
                Name="auth",
                State = MonitorStatuses.Normal,
                Items=items
            },
            new HexagonalMeshViewModel
            {
                Key="0-1",
                Q=1,
                R=0,
                Name="auth",
                State = MonitorStatuses.Normal,
                Items=items
            },
            new HexagonalMeshViewModel
            {
                Key="0-2",
                Q=2,
                R=0,
                Name="auth",
                State = MonitorStatuses.Normal,
                Items=items
            },
            new HexagonalMeshViewModel
            {
                Key="0-3",
                Q=3,
                R=0,
                Name="auth",
                State = MonitorStatuses.Normal,
                Items=items
            },
            new HexagonalMeshViewModel
            {
                Key="0-4",
                Q=4,
                R=0,
                Name="auth",
                State = MonitorStatuses.Normal,
                Items=items
            },
            new HexagonalMeshViewModel
            {
                Key="0-5",
                Q=5,
                R=0,
                Name="auth",
                State = MonitorStatuses.Normal,
                Items=items
            },
            new HexagonalMeshViewModel
            {
                Key="1-0",
                Q=0,
                R=1,
                Name="auth",
                State = MonitorStatuses.Normal,
                Items=items
            },
            new HexagonalMeshViewModel
            {
                Key="1-1",
                Q=1,
                R=1,
                Name="auth",
                State = MonitorStatuses.Normal,
                Items=items
            },
            new HexagonalMeshViewModel
            {
                Key="1-2",
                Q=2,
                R=1,
                Name="auth",
                State = MonitorStatuses.Error,
                Items=items
            },
            new HexagonalMeshViewModel
            {
                Key="1-3",
                Q=3,
                R=1,
                Name="auth",
                State = MonitorStatuses.Normal,
                Items=items
            },
            new HexagonalMeshViewModel
            {
                Key="1-4",
                Q=4,
                R=1,
                Name="auth",
                State = MonitorStatuses.Normal,
                Items=items
            },
            new HexagonalMeshViewModel
            {
                Key="1-5",
                Q=5,
                R=1,
                Name="auth",
                State = MonitorStatuses.Normal,
                Items=items
            },
            new HexagonalMeshViewModel
            {
                Key="2-1",
                Q=1,
                R=2,
                Name="auth",
                State = MonitorStatuses.Normal,
                Items=items
            },
            new HexagonalMeshViewModel
            {
                Key="2-2",
                Q=2,
                R=2,
                Name="auth",
                State = MonitorStatuses.Normal,
                Items=items
            },
            new HexagonalMeshViewModel
            {
                Key="2-3",
                Q=3,
                R=2,
                Name="auth",
                State = MonitorStatuses.Normal,
                Items=items
            },
            new HexagonalMeshViewModel
            {
                Key="2-4",
                Q=4,
                R=2,
                Name="auth",
                State = MonitorStatuses.Normal,
                Items=items
            },
            new HexagonalMeshViewModel
            {
                Key="2-5",
                Q=5,
                R=2,
                Name="auth",
                State = MonitorStatuses.Normal,
                Items=items
            },
            new HexagonalMeshViewModel
            {
                Key="2-6",
                Q=6,
                R=2,
                Name="auth",
                State = MonitorStatuses.Normal,
                Items=items
            },
            new HexagonalMeshViewModel
            {
                Key="3-1",
                Q=1,
                R=3,
                Name="auth",
                State = MonitorStatuses.Normal,
                Items=items
            },
            new HexagonalMeshViewModel
            {
                Key="3-2",
                Q=2,
                R=3,
                Name="auth",
                State = MonitorStatuses.Warn,
                Items=items
            },
            new HexagonalMeshViewModel
            {
                Key="3-3",
                Q=3,
                R=3,
                Name="auth",
                State = MonitorStatuses.Normal,
                Items=items
            },
            new HexagonalMeshViewModel
            {
                Key="3-4",
                Q=4,
                R=3,
                Name="auth",
                State = MonitorStatuses.Normal,
                Items=items
            },
            new HexagonalMeshViewModel
            {
                Key="3-5",
                Q=5,
                R=3,
                Name="auth",
                State = MonitorStatuses.Normal,
                Items=items
            },
            new HexagonalMeshViewModel
            {
                Key="3-6",
                Q=6,
                R=3,
                Name="auth",
                State = MonitorStatuses.Normal,
                Items=items
            },
        };


    }
}
