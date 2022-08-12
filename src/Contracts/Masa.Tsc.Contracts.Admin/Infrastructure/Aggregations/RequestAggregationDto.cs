// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class RequestAggregationDto
{
    public IEnumerable<RequestFieldAggregationDto> FieldMaps { get; set; }

    /// <summary>
    /// third original query conditions
    /// </summary>
    public string RawQuery { get; set; }

    /// <summary>
    /// key-value exact query like {"name","David"} query name="David" not support fuzzy query
    /// </summary>
    public Dictionary<string, string> Queries { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public string Interval { get; set; }

    /// <summary>
    /// true
    /// </summary>
    public bool IsDesc { get; set; } = true;

    /// <summary>
    /// sort key in aggregation keys
    /// </summary>
    public string OrderField { get; set; }
}