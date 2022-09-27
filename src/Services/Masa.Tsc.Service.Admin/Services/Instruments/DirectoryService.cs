﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class DirectoryService : ServiceBase
{
    public DirectoryService() : base("/api/Instrument/directory")
    {
        App.MapDelete($"{BaseUri}/{{id}}/{{userId}}", DeleteAsync);
        App.MapGet($"{BaseUri}/{{userId}}/{{id}}", GetAsync);
        App.MapGet($"{BaseUri}/tree/{{userId}}/{{isContainsInstrument}}", GetTreeAsync);
    }

    public async Task<IEnumerable<DirectoryTreeDto>> GetTreeAsync([FromServices] IEventBus eventBus, Guid userId, bool isContainsInstrument)
    {
        var query = new DirectoryTreeQuery(userId, isContainsInstrument);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<DirectoryDto> GetAsync([FromServices] IEventBus eventBus, Guid userId, Guid id)
    {
        var query = new DirectoryQuery(id, userId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task AddAsync([FromServices] IEventBus eventBus, [FromBody] AddDirectoryDto param)
    {
        var query = new AddDirectoryCommand(param.Name, param.Sort, param.ParentId, param.UserId);
        await eventBus.PublishAsync(query);
    }

    public async Task UpdateAsync([FromServices] IEventBus eventBus, [FromBody] UpdateDirectoryDto param)
    {
        var query = new UpdateDirectoryCommand(param.Id, param.Name, param.Sort, param.UserId);
        await eventBus.PublishAsync(query);
    }

    public async Task DeleteAsync([FromServices] IEventBus eventBus, Guid id, Guid userId)
    {
        var query = new RemoveDirectoryCommand(id, userId);
        await eventBus.PublishAsync(query);
    }
}
