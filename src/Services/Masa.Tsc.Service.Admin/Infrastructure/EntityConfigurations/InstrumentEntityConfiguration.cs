// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Infrastructure.EntityConfigurations;

public class InstrumentEntityConfiguration
{
    public void Configure(EntityTypeBuilder<Instrument> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Sort).HasDefaultValue(0);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Layer).HasMaxLength(40);
        builder.Property(x => x.Entity).HasMaxLength(40);
        //builder.HasMany(x => x.Pannels).WithOne();
        //builder.HasOne(x=>x.Id).WithMany()
        //builder.Property(x=>x.Pannels).m
    }
}
