// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Commands;

public record AddDirectoryCommand(string Name, Guid ParentId, Guid UserId) : Command;