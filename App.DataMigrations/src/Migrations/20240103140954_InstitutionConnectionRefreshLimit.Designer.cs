﻿// <auto-generated />
using System;
using App.Lib.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace App.DataMigrations.src.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20240103140954_InstitutionConnectionRefreshLimit")]
    partial class InstitutionConnectionRefreshLimit
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("App.Lib.Data.Entity.InstitutionAccountEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExternalId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Iban")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ImportStatus")
                        .HasColumnType("int");

                    b.Property<Guid>("InstitutionConnectionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("LastImport")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastImportError")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastImportRequested")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("InstitutionConnectionId");

                    b.HasIndex("ExternalId", "InstitutionConnectionId")
                        .IsUnique();

                    b.ToTable("InstitutionAccounts");
                });

            modelBuilder.Entity("App.Lib.Data.Entity.InstitutionAccountTransactionEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,4)");

                    b.Property<string>("CounterPartyAccount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CounterPartyName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExternalId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("TransactionCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UnstructuredInformation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("ExternalId", "AccountId")
                        .IsUnique();

                    b.ToTable("InstitutionAccountTransactions");
                });

            modelBuilder.Entity("App.Lib.Data.Entity.InstitutionConnectionEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConnectUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExternalId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid>("InstitutionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("OrganisationId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ExternalId")
                        .IsUnique();

                    b.HasIndex("InstitutionId");

                    b.HasIndex("OrganisationId");

                    b.ToTable("InstitutionConnections");
                });

            modelBuilder.Entity("App.Lib.Data.Entity.InstitutionEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CountryIso2")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExternalId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Logo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ExternalId")
                        .IsUnique();

                    b.ToTable("Institutions");
                });

            modelBuilder.Entity("App.Lib.Data.Entity.OrganisationEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.ToTable("Organisations");
                });

            modelBuilder.Entity("App.Lib.Data.Entity.OrganisationUserEntity", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("OrganisationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "OrganisationId");

                    b.HasIndex("OrganisationId");

                    b.ToTable("OrganisationUser");
                });

            modelBuilder.Entity("App.Lib.Data.Entity.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ExternalId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("App.Lib.Data.Entity.InstitutionAccountEntity", b =>
                {
                    b.HasOne("App.Lib.Data.Entity.InstitutionConnectionEntity", "InstitutionConnection")
                        .WithMany("Accounts")
                        .HasForeignKey("InstitutionConnectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("InstitutionConnection");
                });

            modelBuilder.Entity("App.Lib.Data.Entity.InstitutionAccountTransactionEntity", b =>
                {
                    b.HasOne("App.Lib.Data.Entity.InstitutionAccountEntity", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("App.Lib.Data.Entity.InstitutionConnectionEntity", b =>
                {
                    b.HasOne("App.Lib.Data.Entity.InstitutionEntity", "Institution")
                        .WithMany()
                        .HasForeignKey("InstitutionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("App.Lib.Data.Entity.OrganisationEntity", "Organisation")
                        .WithMany("InstitutionConnections")
                        .HasForeignKey("OrganisationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Institution");

                    b.Navigation("Organisation");
                });

            modelBuilder.Entity("App.Lib.Data.Entity.OrganisationUserEntity", b =>
                {
                    b.HasOne("App.Lib.Data.Entity.OrganisationEntity", "Organisation")
                        .WithMany("Users")
                        .HasForeignKey("OrganisationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("App.Lib.Data.Entity.UserEntity", "User")
                        .WithMany("Organisations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Organisation");

                    b.Navigation("User");
                });

            modelBuilder.Entity("App.Lib.Data.Entity.InstitutionConnectionEntity", b =>
                {
                    b.Navigation("Accounts");
                });

            modelBuilder.Entity("App.Lib.Data.Entity.OrganisationEntity", b =>
                {
                    b.Navigation("InstitutionConnections");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("App.Lib.Data.Entity.UserEntity", b =>
                {
                    b.Navigation("Organisations");
                });
#pragma warning restore 612, 618
        }
    }
}
