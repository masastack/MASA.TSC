// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Teams.Repositories;

public interface ITeamRepository
{
    Task<List<Team>> GetAllAsync(Guid userId);

    Task<Team> GetDetail(Guid id);
}
