// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class MappingResponse
{
    public string Name { get; set; }

    public string DataType { get; set; }

    public bool? IsKeyword { get; set; }

    /// <summary>
    /// keyword 支持最长查询
    /// </summary>
    public int? MaxLenth { get; set; }
}