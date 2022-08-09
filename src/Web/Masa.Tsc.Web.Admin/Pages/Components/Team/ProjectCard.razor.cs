// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Components;

public partial class ProjectCard
{
    private int rows = 0;
    private int currentRow = 0;
    private ProjectDto Project;
    private bool isShow = false;

    private void Click(ProjectDto dto)
    {
        Project = dto;
        isShow = true;
    }

    private string GetPardddd(int rowIndex, int total)
    {
        if (total - RowCount == 0)
        {
            if (rowIndex % 2 == 1)
                return "hex-even";
            else
                return "";
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

    protected override void OnParametersSet()
    {
        if (Projects != null)
        {
            rows = Projects.Count / RowCount;
            if (Projects.Count % RowCount > 0)
                rows += 1;
            currentRow = 0;
        }

        base.OnParametersSet();
    }

    private static List<AppDto> apps = new List<AppDto>
    {
        new AppDto
       {
           Id="1",
            Name="masa-auth-web",
             Identity="masa-auth-web",
              ServiceType= BuildingBlocks.BasicAbility.Pm.Enum.ServiceTypes.WebApi,
               Status= Contracts.Admin.Enums.MonitorStatuses.Normal
       },new AppDto
       {
           Id="1",
            Name="masa-auth-api",
             Identity="masa-auth-api",
              ServiceType= BuildingBlocks.BasicAbility.Pm.Enum.ServiceTypes.WebApi,
               Status= Contracts.Admin.Enums.MonitorStatuses.Normal
       },new AppDto
       {
           Id="1",
            Name="masa-auth-gateway",
             Identity="masa-auth-gateway",
              ServiceType= BuildingBlocks.BasicAbility.Pm.Enum.ServiceTypes.WebApi,
               Status= Contracts.Admin.Enums.MonitorStatuses.Normal
       }
    };

    [Parameter]
    public int RowCount { get; set; } = 3;

    [Parameter]
    public List<ProjectOverViewDto> Projects { get; set; }

    private TeamDto TeamData = new TeamDto
    {
        Name = "Masa Stack Component Steam",
        Avatar = "https://cdn.masastack.com/stack/images/website/masa-blazor/jack.png",
        AppTotal = 20,
        ProjectTotal = 50,
        Description = "asdasdasdashujbnhsdfsfdbvx, dsifhsndi hj9idf sfgsdfsgbdfojkmopjm erfgsdvsdfsdfsdfsdfsd",
        Admins = new List<UserDto>
            {
                new UserDto
                {
                    DisplayName="tdadd",
                    Avatar="https://cdn.masastack.com/stack/images/website/masa-blazor/jack.png"

                }

            }

    };
}
