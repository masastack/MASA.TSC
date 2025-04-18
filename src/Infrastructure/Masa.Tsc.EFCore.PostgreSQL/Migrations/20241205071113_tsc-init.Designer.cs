﻿// <auto-generated />
using System;
using Masa.Tsc.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Masa.Tsc.EFCore.PostgreSQL.Migrations
{
    [DbContext(typeof(TscDbContext))]
    [Migration("20241205071113_tsc-init")]
    partial class tscinit
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("tsc")
                .HasAnnotation("ProductVersion", "6.0.33")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Masa.Tsc.Domain.ExceptError", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uuid");

                    b.Property<string>("Environment")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uuid");

                    b.Property<string>("Project")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Service")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Id");

                    b.HasIndex("Environment", "Project", "Service", "Type");

                    b.ToTable("ExceptError", "tsc");
                });

            modelBuilder.Entity("Masa.Tsc.Domain.Instruments.Aggregates.Directory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<Guid>("ParentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("Directory", "tsc");
                });

            modelBuilder.Entity("Masa.Tsc.Domain.Instruments.Aggregates.Instrument", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uuid");

                    b.Property<Guid>("DirectoryId")
                        .HasColumnType("uuid");

                    b.Property<bool>("EnableEdit")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsGlobal")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsRoot")
                        .HasColumnType("boolean");

                    b.Property<string>("Lable")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)");

                    b.Property<string>("Layer")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)");

                    b.Property<DateTime>("ModificationTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("Modifier")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<int>("Sort")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.HasKey("Id");

                    b.HasIndex("DirectoryId");

                    b.ToTable("Instrument", "tsc");
                });

            modelBuilder.Entity("Masa.Tsc.Domain.Instruments.Aggregates.Panel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("ExtensionData")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<string>("Height")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)");

                    b.Property<int>("Index")
                        .HasColumnType("integer");

                    b.Property<Guid>("InstrumentId")
                        .HasColumnType("uuid");

                    b.Property<string>("Left")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)");

                    b.Property<Guid>("ParentId")
                        .HasColumnType("uuid");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Top")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<string>("Width")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)");

                    b.HasKey("Id");

                    b.ToTable("Panel", "tsc");
                });

            modelBuilder.Entity("Masa.Tsc.Domain.Instruments.Aggregates.PanelMetric", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Caculate")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Color")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Icon")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<Guid>("PanelId")
                        .HasColumnType("uuid");

                    b.Property<int>("Sort")
                        .HasColumnType("integer");

                    b.Property<string>("Unit")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.HasKey("Id");

                    b.ToTable("PanelMetric", "tsc");
                });

            modelBuilder.Entity("Masa.Tsc.Domain.Instruments.Aggregates.Instrument", b =>
                {
                    b.HasOne("Masa.Tsc.Domain.Instruments.Aggregates.Directory", "Directory")
                        .WithMany("Instruments")
                        .HasForeignKey("DirectoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Directory");
                });

            modelBuilder.Entity("Masa.Tsc.Domain.Instruments.Aggregates.Directory", b =>
                {
                    b.Navigation("Instruments");
                });
#pragma warning restore 612, 618
        }
    }
}
