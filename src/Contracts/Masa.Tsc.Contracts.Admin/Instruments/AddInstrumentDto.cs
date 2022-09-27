// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Instruments;

public class AddInstrumentDto
{
    public Guid Id { get; set; }=Guid.NewGuid();    

    [Required]
    public string Name { get; set; }

    [Required]
    public string Layer{ get; set; }

    [Required]
    public string Model { get; set; }

    public int Sort { get; set; }

    public bool IsGlobal { get; set; }

    public bool IsRoot { get; set; }    

    [Required]
    public Guid DirectoryId { get; set; }
}