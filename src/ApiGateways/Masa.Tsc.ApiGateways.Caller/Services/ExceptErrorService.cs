// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class ExceptErrorService : BaseService
{
    internal ExceptErrorService(ICaller caller) : base(caller, "/api/except-error") { }

    public Task AddAsync(RequestAddExceptError model) => Caller.PostAsync(RootPath, model);

    public Task RemoveAsync(string id) => Caller.DeleteAsync($"{RootPath}/{id}", default);

    public Task<PaginatedListBase<ExceptErrorDto>> ListAsync(RequestExceptErrorQuery query) => Caller.GetAsync<PaginatedListBase<ExceptErrorDto>>(RootPath, query)!;
}
