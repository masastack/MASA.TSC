// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations.Panel.Chart.Models;

public interface IPanelValue
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public PanelTypes PanelType { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    public List<UpsertPanelDto> ChildPanels { get; set; }

    public List<PanelMetricDto> Metrics { get; set; }

    public Dictionary<ExtensionFieldTypes, object?> ExtensionData { get; set; }
}
