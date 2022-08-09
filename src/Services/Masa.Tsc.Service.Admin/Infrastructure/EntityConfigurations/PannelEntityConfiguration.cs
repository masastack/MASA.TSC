// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Infrastructure.EntityConfigurations;

public class PanelEntityConfiguration
{
    public void Configure(EntityTypeBuilder<Panel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(255);
        builder.Property(x => x.UiType).HasMaxLength(40).IsRequired();
        builder.Property(x => x.XDisplayName).HasMaxLength(40);
        builder.Property(x => x.XName).HasMaxLength(40).IsRequired();

        builder.HasOne(x => x.Instrument).WithMany(p => p.Panels).HasForeignKey(x => new { x.InstrumentId });
    }
}
