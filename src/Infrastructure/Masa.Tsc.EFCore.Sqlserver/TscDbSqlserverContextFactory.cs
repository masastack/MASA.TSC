// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.EFCore.Sqlserver;

public class TscDbSqlserverContextFactory : IDesignTimeDbContextFactory<TscDbContext>
{
    public const string ConnectionStringKey = "MasaTscMssqlStaging";

    public TscDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
             .AddUserSecrets(typeof(TscDbSqlserverContextFactory).Assembly, optional: true)
             .Build();

        var connectionString = configuration[ConnectionStringKey];
        var optionsBuilder = new MasaDbContextOptionsBuilder<TscDbContext>();
        optionsBuilder.DbContextOptionsBuilder.UseSqlServer(connectionString, b => b.MigrationsAssembly("Masa.Tsc.EFCore.Sqlserver"));

        return new TscDbContext(optionsBuilder.MasaOptions);
    }
}