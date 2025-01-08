using Microsoft.EntityFrameworkCore;

namespace odaurehonbe.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Param>().HasNoKey();
            modelBuilder.Entity<BusDriver>()
                .HasOne(bd => bd.Bus)
                .WithMany(b => b.BusDrivers)
                .HasForeignKey(bd => bd.BusID);

            modelBuilder.Entity<BusDriver>()
                .HasOne(bd => bd.Driver)
                .WithMany(d => d.BusDrivers)
                .HasForeignKey(bd => bd.DriverID);

            modelBuilder.Entity<BusBusRoute>()
                .HasOne(br => br.Bus)
                .WithMany(b => b.BusBusRoutes)
                .HasForeignKey(br => br.BusID);

            modelBuilder.Entity<BusBusRoute>()
                .HasOne(br => br.BusRoute)
                .WithMany(br => br.BusBusRoutes)
                .HasForeignKey(br => br.BusRouteID);
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Customer)
                .WithOne(c => c.Account)
                .HasForeignKey<Customer>(c => c.AccountID);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Driver)
                .WithOne(d => d.Account)
                .HasForeignKey<Driver>(d => d.AccountID);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.TicketClerk)
                .WithOne(tc => tc.Account)
                .HasForeignKey<TicketClerk>(tc => tc.AccountID);
            modelBuilder.Entity<Seat>()
              .HasOne(s => s.BusBusRoute)
              .WithMany(b => b.Seats)
              .HasForeignKey(s => s.BusBusRouteID);

            // Cấu hình cho Payment và Promotion
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Promo)
                .WithOne(p => p.Payment)
                .HasForeignKey<Payment>(p => p.PromoID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Payments_Promotions");

            modelBuilder.Entity<Promotion>()
                .HasOne(p => p.Payment)
                .WithOne(p => p.Promo)
                .HasForeignKey<Promotion>(p => p.PaymentID);

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<BusDriver> BusDrivers { get; set; }
        public DbSet<BusBusRoute> BusBusRoutes { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<BusRoute> BusRoutes { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketClerk> TicketClerks { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Param> Params { get; set; }
    }
}
