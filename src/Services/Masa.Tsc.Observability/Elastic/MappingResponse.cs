// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Observability.Elastic;

public class MappingResponse
{
    public string Name { get; set; }

    public string DataType { get; set; }

    public bool? IsKeyword { get; set; }

    /// <summary>
    /// keyword query max length
    /// </summary>
    public int? MaxLenth { get; set; }
}