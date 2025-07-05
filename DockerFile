# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file and project files
COPY WeatherForecast/WeatherForecast.sln WeatherForecast/
COPY WeatherForecast/WeatherForecast.csproj WeatherForecast/
COPY WeatherForecast.Domain/WeatherForecast.Domain.csproj WeatherForecast.Domain/
COPY WeatherForecast.Domain.UnitTests/WeatherForecast.Domain.UnitTests.csproj WeatherForecast.Domain.UnitTests/
COPY WeatherForecast.WebApi.UnitTests/WeatherForecast.WebApi.UnitTests.csproj WeatherForecast.WebApi.UnitTests/

# Restore dependencies
WORKDIR /src/WeatherForecast
RUN dotnet restore WeatherForecast.sln

# Copy the rest of the code
WORKDIR /src
COPY . .

# Build and publish the main Web API project
WORKDIR /src/WeatherForecast
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 80

ENTRYPOINT ["dotnet", "WeatherForecast.dll"]
