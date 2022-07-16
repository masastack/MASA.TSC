// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class RequestAggDto
{
    public IEnumerable<RequestFieldAggDto> FieldMaps { get; set; }

    /// <summary>
    /// third original query conditions
    /// </summary>
    public string RawQuery { get; set; }

    /// <summary>
    /// key-value exact query like {"name","David"} query name="David" not support fuzzy query
    /// </summary>
    public IEnumerable<KeyValuePair<string, object>> Queries { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }
}