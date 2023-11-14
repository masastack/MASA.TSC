// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Topologies;

public class QueryHandler
{
    [EventHandler]
    public void GetTopology(TopologyQuery query)
    {
        var result = new TopologyResultDto();
        query.Result = result;
    }
}