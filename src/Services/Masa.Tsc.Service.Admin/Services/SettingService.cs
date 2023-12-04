// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services;

public class SettingService : ServiceBase
{
    public SettingService() : base("/api/settings") { }

    public SettingDto GetStorage() => new() { IsClickhouse = ConfigConst.StorageConst.IsClickhouse, IsElasticsearch = ConfigConst.StorageConst.IsClickhouse };
}
