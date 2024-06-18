﻿// <auto-generated />
using System;
using Hm.Scheduling.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Hm.Scheduling.Infrastructure.Database.Migrations
{
    [DbContext(typeof(HmDbContext))]
    [Migration("20240618221740_InitialSchema")]
    partial class InitialSchema
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Hm.Scheduling.Core.Entities.AppointmentAvailability", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("EndTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("ProviderId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("StartTime")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("ProviderId");

                    b.ToTable("Availabilities");
                });

            modelBuilder.Entity("Hm.Scheduling.Core.Entities.Reservation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AppointmentAvailabilityId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset?>("ConfirmedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("EndTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("ExpiresOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("StartTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("AppointmentAvailabilityId", "UserId", "StartTime")
                        .IsUnique();

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("Hm.Scheduling.Core.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTimeOffset?>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<string>("Prefix")
                        .HasMaxLength(16)
                        .HasColumnType("character varying(16)");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Hm.Scheduling.Core.Entities.AppointmentAvailability", b =>
                {
                    b.HasOne("Hm.Scheduling.Core.Entities.User", "Provider")
                        .WithMany("Availabilities")
                        .HasForeignKey("ProviderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Provider");
                });

            modelBuilder.Entity("Hm.Scheduling.Core.Entities.Reservation", b =>
                {
                    b.HasOne("Hm.Scheduling.Core.Entities.AppointmentAvailability", "AppointmentAvailability")
                        .WithMany("Reservations")
                        .HasForeignKey("AppointmentAvailabilityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Hm.Scheduling.Core.Entities.User", "User")
                        .WithMany("Reservations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppointmentAvailability");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Hm.Scheduling.Core.Entities.AppointmentAvailability", b =>
                {
                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("Hm.Scheduling.Core.Entities.User", b =>
                {
                    b.Navigation("Availabilities");

                    b.Navigation("Reservations");
                });
#pragma warning restore 612, 618
        }
    }
}