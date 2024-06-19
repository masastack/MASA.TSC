// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class UserService : BaseService
{
    internal UserService(ICaller caller) : base(caller, "/api") { }

    public async Task<List<UserRoleDto>> GetUserRolesAsync(Guid userId)
    {
        return await Caller.GetAsync<List<UserRoleDto>>($"{RootPath}/role/getSelectForUser?userId={userId}");
    }

    public async Task<Dictionary<string, object>> GetUserClaimAsync(Guid userId)
    {
        return await Caller.GetAsync<Dictionary<string, object>>($"{RootPath}/user/claim-values/{userId}");
    }

    public async Task<List<UserClaimDto>> GetClaimsAsync()
    {
        return (await Caller.GetAsync<PaginatedListBase<UserClaimDto>>($"{RootPath}/sso/userClaim/getlist?page=1&pageSize=100")).Result;
    }
}
