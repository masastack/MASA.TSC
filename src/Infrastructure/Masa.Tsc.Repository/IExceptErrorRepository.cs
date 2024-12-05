// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Repository;

public interface IExceptErrorRepository : IRepository<ExceptError>
{
    public IEnumerable<ExceptError> ExceptErrors { get; }
}