﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class AppService : BaseService
{
    public AppService(ICaller caller) : base(caller, "/api/app") { }

    public async Task<List<AppDto>> GetAppsAsync(string projectId) => await Caller.GetAsync<List<AppDto>>($"{RootPath}?projectId={projectId}") ?? default!;

    public async Task<int> GetAppErrorCountAsync(string appid, DateTime start, DateTime end) => await Caller.GetAsync<int>($"{RootPath}/errorCount", new { appid, start, end });
}