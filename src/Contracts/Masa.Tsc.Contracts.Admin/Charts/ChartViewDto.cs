// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Charts;

public class ChartViewDto
{
    public string Title { get; set; }

    public string ChartType { get; set; }
    
    /// <summary>
    /// x value raw toString, like '8' 、'8.00'、'8.00%'
    /// </summary>
    public string[] Points { get; set; }
}
