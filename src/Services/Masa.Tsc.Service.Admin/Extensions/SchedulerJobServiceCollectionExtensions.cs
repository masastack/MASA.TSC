// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Contracts.Admin.Infrastructure.Const;

namespace Microsoft.Extensions.DependencyInjection;

public static class SchedulerJobServiceCollectionExtensions
{
    private static IMasaStackConfig _masaStackConfig;

    public static async Task AddSchedulerJobAsync(this IServiceCollection services)
    {
        if (_masaStackConfig != null)
            return;
        _masaStackConfig = services.GetMasaStackConfig();
        using IServiceScope scope = services.BuildServiceProvider().CreateScope();
        var serviceProvider = scope.ServiceProvider;
        await serviceProvider.SafeExcuteAsync(AddTopologyAsync);
    }

    public static async Task AddTopologyAsync(IServiceProvider serviceProvider)
    {
        var url = _masaStackConfig.GetTscServiceDomain();
        var schedulerClient = serviceProvider.GetRequiredService<ISchedulerClient>();
        await schedulerClient.SchedulerJobService.AddAsync(new AddSchedulerJobRequest()
        {
            ProjectIdentity = MasaStackConstant.TSC,
            JobIdentity = "masa-tsc-topology",
            Name = "CaculateTopologyRelation",
            IsAlertException = false,
            JobType = JobTypes.Http,
            CronExpression = "0 0/10 * * * ?",
            RunTimeoutSecond = 60 * 10,
            HttpConfig = new SchedulerJobHttpConfig()
            {
                HttpMethod = HttpMethods.GET,
                RequestUrl = Path.Combine(url, "api/topology/start")
            }
        });
    }

    async static Task SafeExcuteAsync(this IServiceProvider serviceProvider, Func<IServiceProvider, Task> job, [CallerArgumentExpression("job")] string? jobName = null)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        try
        {
            await job(serviceProvider);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"sync scheduler {jobName} error");
        }
    }
}
