// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Instruments.Commands;

public record RemoveInstrumentCommand(Guid UserId, Guid[] InstrumentIds) : Command;