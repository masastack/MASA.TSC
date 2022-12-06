// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Contracts.Admin.Dashboards;

namespace Masa.Tsc.Service.Admin.Application.Instruments.Commands;

public record UpdateInstrumentCommand(UpdateDashboardDto Data,Guid UserId):Command;