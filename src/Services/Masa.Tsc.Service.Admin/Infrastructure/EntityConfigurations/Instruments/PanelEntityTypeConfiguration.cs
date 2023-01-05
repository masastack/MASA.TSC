// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Infrastructure.EntityConfigurations.Instruments;

public class PanelEntityTypeConfiguration : IEntityTypeConfiguration<Panel>
{
    public void Configure(EntityTypeBuilder<Panel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(255);
        builder.Property(x => x.Height).HasMaxLength(40);
        builder.Property(x => x.Width).HasMaxLength(40);
        builder.Property(x => x.Top).HasMaxLength(40);
        builder.Property(x => x.Left).HasMaxLength(40);

        builder.Ignore(x => x.Panels);
        builder.Ignore(x => x.Instrument);
        builder.Ignore(x => x.Metrics);
        builder.Property(x => x.ExtensionData).HasMaxLength(1000).HasConversion(
            v => JsonSerializer.Serialize(v, v.GetType(), default(JsonSerializerOptions)!) ?? string.Empty,
            v => JsonSerializer.Deserialize<Dictionary<ExtensionFieldTypes, object?>>(v, default(JsonSerializerOptions)!)!).IsRequired(false);
    }
}