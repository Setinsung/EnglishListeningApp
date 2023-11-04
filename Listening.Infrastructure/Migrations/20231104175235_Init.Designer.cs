﻿// <auto-generated />
using System;
using Listening.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Listening.Infrastructure.Migrations
{
    [DbContext(typeof(ListeningDbContext))]
    [Migration("20231104175235_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Listening.Domain.Entities.Album", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsVisible")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastModifiedTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("SequenceNumber")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("Id"), false);

                    b.HasIndex("CategoryId", "IsDeleted");

                    b.ToTable("T_Albums", (string)null);
                });

            modelBuilder.Entity("Listening.Domain.Entities.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CoverUrl")
                        .HasMaxLength(500)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastModifiedTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("SequenceNumber")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("Id"), false);

                    b.ToTable("T_Categories", (string)null);
                });

            modelBuilder.Entity("Listening.Domain.Entities.Episode", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AlbumId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AudioUrl")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedTime")
                        .HasColumnType("datetime2");

                    b.Property<double>("DurationInSecond")
                        .HasColumnType("float");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsVisible")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastModifiedTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("SequenceNumber")
                        .HasColumnType("int");

                    b.Property<string>("Subtitle")
                        .IsRequired()
                        .HasMaxLength(2147483647)
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SubtitleType")
                        .IsRequired()
                        .HasMaxLength(10)
                        .IsUnicode(false)
                        .HasColumnType("varchar(10)");

                    b.HasKey("Id");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("Id"), false);

                    b.HasIndex("AlbumId", "IsDeleted");

                    b.ToTable("T_Episodes", (string)null);
                });

            modelBuilder.Entity("Listening.Domain.Entities.Album", b =>
                {
                    b.OwnsOne("Commons.Domain.Models.MultilingualString", "Name", b1 =>
                        {
                            b1.Property<Guid>("AlbumId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Chinese")
                                .IsRequired()
                                .HasMaxLength(200)
                                .IsUnicode(true)
                                .HasColumnType("nvarchar(200)");

                            b1.Property<string>("English")
                                .IsRequired()
                                .HasMaxLength(200)
                                .IsUnicode(true)
                                .HasColumnType("nvarchar(200)");

                            b1.HasKey("AlbumId");

                            b1.ToTable("T_Albums");

                            b1.WithOwner()
                                .HasForeignKey("AlbumId");
                        });

                    b.Navigation("Name")
                        .IsRequired();
                });

            modelBuilder.Entity("Listening.Domain.Entities.Category", b =>
                {
                    b.OwnsOne("Commons.Domain.Models.MultilingualString", "Name", b1 =>
                        {
                            b1.Property<Guid>("CategoryId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Chinese")
                                .IsRequired()
                                .HasMaxLength(200)
                                .IsUnicode(true)
                                .HasColumnType("nvarchar(200)");

                            b1.Property<string>("English")
                                .IsRequired()
                                .HasMaxLength(200)
                                .IsUnicode(true)
                                .HasColumnType("nvarchar(200)");

                            b1.HasKey("CategoryId");

                            b1.ToTable("T_Categories");

                            b1.WithOwner()
                                .HasForeignKey("CategoryId");
                        });

                    b.Navigation("Name")
                        .IsRequired();
                });

            modelBuilder.Entity("Listening.Domain.Entities.Episode", b =>
                {
                    b.OwnsOne("Commons.Domain.Models.MultilingualString", "Name", b1 =>
                        {
                            b1.Property<Guid>("EpisodeId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Chinese")
                                .IsRequired()
                                .HasMaxLength(200)
                                .IsUnicode(true)
                                .HasColumnType("nvarchar(200)");

                            b1.Property<string>("English")
                                .IsRequired()
                                .HasMaxLength(200)
                                .IsUnicode(true)
                                .HasColumnType("nvarchar(200)");

                            b1.HasKey("EpisodeId");

                            b1.ToTable("T_Episodes");

                            b1.WithOwner()
                                .HasForeignKey("EpisodeId");
                        });

                    b.Navigation("Name")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
