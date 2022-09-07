// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Components;

public partial class TeamSearch
{    
    [Parameter]
    public DateTime? Start { get; set; } = DateTime.Now.Date;

    [Parameter]
    public DateTime? End { get; set; } = DateTime.Now;

    [Parameter]
    public string ProjectId { get; set; } = default!;

    [Parameter]
    public string Keyword { get; set; } = default!;

    private string _bgColor = "#A3AED0";

    private List<ProjectDto> Projects { get; set; } = new List<ProjectDto>
    {
        new ProjectDto
        {
             Name="Masa_Auth",
             Identity="masa-auth"
        },
        new ProjectDto
        {
            Name="Masa_Pm",
             Identity="masa-pm"
        },
        new ProjectDto
        {
            Name="Masa_Dcc",
             Identity="masa-dcc"
        }
    };

}
