using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast.Domain.Entities.Configurations
{
    public class WeatherForecastEntityConfiguration : IEntityTypeConfiguration<WeatherForecast>
    {
        public void Configure(EntityTypeBuilder<WeatherForecast> builder)
        {
            builder.HasKey(weatherForecast => weatherForecast.Id);
            builder.Property(weatherForecast => weatherForecast.Id).ValueGeneratedOnAdd();

            builder.Property(weatherForecast => weatherForecast.Temperature);
            builder.Property(weatherForecast => weatherForecast.WindSpeed);
            builder.Property(weatherForecast => weatherForecast.WeatherCode);
            builder.Property(weatherForecast => weatherForecast.Timezone);
            builder.Property(weatherForecast => weatherForecast.LocationId);


            //builder.HasOne<Location>().WithMany(e => e.WeatherForecasts).HasForeignKey(wf => wf.LocationId).IsRequired();
        }
    }
}
