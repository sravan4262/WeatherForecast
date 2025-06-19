using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherForecast.Domain.Entities;

namespace WeatherForecast.Domain.DataAccess
{
    public class WeatherForecastDbContext : DbContext
    {
        public DbSet<Location> Location { get; set; }
        public DbSet<Entities.WeatherForecast> WeatherForecast { get; set; }

        public WeatherForecastDbContext(DbContextOptions<WeatherForecastDbContext> options) : base(options) 
        { 
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WeatherForecastDbContext).Assembly);
            modelBuilder.Entity<Entities.WeatherForecast>()
            .HasOne(w => w.Location)
            .WithMany(l => l.WeatherForecasts)
            .HasForeignKey(w => w.LocationId)
            .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
