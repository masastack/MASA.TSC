// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

internal class SettingService : ServiceBase
{
    public SettingService() : base("/api/settings")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };
    }

    public SettingDto GetStorage() => new() { IsClickhouse = ConfigConst.StorageSetting.IsClickhouse, IsElasticsearch = ConfigConst.StorageSetting.IsClickhouse };

    public CubejsSettingDto GetCubejs([FromServices] IServiceProvider serviceProvider)
    {
        (string endpoint, string token) = serviceProvider.GetCubeSetting();
        return new CubejsSettingDto() { Endpoint = endpoint, Token = token };
    }
}
