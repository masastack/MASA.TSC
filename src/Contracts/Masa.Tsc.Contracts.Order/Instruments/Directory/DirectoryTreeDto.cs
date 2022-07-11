// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class DirectoryTreeDto : DirectoryDto
{
    public IEnumerable<DirectoryDto> Children { get; set; }
}