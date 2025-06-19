using System.ComponentModel.DataAnnotations;

namespace WeatherForecast.Areas.Location.V1.ViewModels
{
    public class LocationUpdateRequestModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
    }
}
