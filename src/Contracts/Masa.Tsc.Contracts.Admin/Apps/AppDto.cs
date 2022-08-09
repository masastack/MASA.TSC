// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class AppDto
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Identity { get; set; }

    public ServiceTypes ServiceType { get; set; }

    public MonitorStatuses Status { get; set; }
}
