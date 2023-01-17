// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class SeedData
{
    public static async Task MigrateAsync(this IServiceCollection services)
    {
        var context = services.BuildServiceProvider().GetRequiredService<TscDbContext>();

        if (context.Database.GetPendingMigrations().Any())
        {
            await context.Database.MigrateAsync();
        }
    }

    public static async Task SeedAsync(TscDbContext context, IServiceProvider serviceProvider)
    {
        // todo
        await Task.CompletedTask;
    }
}
