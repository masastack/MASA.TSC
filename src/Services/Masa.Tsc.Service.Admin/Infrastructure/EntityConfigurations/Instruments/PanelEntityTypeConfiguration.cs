﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Infrastructure.EntityConfigurations.Instruments;

public class PanelEntityTypeConfiguration : IEntityTypeConfiguration<Panel>
{
    public void Configure(EntityTypeBuilder<Panel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(255);
        builder.Property(x => x.UiType).HasMaxLength(40).IsRequired();
        builder.Property(x => x.Height).HasMaxLength(40);
        builder.Property(x => x.Width).HasMaxLength(40);
        builder.Property(x => x.Top).HasMaxLength(40);
        builder.Property(x => x.Left).HasMaxLength(40);

        builder.HasOne(x => x.Instrument).WithMany(p => p.Panels).HasForeignKey(x => new { x.InstrumentId });
        builder.Property(x => x.ExtensionData).HasMaxLength(500).HasConversion(
            v => JsonSerializer.Serialize(v, v.GetType(), (JsonSerializerOptions)null),
            v => JsonSerializer.Deserialize<Dictionary<ExtensionFieldTypes, object?>>(v, (JsonSerializerOptions)null));
    }
}
