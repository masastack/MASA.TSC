// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class ProjectCard
{
    [Parameter]
    public int RowCount { get { return _rowSize; } set { _rowSize = value; SetTotalRows(); } }

    [Parameter]
    public List<ProjectOverviewDto> Projects { get { return _projects; } set { _projects = value; SetTotalRows(); } }

    private List<ProjectOverviewDto> _projects;
    private int _rowSize = 3;
    private int _totalRows = 0;
    private ProjectDto _current;
    private bool _showDialog = false;

    private void OnItemClick(ProjectDto item)
    {
        _current = item;
        _showDialog = true;
    }

    private string GetPaddingClass(int rowIndex, int total)
    {
        if (rowIndex % 2 == 1)
            return "hex-even";
        else
            return "";

        if (total - RowCount == 0)
        {
           
        }

        var count = RowCount - total;
        if (rowIndex % 2 == 0)
        {
            if ((RowCount + count) % 2 == 0)
                return "hex-even";
        }
        else if ((RowCount + count) % 2 == 1)
        {
            return "hex-even";
        }

        return "";
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