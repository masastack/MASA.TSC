// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Setting.Aggregates;

public class Setting : AggregateRoot<Guid>
{
    public Guid UserId { get; set; }

    public string Language { get; set; }

    public int Interval { get; set; }

    public bool IsEnable { get; set; }

    public byte TimeZone { get; set; }

    public int TimeZoneOffset { get; set; }

    public void Update(string language, int interval, bool isEnable, byte timeZone, int timeZoneOffset)
    {
        Language = language;
        Interval = interval;
        IsEnable = isEnable;
        TimeZone = timeZone;
        TimeZoneOffset = timeZoneOffset;
    }
}
