// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Dashboards;

public class AddDashboardDto
{
    public string Name { get; set; }

    public Guid Folder { get; set; }

    public bool IsRoot { get; set; }

    public int Order {  get; set; }

    public LayerTypes Layer { get; set; }

    public ModelTypes Model { get; set; }

    public string Type { get; set; } = "";
}
