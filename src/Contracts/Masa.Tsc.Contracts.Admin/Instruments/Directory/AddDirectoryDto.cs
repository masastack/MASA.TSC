﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class AddDirectoryDto
{
    public string Name { get; set; }

    public Guid ParentId { get; set; }

    public int Sort { get; set; }

    public Guid UserId { get; set; }
}