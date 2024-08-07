﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Dashboards;

public class AddDashboardDto
{
    public string Name { get; set; }

    public Guid Folder { get; set; }

    public bool IsRoot { get; set; }

    public int Order { get; set; }

    public string Layer { get; set; } = string.Empty;

    public ModelTypes Model { get; set; } = ModelTypes.All;

    public string Type { get; set; } = "";
}
