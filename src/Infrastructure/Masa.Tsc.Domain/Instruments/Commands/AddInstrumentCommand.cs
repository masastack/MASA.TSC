﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Commands;

public record AddInstrumentCommand(AddDashboardDto Data, Guid UserId) : Command;