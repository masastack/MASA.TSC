// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class BaseService
{
    internal ICallerProvider Caller { get; private set; }

    internal string RootPath { get; private set; }

    public BaseService(ICallerProvider caller, string rootPath)
    {
        ArgumentNullException.ThrowIfNull(caller);
        Caller = caller;
        if (!string.IsNullOrEmpty(rootPath))
            RootPath = rootPath;
    }
}