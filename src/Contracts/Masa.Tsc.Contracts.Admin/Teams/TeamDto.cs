// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class TeamDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Avatar { get; set; }

    public string Description { get; set; }

    public List<UserDto> Admins { get; set; }

    public int ProjectTotal { get; set; }

    public int AppTotal { get; set; }

    public ProjectDto CurrentProject { get; set; }

    public string CurrentAppId { get; set; }
}

