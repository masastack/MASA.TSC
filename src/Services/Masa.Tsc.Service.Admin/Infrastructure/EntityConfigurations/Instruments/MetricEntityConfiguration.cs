// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Infrastructure.EntityConfigurations;

public class MetricEntityConfiguration : IEntityTypeConfiguration<PanelMetric>
{
    public void Configure(EntityTypeBuilder<PanelMetric> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Caculate).HasMaxLength(200).IsRequired(false);
        builder.Property(x => x.Unit).HasMaxLength(20).IsRequired(false);
        builder.Property(x => x.Color).HasMaxLength(20).IsRequired(false);
        builder.Property(x => x.Icon).HasMaxLength(20).IsRequired(false);
        builder.Property(x => x.DisplayName).HasMaxLength(200).IsRequired(false);
        builder.Ignore(x => x.Panel);
    }
}
