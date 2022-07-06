// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Teams.Aggregates;

public class App : Entity<Guid>
{
    public string Name { get; set; }

    public string Description { get; set; }
}
