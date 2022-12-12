// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards.Configurations;

public partial class Configuration
{
    [Parameter]
    public string DashboardId { get; set; }

    int AppId { get; set; }

    string Search { get; set; }

    bool IsEdit { get; set; }

    DateTimeOffset StartTime { get; set; } = DateTimeOffset.Now.AddDays(-1);

    DateTimeOffset EndTime { get; set; } = DateTimeOffset.Now;

    public List<ComponentTypes> GetComponentTypes(ComponentTypes type = default)
    {
        if(type == default)
        {
            return new List<ComponentTypes> 
            {
                ComponentTypes.Tab,
                ComponentTypes.Text,
                ComponentTypes.Panel,
                ComponentTypes.Topology,
                ComponentTypes.Log,
                ComponentTypes.Trace,
                ComponentTypes.Indicators
            };
        }
        return new List<ComponentTypes>();
    }

    public void AddCompontent(ComponentTypes type)
    {

    }
}
