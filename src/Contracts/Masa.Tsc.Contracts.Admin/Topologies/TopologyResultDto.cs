// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Topologies;

public class TopologyResultDto
{
    public List<TopologyServiceDto> Services { get; set; }

    public List<TopologyServiceRelationDto> Relations { get; set; }

    public List<TopologyServiceDataDto> Data { get; set; }
}

public class TopologyServiceDto
{ 
    public string Id { get; set; }

    public string Name { get; set; }

    public TraceNodeTypes Type { get; set; }
}

public class TopologyServiceRelationDto
{ 
    public string CurrentId { get; set; }

    public string DestId { get; set; }
}

public class TopologyServiceDataDto: TopologyServiceRelationDto
{ 
    public int CallsCount { get; set; }

    public int AvgLatency { get; set; }

    public int CallErrorCount { get; set; }
}
