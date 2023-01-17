﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Instruments.Commands;

public record UpInsertCommand(UpsertPanelDto[] Data, Guid InstumentId, Guid UserId) : Command;