// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Infrastructure.Handler;

public abstract class TraceStatusQueryHandler : EnvQueryHandler
{
    protected readonly IMasaConfiguration _masaConfiguration;

    protected TraceStatusQueryHandler(IMasaConfiguration masaConfiguration, IMasaStackConfig masaStackConfig, IWebHostEnvironment environment, IMultiEnvironmentContext multiEnvironment) 
        : base(masaStackConfig, environment, multiEnvironment)
    {
        _masaConfiguration = masaConfiguration;
    }

    public int[] GetTraceErrorStatus() => _masaConfiguration.GetTraceErrorStatus(_masaStackConfig);
}
