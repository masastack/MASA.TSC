// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class ProjectHexagon : IAsyncDisposable
{
    [Parameter]
    public int RowCount { get { return _rowSize; } set { _rowSize = value; SetTotalRows(); } }

    [Parameter]
    public List<ProjectOverviewDto> Projects { get { return _projects; } set { _projects = value; SetTotalRows(); } }

    private List<ProjectOverviewDto> _projects;
    private int _rowSize = 3;
    private int _totalRows = 0;
    private ProjectDto _current;

    //[Parameter]
    public List<HexagonalMeshViewModel> Value { get; set; } = new();

    private IJSObjectReference _helper = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        //if (!firstRender)
        //{
        //    return;
        //}

        if (_projects != null && _projects.Any())
        {
            //_totalRows = _projects.Count / _rowSize;
            //int temp = _projects.Count % _rowSize;
            //if (temp > 0)
            //    _totalRows += 1;
            int rowStart = _totalRows - 1;
            int colStart = 0;
            foreach (var item in _projects)
            {
                Value.Add(new HexagonalMeshViewModel
                {
                    Key = item.Identity,
                    Name = item.Name,
                    Q = colStart,
                    R = rowStart,
                    State = item.Status,
                    Items = item.Apps
                });
                colStart++;
                if (colStart - _rowSize == 0)
                {
                    colStart = 0;
                    rowStart -= 1;
                }
            }

            _helper = await Js.InvokeAsync<IJSObjectReference>("import", "./_content/Masa.Tsc.Web.Admin.Rcl/js/antv-g2/hexagonalMesh-helper.js");

            await InitChart();
            await AddPolygon();
            await Render();
        }
    }

    public async Task InitChart()
    {
        await _helper.InvokeVoidAsync("init", Ref, Value);
    }

    public async Task AddPolygon()
    {
        await _helper.InvokeVoidAsync("addPolygon", Ref, Value);
    }

    public async Task Render()
    {
        await _helper.InvokeVoidAsync("render", Ref);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _helper.InvokeVoidAsync("destroy", Ref);

            if (_helper != null)
            {
                await _helper.DisposeAsync();
            }
        }
        catch (Exception)
        {
        }
    }

    [JSInvokable]
    public void OnItemClick(string projectId)
    {
        var find = _projects.FirstOrDefault(p => p.Identity == projectId);
        if (find != null)
        {
            _current = find;
            OpenDialog();
        }
    }

    private void SetTotalRows()
    {
        if (Projects != null && Projects.Any())
        {
            _totalRows = Projects.Count / RowCount;
            if (Projects.Count % RowCount > 0)
                _totalRows += 1;
        }
    }
}