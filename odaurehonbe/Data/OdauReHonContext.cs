//using System;
//using System.Collections.Generic;
//using Microsoft.EntityFrameworkCore;

//namespace odaurehonbe.Data;

//public partial class OdauReHonContext : DbContext
//{
//    public OdauReHonContext()
//    {
//    }

//    public OdauReHonContext(DbContextOptions<OdauReHonContext> options)
//        : base(options)
//    {
//    }

//    public virtual DbSet<Account> Accounts { get; set; }

//    public virtual DbSet<Bus> Buses { get; set; }

//    public virtual DbSet<BusBusRoute> BusBusRoutes { get; set; }

//    public virtual DbSet<BusDriver> BusDrivers { get; set; }

//    public virtual DbSet<BusRoute> BusRoutes { get; set; }

//    public virtual DbSet<Customer> Customers { get; set; }

//    public virtual DbSet<Driver> Drivers { get; set; }

//    public virtual DbSet<Notification> Notifications { get; set; }

//    public virtual DbSet<Param> Params { get; set; }

//    public virtual DbSet<Payment> Payments { get; set; }

//    public virtual DbSet<Promotion> Promotions { get; set; }

//    public virtual DbSet<Seat> Seats { get; set; }

//    public virtual DbSet<Ticket> Tickets { get; set; }

