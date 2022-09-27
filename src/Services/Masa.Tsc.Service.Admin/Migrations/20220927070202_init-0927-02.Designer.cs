﻿// <auto-generated />
using System;
using Masa.Tsc.Service.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Masa.Tsc.Service.Admin.Migrations
{
    [DbContext(typeof(TscDbContext))]
    [Migration("20220927070202_init-0927-02")]
    partial class init092702
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("tsc")
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Masa.BuildingBlocks.Dispatcher.IntegrationEvents.Logs.IntegrationEventLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("EventId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("EventTypeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("RowVersion")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)")
                        .HasColumnName("RowVersion");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<int>("TimesSent")
                        .HasColumnType("int");

                    b.Property<Guid>("TransactionId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "EventId", "RowVersion" }, "index_eventid_version");

                    b.HasIndex(new[] { "State", "ModificationTime" }, "index_state_modificationtime");

                    b.HasIndex(new[] { "State", "TimesSent", "ModificationTime" }, "index_state_timessent_modificationtime");

                    b.ToTable("IntegrationEventLog", "tsc");
                });

            modelBuilder.Entity("Masa.Tsc.Service.Admin.Domain.Aggregates.Directory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<Guid>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Sort")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Directory", "tsc");
                });

            modelBuilder.Entity("Masa.Tsc.Service.Admin.Domain.Aggregates.Instrument", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DirectoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsGlobal")
                        .HasColumnType("bit");

                    b.Property<bool>("IsRoot")
                        .HasColumnType("bit");

                    b.Property<string>("Layer")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("Sort")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.HasKey("Id");

                    b.HasIndex("DirectoryId");

                    b.ToTable("Instrument", "tsc");
                });

            modelBuilder.Entity("Masa.Tsc.Service.Admin.Domain.Aggregates.Panel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ChartType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Height")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("InstrumentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Sort")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<string>("UiType")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<string>("Width")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("InstrumentId");

                    b.ToTable("Panel", "tsc");
                });

            modelBuilder.Entity("Masa.Tsc.Service.Admin.Domain.Aggregates.PanelMetric", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<Guid>("PanelId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Sort")
                        .HasColumnType("int");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasDefaultValue("");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.HasIndex("PanelId");

                    b.ToTable("PanelMetric", "tsc");
                });

            modelBuilder.Entity("Masa.Tsc.Service.Admin.Domain.Aggregates.Setting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<short>("Interval")
                        .HasColumnType("smallint");

                    b.Property<bool>("IsEnable")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<short>("TimeZone")
                        .HasColumnType("smallint");

                    b.Property<short>("TimeZoneOffset")
                        .HasColumnType("smallint");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Setting", "tsc");
                });

            modelBuilder.Entity("Masa.Tsc.Service.Admin.Domain.Aggregates.Instrument", b =>
                {
                    b.HasOne("Masa.Tsc.Service.Admin.Domain.Aggregates.Directory", "Directory")
                        .WithMany("Instruments")
                        .HasForeignKey("DirectoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Directory");
                });

            modelBuilder.Entity("Masa.Tsc.Service.Admin.Domain.Aggregates.Panel", b =>
                {
                    b.HasOne("Masa.Tsc.Service.Admin.Domain.Aggregates.Instrument", "Instrument")
                        .WithMany("Panels")
                        .HasForeignKey("InstrumentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Instrument");
                });

            modelBuilder.Entity("Masa.Tsc.Service.Admin.Domain.Aggregates.PanelMetric", b =>
                {
                    b.HasOne("Masa.Tsc.Service.Admin.Domain.Aggregates.Panel", "Panel")
                        .WithMany("Metrics")
                        .HasForeignKey("PanelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Panel");
                });

            modelBuilder.Entity("Masa.Tsc.Service.Admin.Domain.Aggregates.Directory", b =>
                {
                    b.Navigation("Instruments");
                });

            modelBuilder.Entity("Masa.Tsc.Service.Admin.Domain.Aggregates.Instrument", b =>
                {
                    b.Navigation("Panels");
                });

            modelBuilder.Entity("Masa.Tsc.Service.Admin.Domain.Aggregates.Panel", b =>
                {
                    b.Navigation("Metrics");
                });
#pragma warning restore 612, 618
        }
    }
}
