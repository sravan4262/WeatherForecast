using System.ComponentModel.DataAnnotations;

namespace WeatherForecast.Areas.Location.V1.ViewModels
{
    public class LocationInsertRequestModel
    {
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
    }
}
