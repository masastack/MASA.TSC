// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class AppService : ServiceBase
{
    public AppService(IServiceCollection services) : base(services, "/api/app")
    {
        App.MapGet($"{BaseUri}", GetAppsAsync);
    }

    public async Task<List<AppDto>> GetAppsAsync([FromServices] IEventBus eventBus, [FromQuery] string projectId)
    {
        var query = new AppsQuery
        {
            ProjectId = projectId
        };
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
