// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

internal class ExceptErrorService : ServiceBase
{   
    public ExceptErrorService() : base("/api/except-error")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };
    }

    public async Task AddAsync([FromServices] IEventBus eventBus,[FromServices] IUserContext userContext, [FromBody] RequestAddExceptError request)
    {
        await eventBus.PublishAsync(new CreateExceptErrorCommand(request, userContext.UserName ?? userContext.UserId!));
    }

    public async Task UpdateAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, [FromBody] RequestUpdateExceptError request)
    {
        await eventBus.PublishAsync(new UpdateExceptErrorCommand(request.Id, request.Comment, userContext.UserName ?? userContext.UserId!));
    }

    public async Task RemoveAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, string id)
    {
        await eventBus.PublishAsync(new DeleteExceptErrorCommand(id));
    }

    public async Task DeletebyIdsAsync([FromServices] IEventBus eventBus, [FromServices] IUserContext userContext, [FromBody] string[] ids)
    {
        await eventBus.PublishAsync(new DeletesExceptErrorCommand(ids));
    }

    public async Task<PaginatedListBase<ExceptErrorDto>> GetAsync([FromServices] IExceptErrorRepository repository, int page, int pageSize, string? environment, string? project, string? service, string? type, string? message)
    {
        Expression<Func<ExceptError, bool>> expression = m => true;
        if (!string.IsNullOrEmpty(environment))
            expression=expression.And(m => m.Environment == environment);
        if (!string.IsNullOrEmpty(project))
            expression = expression.And(m => m.Project == project);
        if (!string.IsNullOrEmpty(service))
            expression = expression.And(m => m.Service == service);
        if (!string.IsNullOrEmpty(type))
            expression = expression.And(m => m.Type == type);
        if (!string.IsNullOrEmpty(message))
            expression = expression.And(m => m.Message.Contains(message));
       
        var data = await repository.GetPaginatedListAsync(expression, new PaginatedOptions { Page = page, PageSize = pageSize, Sorting = new Dictionary<string, bool> { { nameof(ExceptError.CreationTime), true } } });
        if (data != null)
        {
            var result = new PaginatedListBase<ExceptErrorDto>() { Total = data.Total };
            if (data.Result != null && data.Result.Any())
            {
                result.Result = data.Result.Select(x =>x.Adapt<ExceptErrorDto>()).ToList();
            }
            return result;
        }
        return default!;
    }
}