// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Caller.Services;

public class AppService : BaseService
{
    public AppService(ICallerProvider caller) : base(caller, "/api/app") { }

    public async Task<List<AppDto>> GetAppsAsync(string projectId)
    {
        return await Caller.GetAsync<List<AppDto>>($"{RootPath}?projectId={projectId}") ?? default!;
    }
}
