// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.EFCore.Sqlserver.EntityConfigurations;

internal class DirectoryEntityTypeConfiguration : IEntityTypeConfiguration<Domain.Shared.Entities.Directory>
{
    public void Configure(EntityTypeBuilder<Domain.Shared.Entities.Directory> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.UserId);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.HasIndex(x => x.Name).IsUnique();
    }
}