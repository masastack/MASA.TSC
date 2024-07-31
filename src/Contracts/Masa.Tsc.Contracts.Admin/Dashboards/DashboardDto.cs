// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Dashboards;

public class DashboardDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public FolderDto Folder { get; set; }

    public bool IsRoot { get; set; }

    public string Layer { get; set; } = MetricConstants.DEFAULT_LAYER;

    public ModelTypes Model { get; set; }

    public string Type { get; set; }
}
