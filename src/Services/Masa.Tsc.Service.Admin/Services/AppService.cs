// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class AppService : ServiceBase
{
    public AppService() : base("/api/app")
    {
        //App.MapGet($"{BaseUri}/list{{projectId}}", GetListAsync);
    }

    public async Task<List<AppDto>> GetListAsync([FromServices] IEventBus eventBus, string projectId)
    {
        var query = new AppsQuery(projectId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
