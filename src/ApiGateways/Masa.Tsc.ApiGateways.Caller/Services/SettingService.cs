// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class SettingService : BaseService
{
    public SettingService(ICaller caller, TokenProvider tokenProvider) : base(caller, "/api/setting", tokenProvider) { }

    public async Task<SettingDto> GetAsync(Guid userId) => (await Caller.GetAsync<SettingDto>($"{RootPath}/{userId}"))!;

    public async Task SetAsync(SettingDto model) => await Caller.PostAsync($"{RootPath}", model);
}
