﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Dashboards;

public class FolderDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public List<DashboardDto> Dashboards { get; set; } = new();

    #region UI field

    public bool IsActive { get; set; } = true;

    #endregion
}
