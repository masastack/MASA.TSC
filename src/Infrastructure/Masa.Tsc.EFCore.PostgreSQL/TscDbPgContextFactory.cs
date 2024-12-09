// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.EFCore.PostgreSQL;

public class TscDbPgContextFactory : IDesignTimeDbContextFactory<TscDbContext>
{
    public TscDbContext CreateDbContext(string[] args)
    {
        TscDbContext.RegistAssembly(typeof(TscDbPgContextFactory).Assembly);
        var optionsBuilder = new MasaDbContextOptionsBuilder<TscDbContext>();
        var configurationBuilder = new ConfigurationBuilder();
        var configuration = configurationBuilder
            .AddJsonFile("migrate-pgsql.json")
            .Build();
       
        optionsBuilder.DbContextOptionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection")!, b => b.MigrationsAssembly("Masa.Tsc.EFCore.PostgreSQL"));

        return new TscDbContext(optionsBuilder.MasaOptions);
    }
}