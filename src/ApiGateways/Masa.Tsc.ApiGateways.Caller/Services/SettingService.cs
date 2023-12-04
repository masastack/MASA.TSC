// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class SettingService: BaseService
{
    public SettingService(ICaller caller) : base(caller, "/api/settings") { }

    public async Task<SettingDto> GetStorage() => await Caller.GetAsync<SettingDto>($"{RootPath}/storage") ?? default!;
}
