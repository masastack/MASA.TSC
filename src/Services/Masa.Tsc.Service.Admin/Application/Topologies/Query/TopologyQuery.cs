// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Contracts.Admin.Topologies;

namespace Masa.Tsc.Service.Admin.Application.Topologies.Query;

public record TopologyQuery(TopologyRequestDto Data) : Query<TopologyResultDto>
{
    public override TopologyResultDto Result { get; set; }
}
