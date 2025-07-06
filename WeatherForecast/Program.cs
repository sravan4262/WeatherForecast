using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using System;
using WeatherForecast.Areas.Location.V1.Mappers;
using WeatherForecast.Areas.WeatherForecast.V1.Mappers;
using WeatherForecast.Domain.Business.Classes;
using WeatherForecast.Domain.Business.Interfaces;
using WeatherForecast.Domain.DataAccess;
using WeatherForecast.Domain.DataAccess.Classes;
using WeatherForecast.Domain.DataAccess.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(80); // required for Docker/Azure
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
}); 
builder.Services.AddDbContext<WeatherForecastDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WeatherForecast")));

// --- Configure Azure AD Authentication ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// --- Configure Authorization Policies for Scopes ---
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequiresForecastRetrieveAllScope", policy =>
    {
        policy.RequireAuthenticatedUser(); // Must be authenticated
        policy.RequireClaim("http://schemas.microsoft.com/identity/claims/scope", "location.locations");
    });

    // Optional: A default policy if you want all APIs to require authentication by default
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WeatherForecast API", Version = "v1" });

    // Define the security scheme for JWT Bearer tokens
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Require the Bearer token for all operations (can be refined per operation)
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();

app.UseHttpsRedirection();

if (!app.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
        serverOptions.ListenAnyIP(80); // required for Docker/Azure
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
