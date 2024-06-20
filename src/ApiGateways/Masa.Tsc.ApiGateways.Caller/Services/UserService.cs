// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.StackSdks.Auth;
using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class UserService : BaseService
{
    private readonly IAuthClient _authClient;
    internal UserService(ICaller caller, IAuthClient authClient) : base(caller, "/api")
    {
        _authClient = authClient;
    }

    public async Task<UserModel> GetUserDetailAsync(Guid userId)
    {
        return await _authClient.UserService.GetByIdAsync(userId);
    }

    public async Task<List<UserRoleDto>> GetUserRolesAsync(Guid userId)
    {
        return await Caller.GetAsync<List<UserRoleDto>>($"{RootPath}/role/getSelectForUser?userId={userId}");
    }

    public async Task<Dictionary<string, string>> GetUserClaimAsync(Guid userId)
    {
        return await _authClient.UserService.GetClaimValuesAsync(userId);

        //return await Caller.GetAsync<Dictionary<string, object>>($"{RootPath}/user/claim-values/{userId}");
    }

    public async Task<List<UserClaimDto>> GetClaimsAsync()
    {
        return (await Caller.GetAsync<PaginatedListBase<UserClaimDto>>($"{RootPath}/sso/userClaim/getlist?page=1&pageSize=100")).Result;
    }
}
