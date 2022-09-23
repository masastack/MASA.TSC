// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class BaseService
{
    internal ICaller Caller { get; private set; }

    internal string RootPath { get; private set; }

    private TokenProvider _tokenProvider { get; set; }

    public BaseService(ICaller caller, string rootPath, TokenProvider tokenProvider)
    {
        ArgumentNullException.ThrowIfNull(caller);
        Caller = caller;
        _tokenProvider = tokenProvider;
        if (!string.IsNullOrEmpty(rootPath))
            RootPath = rootPath;
        Caller.ConfigRequestMessage(message =>
        {
            if (_tokenProvider != null && !string.IsNullOrEmpty(_tokenProvider.AccessToken))
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);
        });
    }
}