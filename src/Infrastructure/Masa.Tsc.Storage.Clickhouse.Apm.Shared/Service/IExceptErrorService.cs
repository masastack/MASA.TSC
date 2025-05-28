// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Clickhouse.Apm.Shared.Service;

public interface IExceptErrorService
{
    Task AddAsync(params ExceptErrorDto[] values);
}
