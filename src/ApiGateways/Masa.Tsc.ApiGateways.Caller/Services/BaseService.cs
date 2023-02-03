// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class BaseService
{
    internal ICaller Caller { get; private set; }

    internal string RootPath { get; private set; }

    internal TokenProvider TokenProvider { get; private set; }

    public BaseService(ICaller caller, string rootPath, TokenProvider tokenProvider)
    {
        ArgumentNullException.ThrowIfNull(caller);
        Caller = caller;
        TokenProvider = tokenProvider;
        if (!string.IsNullOrEmpty(rootPath))
            RootPath = rootPath;
        Caller.ConfigRequestMessage(async message =>
        {
            if (TokenProvider != null && !string.IsNullOrEmpty(TokenProvider.AccessToken))
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenProvider.AccessToken);

            await Task.CompletedTask;
        });
    }
}