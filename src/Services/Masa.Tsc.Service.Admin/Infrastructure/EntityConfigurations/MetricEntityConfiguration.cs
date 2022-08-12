// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Infrastructure.EntityConfigurations;

public class MetricEntityConfiguration
{
    public void Configure(EntityTypeBuilder<Domain.Aggregates.Metric> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.DisplayName).HasMaxLength(200);
        builder.Property(x => x.Unit).HasMaxLength(20).HasDefaultValue(string.Empty);
        builder.Property(x => x.Value).HasMaxLength(200).IsRequired();

        builder.HasOne(x => x.Panel).WithMany(p => p.Metrics).HasForeignKey(x => new { x.PanelId });
    }
}
