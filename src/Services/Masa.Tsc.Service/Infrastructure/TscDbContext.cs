// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.
using Masa.Contrib.Data.EntityFrameworkCore;

namespace Masa.Tsc.Service.Infrastructure;

public class TscDbContext : MasaDbContext
{
    //public DbSet<Instrument> Instruments { get; set; } = default!;

    public TscDbContext(MasaDbContextOptions<TscDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreatingExecuting(ModelBuilder builder)
    {
        base.OnModelCreatingExecuting(builder);
    }
}
