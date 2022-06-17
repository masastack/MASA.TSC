// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.


namespace Masa.Tsc.Service.Admin.Domain.Teams.Events.Query;

public record TeamDetailQuery : Query<TeamDto>
{
    public Guid Id { get; set; }

    public string AppId { get; set; }

    public override TeamDto Result { get; set; }
}
