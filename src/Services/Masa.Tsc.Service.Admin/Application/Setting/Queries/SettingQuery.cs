// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Setting;

public record SettingQuery(Guid UserId) : Query<SettingDto>
{
    public override SettingDto Result { get; set; }
}
