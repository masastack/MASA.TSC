// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.ExceptError;

public record CreateExceptErrorCommand(RequestAddExceptError Data,string User) : Command;

public record UpdateExceptErrorCommand(string Id, string Comment,string User) : Command;

public record DeleteExceptErrorCommand(string Id) : Command;

public record DeletesExceptErrorCommand(string[] Ids) : Command;