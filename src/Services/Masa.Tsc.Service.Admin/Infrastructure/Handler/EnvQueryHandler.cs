// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Infrastructure.Handler;

public abstract class EnvQueryHandler
{
    protected readonly IMasaStackConfig _masaStackConfig;
    protected readonly IWebHostEnvironment _environment;
    protected readonly IMultiEnvironmentContext _multiEnvironment;

    protected EnvQueryHandler(IMasaStackConfig masaStackConfig, IWebHostEnvironment environment, IMultiEnvironmentContext multiEnvironment)
    {
        _masaStackConfig = masaStackConfig;
        _environment = environment;
        _multiEnvironment = multiEnvironment;
    }

    protected string GetServiceEnvironmentName(string service) => _masaStackConfig.GetServiceEnvironmentName(_environment, service, _multiEnvironment.CurrentEnvironment);

    protected string GetServiceEnvironmentName(IEnumerable<string> services) => _masaStackConfig.GetServiceEnvironmentName(_environment, services, _multiEnvironment.CurrentEnvironment);
}
