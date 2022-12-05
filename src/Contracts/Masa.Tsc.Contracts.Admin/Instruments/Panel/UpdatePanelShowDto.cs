// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Instruments.Panel;

public class UpdatePanelShowDto
{
    public Guid Id { get; set; }

    public int Index { get; set; }

    public string Top { get; set; }

    public string Left { get; set; }

    public string Height { get; set; }

    public string Width { get; set; }
}
