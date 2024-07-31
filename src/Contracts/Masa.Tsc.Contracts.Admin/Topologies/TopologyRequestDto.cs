// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Topologies;

public class TopologyRequestDto : FromUri<TopologyRequestDto>
{
    public string ServiceName { get; set; }

    public int Level { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }
}
