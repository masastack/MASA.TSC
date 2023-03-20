// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class InitData
{
    public static async Task MigrateAsync(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var context = serviceProvider.GetRequiredService<TscDbContext>();

        if (context.Database.GetPendingMigrations().Any())
        {
            await context.Database.MigrateAsync();
        }
        await SeedAsync(context, serviceProvider);

    }

    public static async Task SeedAsync(TscDbContext context, IServiceProvider serviceProvider)
    {
        var serviceDirectoryId = Guid.Parse("21e43ab2-a853-41df-7baf-08dae7dc60af");
        if (await context.Set<Directory>().AnyAsync(item => item.Id == serviceDirectoryId))
            return;

        try
        {
            var path = "init-db.txt";
            if (!File.Exists("init-db.txt"))
            {
                Console.WriteLine("init-db.txt not exists, init db failed");
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

