// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class SettingService : BaseService
{
    public SettingService(ICallerProvider caller) : base(caller, "/api/setting") { }

    public async Task<SettingDto> GetAsync(Guid userId)
    {
        return (await Caller.GetAsync<SettingDto>($"{RootPath}/{userId}"))!;
    }

    public async Task SetAsync(SettingDto model)
    {
        await Caller.PostAsync($"{RootPath}", model);
    }
}
