// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.EFCore;

public partial class TscDbContext : MasaDbContext<TscDbContext>
{
    internal static Assembly Assembly = typeof(TscDbContext).Assembly;

    public static void RegistAssembly(Assembly assembly)
    {
        Assembly = assembly;
    }

    public TscDbContext(MasaDbContextOptions<TscDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreatingExecuting(ModelBuilder builder)
    {
        builder.HasDefaultSchema("tsc");
        builder.ApplyConfigurationsFromAssembly(Assembly);
        base.OnModelCreatingExecuting(builder);
    }
}