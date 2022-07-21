// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Chart;

public class ChartLineDataDto<T> where T : ChartPointDto
{
    public IEnumerable<T> Data { get; set; }
}