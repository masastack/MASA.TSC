// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Teams.Repositories;

public interface IProjectRepository
{
    Task<IEnumerable<Project>> GetAllAsync(Guid teamId);

    Task<Project> GetDetailAsync(Guid id);
}
