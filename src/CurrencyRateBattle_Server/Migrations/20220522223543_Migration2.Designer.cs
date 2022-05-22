﻿// <auto-generated />
using System;
using CurrencyRateBattleServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CurrencyRateBattleServer.Migrations
{
    [DbContext(typeof(CurrencyRateBattleContext))]
    [Migration("20220522223543_Migration2")]
    partial class Migration2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CurrencyRateBattleServer.Models.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Account", (string)null);
                });

            modelBuilder.Entity("CurrencyRateBattleServer.Models.AccountHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsCredit")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("RoomId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("RoomId");

                    b.ToTable("AccountHistory", (string)null);
                });

            modelBuilder.Entity("CurrencyRateBattleServer.Models.Currency", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CurrencyName")
                        .IsRequired()
                        .HasColumnType("varchar(3)");

                    b.Property<string>("CurrencySymbol")
                        .IsRequired()
                        .HasColumnType("varchar(3)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("varchar(128)");

                    b.HasKey("Id");

                    b.ToTable("Currency", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("4e19a30d-0778-4066-9880-221053bed5cd"),
                            CurrencyName = "USD",
                            CurrencySymbol = "$",
                            Description = "US Dollar"
                        },
                        new
                        {
                            Id = new Guid("ab84e758-3939-46cf-a45a-76d9372cf3e3"),
                            CurrencyName = "EUR",
                            CurrencySymbol = "$",
                            Description = "Euro"
                        },
                        new
                        {
                            Id = new Guid("f8de1def-1776-44b1-9b7b-fc6a16e4712d"),
                            CurrencyName = "PLN",
                            CurrencySymbol = "zł",
                            Description = "Polish Zlotych"
                        },
                        new
                        {
                            Id = new Guid("d748c53f-3f4c-42e8-99e5-717c00fa3678"),
                            CurrencyName = "GBP",
                            CurrencySymbol = "£",
                            Description = "British Pound"
                        },
                        new
                        {
                            Id = new Guid("08c5fc6a-63d5-4edf-a0f2-ea715200ba45"),
                            CurrencyName = "CHF",
                            CurrencySymbol = "Fr",
                            Description = "Swiss Franc"
                        });
                });

            modelBuilder.Entity("CurrencyRateBattleServer.Models.CurrencyState", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("CurrencyExchangeRate")
                        .HasColumnType("numeric");

                    b.Property<Guid>("CurrencyId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("RoomId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("RoomId");

                    b.ToTable("CurrencyState", (string)null);
                });

            modelBuilder.Entity("CurrencyRateBattleServer.Models.Rate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<Guid>("CurrencyId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsClosed")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<bool>("IsWon")
                        .HasColumnType("boolean");

                    b.Property<Guid>("RoomId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("SetDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("RoomId");

                    b.ToTable("Rate", (string)null);
                });

            modelBuilder.Entity("CurrencyRateBattleServer.Models.Room", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsClosed")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.HasKey("Id");

                    b.ToTable("Room", (string)null);
                });

            modelBuilder.Entity("CurrencyRateBattleServer.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("CurrencyRateBattleServer.Models.Account", b =>
                {
                    b.HasOne("CurrencyRateBattleServer.Models.User", "User")
                        .WithOne("Account")
                        .HasForeignKey("CurrencyRateBattleServer.Models.Account", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CurrencyRateBattleServer.Models.AccountHistory", b =>
                {
                    b.HasOne("CurrencyRateBattleServer.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CurrencyRateBattleServer.Models.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId");

                    b.Navigation("Account");

                    b.Navigation("Room");
                });

            modelBuilder.Entity("CurrencyRateBattleServer.Models.CurrencyState", b =>
                {
                    b.HasOne("CurrencyRateBattleServer.Models.Currency", "Currency")
                        .WithMany()
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CurrencyRateBattleServer.Models.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Currency");

                    b.Navigation("Room");
                });

            modelBuilder.Entity("CurrencyRateBattleServer.Models.Rate", b =>
                {
                    b.HasOne("CurrencyRateBattleServer.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CurrencyRateBattleServer.Models.Currency", "Currency")
                        .WithMany()
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CurrencyRateBattleServer.Models.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Currency");

                    b.Navigation("Room");
                });

            modelBuilder.Entity("CurrencyRateBattleServer.Models.User", b =>
                {
                    b.Navigation("Account")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
