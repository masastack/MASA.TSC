// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class BaseService
{
    internal ICaller Caller { get; private set; }

    internal string RootPath { get; private set; }

    public BaseService(ICaller caller, string rootPath)
    {
        ArgumentNullException.ThrowIfNull(caller);
        Caller = caller;
        if (!string.IsNullOrEmpty(rootPath))
            RootPath = rootPath;
    }
}