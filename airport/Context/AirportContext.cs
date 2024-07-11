using airport.Models;
using Microsoft.EntityFrameworkCore;

namespace airport.Context
{
    public class AirportContext : DbContext
    {
        public AirportContext(DbContextOptions<AirportContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Flight>()
                .HasQueryFilter(a => !a.isDeleted);

            modelBuilder.Entity<Plan>()
                .HasQueryFilter(a => !a.isDeleted);

            modelBuilder.Entity<Reservation>()
                .HasQueryFilter(a => !a.isDeleted);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Flights)
                .WithMany(u => u.Users)
                .UsingEntity<Reservation>();

            modelBuilder.Entity<Flight>()
                .HasMany(f => f.Plans)
                .WithOne(f => f.Flight)
                .HasForeignKey(f => f.FlightId)
                .IsRequired();

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Plan)
                .WithMany()
                .HasForeignKey(r => r.PlanId)
                .IsRequired();
        }
    }
}
