using Microsoft.EntityFrameworkCore;
using System;
using WeatherForecast.Areas.Location.V1.Mappers;
using WeatherForecast.Areas.WeatherForecast.V1.Mappers;
using WeatherForecast.Domain.Business.Classes;
using WeatherForecast.Domain.Business.Interfaces;
using WeatherForecast.Domain.DataAccess;
using WeatherForecast.Domain.DataAccess.Classes;
using WeatherForecast.Domain.DataAccess.Interfaces;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
}); ;
builder.Services.AddDbContext<WeatherForecastDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WeatherForecast")));


builder.Services.AddHttpClient<IWeatherRetriever, WeatherRetriever>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AppSettings:OpenMateoBaseUrl"]);
});

// Register Mapper Dependencies
builder.Services.AddScoped<ILocationViewModelMapper, LocationViewModelMapper>();
builder.Services.AddScoped<IWeatherViewModelMapper, WeatherViewModelMapper>();

// Register Business Dependencies
builder.Services.AddScoped<ILocationRetriever, LocationRetriever>();
builder.Services.AddScoped<ILocationUpserter, LocationUpserter>();
builder.Services.AddScoped<IWeatherRetriever, WeatherRetriever>();
builder.Services.AddScoped<IWeatherUpserter, WeatherUpserter>();

// Register Data Access Dependencies
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IWeatherForecastRepository, WeatherForecastRepository>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
