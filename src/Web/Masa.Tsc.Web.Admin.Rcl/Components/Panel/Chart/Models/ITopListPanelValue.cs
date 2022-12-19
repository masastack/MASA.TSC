// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Chart.Models;

public interface ITopListPanelValue: IPanelValue
{
    public string Color { get; set; }

    public string SystemIdentity { get; set; }

    public string Desc { get; set; }

    public int MaxCount { get; set; }

    public string ChartType { get; set; }
}
