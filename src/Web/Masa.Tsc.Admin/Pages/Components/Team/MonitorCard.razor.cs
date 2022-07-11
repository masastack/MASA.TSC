// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Admin.Rcl.Pages.Components;

public partial class MonitorCard
{
    [Parameter]
    public int Error { get { return GetValue(2); } set { SetData(value, 2); } }

    [Parameter]
    public int Monitor { get { return GetValue(0); } set { SetData(value, 0); } }

    [Parameter]
    public int Normal { get { return GetValue(3); } set { SetData(value, 3); } }

    [Parameter]
    public int Warn { get { return GetValue(1); } set { SetData(value, 1); } }

    /// <summary>
    /// 0monitor 1warn 2error 3normal 
    /// </summary>
    public List<AppMonitorViewDto> Data { get; set; } = new List<AppMonitorViewDto>
        {
            new AppMonitorViewDto
            {
                Name="MONITORING",
                Color="#A3AED0",
                Total=0,
                Icon="mdi-chart-line-variant"
            },
            new AppMonitorViewDto
            {
                Name="WARN",
                Color="#C07401",
                Total=0,
                Icon="mdi-bell-ring"
            },
            new AppMonitorViewDto
            {
                Name="ERROR",
                Color="#F80E1C",
                Total=0,
                Icon="mdi-bell"
            },
            new AppMonitorViewDto
            {
                Name="NORMAL",
                Color="#299F00",
                Total=0,
                Icon="mdi-shield"
            }
        };

    private void SetData(int value, int index)
    {
        AppMonitorViewDto find = Data[index];
        find.Total = value;
    }

    private int GetValue(int index)
    {
        return Data[index].Total;
    }
}

