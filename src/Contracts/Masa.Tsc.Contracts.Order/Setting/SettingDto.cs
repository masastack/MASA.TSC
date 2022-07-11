// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class SettingDto
{
    public string Langauge { get; set; }

    public bool IsEnable { get; set; }

    public byte TimeZone { get; set; }

    public int TimeZoneOffset { get; set; }

    public int Interval { get; set; }

    public Guid UserId { get; set; }
}
