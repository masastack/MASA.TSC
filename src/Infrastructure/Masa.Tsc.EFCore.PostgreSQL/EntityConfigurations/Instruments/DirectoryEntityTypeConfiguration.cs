// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.EFCore.PostgreSQL.EntityConfigurations;

internal class DirectoryEntityTypeConfiguration : IEntityTypeConfiguration<Masa.Tsc.Domain.Instruments.Aggregates.Directory>
{
    public void Configure(EntityTypeBuilder<Masa.Tsc.Domain.Instruments.Aggregates.Directory> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.UserId);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.HasIndex(x => x.Name).IsUnique();
    }
}