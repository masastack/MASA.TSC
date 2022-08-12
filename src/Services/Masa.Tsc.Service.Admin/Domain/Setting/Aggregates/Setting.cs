// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Aggregates;

public class Setting : AggregateRoot<Guid>
{
    public Guid UserId { get; set; }

    public string Language { get; set; }

    public short Interval { get; set; }

    public bool IsEnable { get; set; }

    public short TimeZone { get; set; }

    public short TimeZoneOffset { get; set; }

    public void Update(string language, short interval, bool isEnable, short timeZone, short timeZoneOffset)
    {
        Language = language;
        Interval = interval;
        IsEnable = isEnable;
        TimeZone = timeZone;
        TimeZoneOffset = timeZoneOffset;
    }
}
