// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Domain.Shared.Entities;

namespace Masa.Tsc.Repository;

public interface IExceptErrorRepository : IRepository<ExceptError>
{
    public IEnumerable<ExceptError> ExceptErrors { get; }
}