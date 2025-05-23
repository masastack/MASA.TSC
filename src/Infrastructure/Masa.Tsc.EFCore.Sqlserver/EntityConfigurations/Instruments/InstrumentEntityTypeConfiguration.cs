﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.EFCore.Sqlserver.EntityConfigurations.Instruments;

internal class InstrumentEntityTypeConfiguration : IEntityTypeConfiguration<Instrument>
{
    public void Configure(EntityTypeBuilder<Instrument> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Sort).HasDefaultValue(0);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Layer).HasMaxLength(40);
        builder.Property(x => x.Model).HasMaxLength(40);
        builder.Property(x => x.Lable).HasMaxLength(40);
        builder.Property(x => x.EnableEdit).HasDefaultValue(false);
        builder.HasOne(x => x.Directory).WithMany(x => x.Instruments).HasForeignKey(x => x.DirectoryId);
        builder.Ignore(x => x.Panels);
    }
}
