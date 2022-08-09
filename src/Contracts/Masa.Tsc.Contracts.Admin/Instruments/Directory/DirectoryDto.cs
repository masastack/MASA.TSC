// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class DirectoryDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public Guid ParentId { get; set; }

    public int Sort { get; set; }
}
