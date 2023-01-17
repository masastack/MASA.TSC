// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Topologies.Commands;

/// <summary>
/// start Topology create
/// </summary>
public record StartCommand(DateTime Start, DateTime End) : Command { }
