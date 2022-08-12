// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class SettingDto
{
    public string Language { get; set; }

    public bool IsEnable { get; set; }

    public short TimeZone { get; set; }

    public short TimeZoneOffset { get; set; }

    public short Interval { get; set; }

    public Guid UserId { get; set; }
}