//    public virtual DbSet<TicketClerk> TicketClerks { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseNpgsql("Host=localhost;Database=ODauReHon;Username=postgres;Password=160104");

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        modelBuilder.Entity<Account>(entity =>
//        {
//            entity.Property(e => e.AccountId).HasColumnName("AccountID");
//        });

//        modelBuilder.Entity<Bus>(entity =>
//        {
//            entity.Property(e => e.BusId).HasColumnName("BusID");
//            entity.Property(e => e.Type).HasDefaultValueSql("''::text");
//        });

//        modelBuilder.Entity<BusBusRoute>(entity =>
//        {
//            entity.HasIndex(e => e.BusId, "IX_BusBusRoutes_BusID");

//            entity.HasIndex(e => e.BusRouteId, "IX_BusBusRoutes_BusRouteID");

//            entity.Property(e => e.BusBusRouteId).HasColumnName("BusBusRouteID");
//            entity.Property(e => e.BusId).HasColumnName("BusID");
//            entity.Property(e => e.BusRouteId).HasColumnName("BusRouteID");

//            entity.HasOne(d => d.Bus).WithMany(p => p.BusBusRoutes).HasForeignKey(d => d.BusId);

//            entity.HasOne(d => d.BusRoute).WithMany(p => p.BusBusRoutes).HasForeignKey(d => d.BusRouteId);
//        });

//        modelBuilder.Entity<BusDriver>(entity =>
//        {
//            entity.HasIndex(e => e.BusId, "IX_BusDrivers_BusID");

//            entity.HasIndex(e => e.DriverId, "IX_BusDrivers_DriverID");

//            entity.Property(e => e.BusDriverId).HasColumnName("BusDriverID");
//            entity.Property(e => e.BusId).HasColumnName("BusID");
//            entity.Property(e => e.DriverId).HasColumnName("DriverID");

//            entity.HasOne(d => d.Bus).WithMany(p => p.BusDrivers).HasForeignKey(d => d.BusId);

//            entity.HasOne(d => d.Driver).WithMany(p => p.BusDrivers).HasForeignKey(d => d.DriverId);
//        });

//        modelBuilder.Entity<BusRoute>(entity =>
//        {
//            entity.Property(e => e.BusRouteId).HasColumnName("BusRouteID");
//            entity.Property(e => e.ArrivalStation).HasDefaultValueSql("''::text");
//            entity.Property(e => e.DepartStation).HasDefaultValueSql("''::text");
//            entity.Property(e => e.DepartureTime).HasDefaultValueSql("'-infinity'::timestamp with time zone");
//        });

//        modelBuilder.Entity<Customer>(entity =>
//        {
//            entity.HasKey(e => e.AccountId);

//            entity.Property(e => e.AccountId)
//                .ValueGeneratedNever()
//                .HasColumnName("AccountID");

//            entity.HasOne(d => d.Account).WithOne(p => p.Customer).HasForeignKey<Customer>(d => d.AccountId);
//        });

//        modelBuilder.Entity<Driver>(entity =>
//        {
//            entity.HasKey(e => e.AccountId);

//            entity.Property(e => e.AccountId)
//                .ValueGeneratedNever()
//                .HasColumnName("AccountID");

//            entity.HasOne(d => d.Account).WithOne(p => p.Driver).HasForeignKey<Driver>(d => d.AccountId);
//        });

//        modelBuilder.Entity<Notification>(entity =>
//        {
//            entity.HasIndex(e => e.ClerkId, "IX_Notifications_ClerkID");

//            entity.HasIndex(e => e.TicketId, "IX_Notifications_TicketID");

//            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
//            entity.Property(e => e.ClerkId).HasColumnName("ClerkID");
//            entity.Property(e => e.TicketId).HasColumnName("TicketID");

//            entity.HasOne(d => d.Clerk).WithMany(p => p.Notifications).HasForeignKey(d => d.ClerkId);

//            entity.HasOne(d => d.Ticket).WithMany(p => p.Notifications).HasForeignKey(d => d.TicketId);
//        });

//        modelBuilder.Entity<Param>(entity =>
//        {
//            entity.HasNoKey();
//        });

//        modelBuilder.Entity<Payment>(entity =>
//        {
//            entity.HasIndex(e => e.PromoId, "IX_Payments_PromotionPromoID");

//            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
//            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
//            entity.Property(e => e.PromoId)
//                .HasDefaultValue(0)
//                .HasColumnName("PromoID");
//            entity.Property(e => e.StaffId).HasColumnName("StaffID");

//            entity.HasOne(d => d.Promo).WithMany(p => p.Payments)
//                .HasForeignKey(d => d.PromoId)
//                .OnDelete(DeleteBehavior.Cascade)
//                .HasConstraintName("FK_Payments_Promotions_PromotionPromoID");
//        });

//        modelBuilder.Entity<Promotion>(entity =>
//        {
//            entity.HasKey(e => e.PromoId);

//            entity.Property(e => e.PromoId).HasColumnName("PromoID");
//            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");

//            entity.HasOne(d => d.Payment).WithMany(p => p.Promotions).HasForeignKey(d => d.PaymentId);
//        });

//        modelBuilder.Entity<Seat>(entity =>
//        {
//            entity.HasIndex(e => e.BusBusRouteId, "IX_Seats_BusBusRouteID");

//            entity.Property(e => e.SeatId).HasColumnName("SeatID");
//            entity.Property(e => e.BusBusRouteId)
//                .HasDefaultValue(0)
//                .HasColumnName("BusBusRouteID");

//            entity.HasOne(d => d.BusBusRoute).WithMany(p => p.Seats).HasForeignKey(d => d.BusBusRouteId);
//        });

//        modelBuilder.Entity<Ticket>(entity =>
//        {
//            entity.HasIndex(e => e.CustomerId, "IX_Tickets_CustomerID");

//            entity.HasIndex(e => e.PaymentId, "IX_Tickets_PaymentID");

//            entity.Property(e => e.TicketId).HasColumnName("TicketID");
//            entity.Property(e => e.BusBusRouteId).HasColumnName("BusBusRouteID");
//            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
//            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");

//            entity.HasOne(d => d.Payment).WithMany(p => p.Tickets).HasForeignKey(d => d.PaymentId);
//        });

//        modelBuilder.Entity<TicketClerk>(entity =>
//        {
//            entity.HasKey(e => e.AccountId);

//            entity.Property(e => e.AccountId)
//                .ValueGeneratedNever()
//                .HasColumnName("AccountID");

//            entity.HasOne(d => d.Account).WithOne(p => p.TicketClerk).HasForeignKey<TicketClerk>(d => d.AccountId);
//        });

//        OnModelCreatingPartial(modelBuilder);
//    }

//    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
//}
