﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using odaurehonbe.Data;

#nullable disable

namespace odaurehonbe.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BusBusRoute", b =>
                {
                    b.Property<int>("BusBusRouteID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("BusBusRouteID"));

                    b.Property<int>("BusID")
                        .HasColumnType("integer");

                    b.Property<int>("BusRouteID")
                        .HasColumnType("integer");

                    b.HasKey("BusBusRouteID");

                    b.HasIndex("BusID");

                    b.HasIndex("BusRouteID");

                    b.ToTable("BusBusRoutes");
                });

            modelBuilder.Entity("BusDriver", b =>
                {
                    b.Property<int>("BusDriverID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("BusDriverID"));

                    b.Property<int>("BusID")
                        .HasColumnType("integer");

                    b.Property<int>("DriverID")
                        .HasColumnType("integer");

                    b.HasKey("BusDriverID");

                    b.HasIndex("BusID");

                    b.HasIndex("DriverID");

                    b.ToTable("BusDrivers");
                });

            modelBuilder.Entity("Driver", b =>
                {
                    b.Property<int>("AccountID")
                        .HasColumnType("integer");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LicenseNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("AccountID");

                    b.ToTable("Drivers");
                });

            modelBuilder.Entity("odaurehonbe.Data.Account", b =>
                {
                    b.Property<int>("AccountID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("AccountID"));

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("AccountID");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("odaurehonbe.Data.Bus", b =>
                {
                    b.Property<int>("BusID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("BusID"));

                    b.Property<int>("NumSeat")
                        .HasColumnType("integer");

                    b.Property<string>("PlateNum")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("BusID");

                    b.ToTable("Buses");
                });

            modelBuilder.Entity("odaurehonbe.Data.BusRoute", b =>
                {
                    b.Property<int>("BusRouteID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("BusRouteID"));

                    b.Property<string>("ArrivalPlace")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ArrivalStation")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DepartPlace")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DepartStation")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("DepartureTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Duration")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("PricePerSeat")
                        .HasColumnType("numeric");

                    b.Property<decimal>("PricePerSeatVip")
                        .HasColumnType("numeric");

                    b.HasKey("BusRouteID");

                    b.ToTable("BusRoutes");
                });

            modelBuilder.Entity("odaurehonbe.Data.Customer", b =>
                {
                    b.Property<int>("AccountID")
                        .HasColumnType("integer");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("AccountID");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("odaurehonbe.Data.Notification", b =>
                {
                    b.Property<int>("NotificationID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("NotificationID"));

                    b.Property<int?>("ClerkID")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsHandled")
                        .HasColumnType("boolean");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("TicketID")
                        .HasColumnType("integer");

                    b.HasKey("NotificationID");

                    b.HasIndex("ClerkID");

                    b.HasIndex("TicketID");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("odaurehonbe.Data.Param", b =>
                {
                    b.Property<DateTime>("CancelDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("MaxPromo")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("PaymentTerm")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("RefundableDate")
                        .HasColumnType("timestamp with time zone");

                    b.ToTable("Params");
                });

            modelBuilder.Entity("odaurehonbe.Data.Payment", b =>
                {
                    b.Property<int>("PaymentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("PaymentID"));

                    b.Property<int?>("CustomerID")
                        .HasColumnType("integer");

                    b.Property<string>("PaymentMethod")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("PaymentTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("PromoID")
                        .HasColumnType("integer");

                    b.Property<int?>("StaffID")
                        .HasColumnType("integer");

                    b.Property<decimal>("TotalFee")
                        .HasColumnType("numeric");

                    b.HasKey("PaymentID");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("odaurehonbe.Data.Promotion", b =>
                {
                    b.Property<int>("PromoID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("PromoID"));

                    b.Property<decimal>("Discount")
                        .HasColumnType("numeric");

                    b.Property<float>("DiscountPercent")
                        .HasColumnType("real");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("PaymentID")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("PromoID");

                    b.HasIndex("PaymentID")
                        .IsUnique();

                    b.ToTable("Promotions");
                });

            modelBuilder.Entity("odaurehonbe.Data.Seat", b =>
                {
                    b.Property<int>("SeatID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("SeatID"));

                    b.Property<int>("BusBusRouteID")
                        .HasColumnType("integer");

                    b.Property<bool>("IsBooked")
                        .HasColumnType("boolean");

                    b.Property<string>("SeatNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("SeatID");

                    b.HasIndex("BusBusRouteID");

                    b.ToTable("Seats");
                });

            modelBuilder.Entity("odaurehonbe.Data.Ticket", b =>
                {
                    b.Property<int>("TicketID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("TicketID"));

                    b.Property<DateTime>("BookingDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("BusBusRouteID")
                        .HasColumnType("integer");

                    b.Property<int>("CustomerID")
                        .HasColumnType("integer");

                    b.Property<int?>("PaymentID")
                        .HasColumnType("integer");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<int>("SeatNum")
                        .HasColumnType("integer");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("TicketID");

                    b.HasIndex("PaymentID");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("odaurehonbe.Data.TicketClerk", b =>
                {
                    b.Property<int>("AccountID")
                        .HasColumnType("integer");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly>("HireDate")
                        .HasColumnType("date");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("AccountID");

                    b.ToTable("TicketClerks");
                });

            modelBuilder.Entity("BusBusRoute", b =>
                {
                    b.HasOne("odaurehonbe.Data.Bus", "Bus")
                        .WithMany("BusBusRoutes")
                        .HasForeignKey("BusID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("odaurehonbe.Data.BusRoute", "BusRoute")
                        .WithMany("BusBusRoutes")
                        .HasForeignKey("BusRouteID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bus");

                    b.Navigation("BusRoute");
                });

            modelBuilder.Entity("BusDriver", b =>
                {
                    b.HasOne("odaurehonbe.Data.Bus", "Bus")
                        .WithMany("BusDrivers")
                        .HasForeignKey("BusID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Driver", "Driver")
                        .WithMany("BusDrivers")
                        .HasForeignKey("DriverID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bus");

                    b.Navigation("Driver");
                });

            modelBuilder.Entity("Driver", b =>
                {
                    b.HasOne("odaurehonbe.Data.Account", "Account")
                        .WithOne("Driver")
                        .HasForeignKey("Driver", "AccountID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("odaurehonbe.Data.Customer", b =>
                {
                    b.HasOne("odaurehonbe.Data.Account", "Account")
                        .WithOne("Customer")
                        .HasForeignKey("odaurehonbe.Data.Customer", "AccountID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("odaurehonbe.Data.Notification", b =>
                {
                    b.HasOne("odaurehonbe.Data.TicketClerk", "Clerk")
                        .WithMany("Notifications")
                        .HasForeignKey("ClerkID");

                    b.HasOne("odaurehonbe.Data.Ticket", "Ticket")
                        .WithMany("Notifications")
                        .HasForeignKey("TicketID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Clerk");

                    b.Navigation("Ticket");
                });

            modelBuilder.Entity("odaurehonbe.Data.Promotion", b =>
                {
                    b.HasOne("odaurehonbe.Data.Payment", "Payment")
                        .WithOne("Promo")
                        .HasForeignKey("odaurehonbe.Data.Promotion", "PaymentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_Payments_Promotions");

                    b.Navigation("Payment");
                });

            modelBuilder.Entity("odaurehonbe.Data.Seat", b =>
                {
                    b.HasOne("BusBusRoute", "BusBusRoute")
                        .WithMany("Seats")
                        .HasForeignKey("BusBusRouteID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BusBusRoute");
                });

            modelBuilder.Entity("odaurehonbe.Data.Ticket", b =>
                {
                    b.HasOne("odaurehonbe.Data.Payment", "Payment")
                        .WithMany("Tickets")
                        .HasForeignKey("PaymentID");

                    b.Navigation("Payment");
                });

            modelBuilder.Entity("odaurehonbe.Data.TicketClerk", b =>
                {
                    b.HasOne("odaurehonbe.Data.Account", "Account")
                        .WithOne("TicketClerk")
                        .HasForeignKey("odaurehonbe.Data.TicketClerk", "AccountID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("BusBusRoute", b =>
                {
                    b.Navigation("Seats");
                });

            modelBuilder.Entity("Driver", b =>
                {
                    b.Navigation("BusDrivers");
                });

            modelBuilder.Entity("odaurehonbe.Data.Account", b =>
                {
                    b.Navigation("Customer");

                    b.Navigation("Driver");

                    b.Navigation("TicketClerk");
                });

            modelBuilder.Entity("odaurehonbe.Data.Bus", b =>
                {
                    b.Navigation("BusBusRoutes");

                    b.Navigation("BusDrivers");
                });

            modelBuilder.Entity("odaurehonbe.Data.BusRoute", b =>
                {
                    b.Navigation("BusBusRoutes");
                });

            modelBuilder.Entity("odaurehonbe.Data.Payment", b =>
                {
                    b.Navigation("Promo");

                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("odaurehonbe.Data.Ticket", b =>
                {
                    b.Navigation("Notifications");
                });

            modelBuilder.Entity("odaurehonbe.Data.TicketClerk", b =>
                {
                    b.Navigation("Notifications");
                });
#pragma warning restore 612, 618
        }
    }
}
