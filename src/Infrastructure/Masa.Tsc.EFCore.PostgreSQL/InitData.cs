// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class InitData
{
    public static async Task MigratePgAsync(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var context = serviceProvider.GetRequiredService<TscDbContext>();

        if (context.Database.GetPendingMigrations().Any())
        {
            await context.Database.MigrateAsync();
        }
        await SeedAsync(context, serviceProvider);

    }

    private static async Task SeedAsync(TscDbContext context, IServiceProvider serviceProvider)
    {
        var serviceDirectoryId = Guid.Parse("21e43ab2-a853-41df-7baf-08dae7dc60af");
        if (await context.Set<Masa.Tsc.Domain.Instruments.Aggregates.Directory>().AnyAsync(item => item.Id == serviceDirectoryId))
            return;

        try
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var path = $"{basePath}init-pg-db.txt";

            if (!File.Exists(path))
            {
                Console.WriteLine($"{path} not exists, init db failed");
                return;
            }
            using var file = File.OpenText(path);
            var sql = file.ReadToEnd();
            if (string.IsNullOrEmpty(sql))
            {
                Console.WriteLine("init sql is empty");
                return;
            }

            _ = await context.Database.ExecuteSqlRawAsync(sql);
            Console.WriteLine("db init success");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"db init fail: message:{ex.Message},stacktrace:{ex.StackTrace}");
        }
    }
}

