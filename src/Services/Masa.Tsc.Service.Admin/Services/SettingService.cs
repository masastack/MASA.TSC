// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class SettingService : ServiceBase
{
    public SettingService(IServiceCollection services) : base(services, "/api/setting")
    {
        App.MapGet($"{BaseUri}/{{userId}}", GetAsync);
        App.MapPost($"{BaseUri}", SetAsync);
    }

    private async Task SetAsync([FromServices] IEventBus eventBus, [FromBody] SettingDto setting)
    {
        await eventBus.PublishAsync(new SetSettingCommand(setting.Language, setting.IsEnable, setting.TimeZone, setting.TimeZoneOffset, setting.Interval, setting.UserId));
    }

    private async Task<SettingDto> GetAsync([FromServices] IEventBus eventBus, Guid userId)
    {
        var query = new SettingQuery(userId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
