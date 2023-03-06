// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.StackSdks.Middleware;

namespace Masa.Tsc.Service.Admin.Infrastructure.Middleware;

public class TscDisabledEventDeterminer : IDisabledEventDeterminer
{
    private readonly IUserContext _userContext;
    private readonly IMasaStackConfig _masaStackConfig;

    public TscDisabledEventDeterminer(IUserContext userContext,
    IMasaStackConfig masaStackConfig)
    {
        _userContext = userContext;
        _masaStackConfig = masaStackConfig;
    }

    public bool DisabledCommand => throw new NotImplementedException();

    public bool Determiner()
    {
        var user = _userContext.GetUser<MasaUser>();
        return _masaStackConfig.IsDemo && string.Equals(user?.Account, "guest", StringComparison.OrdinalIgnoreCase);
    }
}
