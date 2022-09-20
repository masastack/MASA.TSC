// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Instruments;

public class AddInstrumentsDto
{
    public Guid UserId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Layer{ get; set; }

    [Required]
    public string Model { get; set; }

    public int Sort { get; set; }

    [Required]
    public Guid? DirectoryId { get; set; }

    public List<AddPanelDto> Panels { get; set; }
}