// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data.Apm;

public class SearchData
{
    //public string Text { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public string? Environment { get; set; }

    public string? Service { get; set; }

    public string? Endpoint { get; set; }

    public ApmComparisonTypes ComparisonType { get; set; }

    public string TextField { get; set; }

    public string TextValue { get; set; }

    public string Status { get; set; }

    public string Method { get; set; }

    public string ExceptionType { get; set; }

    public string ExceptionMsg { get; set; }

    public string TraceId { get; set; }

    public string SpanId { get; set; }
}

public enum ApmComparisonTypes
{
    None,
    Day = 1,
    Week = 2
}

public static class ApmComparisonTypeExtensions
{
    public static ComparisonTypes? ToComparisonType(this ApmComparisonTypes value)
    {
        if ((int)value - ComparisonTypes.DayBefore == 0 || (int)value - ComparisonTypes.WeekBefore == 0)
            return (ComparisonTypes)((int)value);
        return default;
    }
}