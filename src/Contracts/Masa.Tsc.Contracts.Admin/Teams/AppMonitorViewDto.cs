// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class AppMonitorViewDto
{
    public int ServiceTotal { get; set; }

    public long AppTotal { get; set; }

    public string Color { get; set; }

    public string Text { get; set; }

    public string Value { get; set; }

    public string Icon { get; set; }

    public bool IsShowApp { get; set; } = true;

    public string ToDispaly()
    {
        if (IsShowApp)
            return $"{ServiceTotal} ({AppTotal})";
        else
            return ServiceTotal.ToString();
    }
}
