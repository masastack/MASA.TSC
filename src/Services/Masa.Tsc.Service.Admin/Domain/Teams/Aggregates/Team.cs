// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Aggregates;

public class Team : AggregateRoot<Guid>
{
    public string Name { get; set; }

    public string Avatar { get; set; }

    public string Description { get; set; }

    public List<User> Admins { get; set; }

    public List<User> Members { get; set; }
}
