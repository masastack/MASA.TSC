// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application;

internal class ExceptErrorHandler
{
    private readonly IExceptErrorRepository _repository;

    public ExceptErrorHandler(IExceptErrorRepository repository)
    {
        _repository = repository;
    }

    [EventHandler]
    public async Task AddAsync(CreateExceptErrorCommand command)
    {
        if (_repository.ExceptErrors.Any(m => m.Environment == command.Data.Environment
        && m.Project == command.Data.Project
        && m.Service == command.Data.Service
        && m.Type == command.Data.Type
        && (string.IsNullOrEmpty(command.Data.Message) || m.Message == command.Data.Message)))
        {
            throw new UserFriendlyException("数据已存在");
        }
        var entity = new ExceptError
        {
            Environment = command.Data.Environment,
            Project = command.Data.Project,
            Service = command.Data.Service,
            Type = command.Data.Type,
            Message = command.Data.Message,
            Comment = command.Data.Comment,
        };
        await _repository.AddAsync(entity);        
    }

    [EventHandler]
    public async Task UpdateAsync(UpdateExceptErrorCommand command)
    {
        var entity = await _repository.FindAsync(x => x.Id == command.Id);
        if (entity == null)
        {
            throw new UserFriendlyException("数据不已存在");
        }
        if (string.IsNullOrEmpty(command.Comment) || string.Equals(command.Comment, entity.Comment))
            return;

        entity.Comment = command.Comment;
        await _repository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteAsync(DeleteExceptErrorCommand command)
    {
        var entity = await _repository.FindAsync(x => x.Id == command.Id);
        if (entity == null)
            return;
        await _repository.RemoveAsync(entity);
    }

    [EventHandler]
    public async Task DeletesAsync(DeletesExceptErrorCommand command)
    {
        var entities = await _repository.GetListAsync(x => command.Ids.Contains(x.Id));
        if (entities != null && entities.Any())
            await _repository.RemoveRangeAsync(entities);
    }
}