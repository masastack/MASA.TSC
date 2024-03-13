// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class AppService : ServiceBase
{
    public AppService() : base("/api/app")
    {

    }

    public async Task<List<AppDto>> GetListAsync([FromServices] IEventBus eventBus, string projectId)
    {
        var query = new AppsQuery(projectId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<long> GetErrorCountAsync([FromServices] IEventBus eventBus, string appid, string start, string end)
    {
        var query = new AppErrorCountQuery(appid, start.ParseTime(), end.ParseTime());
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
