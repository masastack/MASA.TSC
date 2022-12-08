// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Instruments.Queries;

public record PanelQuery(Guid InstrumentId,Guid UserId) : Query<List<PanelDto>>
{
    public override List<PanelDto> Result { get; set; }
}