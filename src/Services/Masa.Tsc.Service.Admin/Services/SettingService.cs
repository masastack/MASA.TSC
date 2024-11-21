// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

internal class SettingService : ServiceBase
{
    public SettingService() : base("/api/settings") { }

    public SettingDto GetStorage() => new() { IsClickhouse = ConfigConst.StorageSetting.IsClickhouse, IsElasticsearch = ConfigConst.StorageSetting.IsClickhouse };
}
