// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Domain.Shared.Entities;

namespace Masa.Tsc.EFCore.Sqlserver.EntityConfigurations;

internal class ExceptErrorEntityTypeConfiguration : IEntityTypeConfiguration<ExceptError>
{
    public void Configure(EntityTypeBuilder<ExceptError> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.Environment, x.Project, x.Service, x.Type });
        builder.Property(x => x.Id).HasMaxLength(40);
        builder.Property(x => x.Environment).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Project).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Service).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Type).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Message).HasMaxLength(500);
        builder.Property(x=>x.Comment).HasMaxLength(500);
    }
}
