// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.EFCore.PostgreSQL;

public class TscDbPgContextFactory : IDesignTimeDbContextFactory<TscDbContext>
{
    public const string ConnectionStringKey = "MasaTscMssqlStaging";

    public TscDbContext CreateDbContext(string[] args)
    {
        TscDbContext.RegistAssembly(typeof(TscDbPgContextFactory).Assembly);

        var configuration = new ConfigurationBuilder()
            .AddUserSecrets(typeof(TscDbPgContextFactory).Assembly, optional: true)
            .Build();

        var connectionString = configuration[ConnectionStringKey];
        var optionsBuilder = new MasaDbContextOptionsBuilder<TscDbContext>();
        optionsBuilder.DbContextOptionsBuilder.UseNpgsql(
            connectionString,
            b => b.MigrationsAssembly("Masa.Tsc.EFCore.PostgreSQL"));

        return new TscDbContext(optionsBuilder.MasaOptions);
    }
}
