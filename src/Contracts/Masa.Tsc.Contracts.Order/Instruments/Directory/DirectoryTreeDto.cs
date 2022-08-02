// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class DirectoryTreeDto : DirectoryDto
{
    public DirectoryTypes DirectoryType { get; set; } = DirectoryTypes.Directory;

    public bool Expand { get; set; }

    public bool Selected { get; set; }

    public IEnumerable<DirectoryTreeDto> Children { get; set; }
}