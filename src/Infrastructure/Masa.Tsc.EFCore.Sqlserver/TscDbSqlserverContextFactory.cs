﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.EFCore.Sqlserver;

public class TscDbSqlserverContextFactory : IDesignTimeDbContextFactory<TscDbContext>
{
    public TscDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new MasaDbContextOptionsBuilder<TscDbContext>();
        var configurationBuilder = new ConfigurationBuilder();
        var configuration = configurationBuilder
            .AddJsonFile("migrate-sqlserver.json")
            .Build();

        optionsBuilder.DbContextOptionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection")!, b => b.MigrationsAssembly("Masa.Tsc.EFCore.Sqlserver"));

        return new TscDbContext(optionsBuilder.MasaOptions);
    }
}