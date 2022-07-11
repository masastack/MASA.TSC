// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Service.Admin.Application.Instruments.Queries;

namespace Masa.Tsc.Service.Admin.Application.Instruments;

public class DirectoryQueryHandler
{

    [EventHandler]
    public async Task GetAsync(DirectoryQuery query)
    { }

    [EventHandler]
    public async Task GetTreeAsync(DirectoryTreeQuery query)
    { }
}
