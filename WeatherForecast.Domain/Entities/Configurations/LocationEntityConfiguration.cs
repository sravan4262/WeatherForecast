using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WeatherForecast.Domain.Entities.Configurations
{
    public class LocationEntityConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
           
            builder.HasKey(location => location.Id);
            builder.Property(location => location.Id).ValueGeneratedOnAdd();

            builder.Property(location => location.Latitude).IsRequired();
            builder.Property(location => location.Longitude).IsRequired();
            builder.Property(location => location.AccessedDateTime).IsRequired();
        }
    }
}
