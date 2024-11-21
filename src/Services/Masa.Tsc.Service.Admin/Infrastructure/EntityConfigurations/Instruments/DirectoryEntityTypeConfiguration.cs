﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Infrastructure.EntityConfigurations;

internal class DirectoryEntityTypeConfiguration : IEntityTypeConfiguration<Domain.Aggregates.Directory>
{
    public void Configure(EntityTypeBuilder<Domain.Aggregates.Directory> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.UserId);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.HasIndex(x => x.Name).IsUnique();
    }
}