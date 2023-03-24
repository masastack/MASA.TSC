// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore;

namespace Masa.Tsc.Service.Infrastructure;

public class TscDbContext : MasaDbContext
{
    public TscDbContext(MasaDbContextOptions<TscDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreatingExecuting(ModelBuilder builder)
    {
        builder.HasDefaultSchema("tsc");
        builder.ApplyConfiguration(new IntegrationEventLogEntityTypeConfiguration());
        builder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(TscDbContext))!);
        base.OnModelCreatingExecuting(builder);
    }
}