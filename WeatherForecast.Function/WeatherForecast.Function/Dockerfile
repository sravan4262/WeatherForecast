# WeatherForecast.Function/Dockerfile (This file should be placed directly in your function project directory)

# Stage 1: Build the Function App
# Using .NET 9.0 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy just the .csproj file first for dependency restoration caching
# This assumes WeatherForecast.Function.csproj is in the same directory as this Dockerfile
COPY WeatherForecast.Function.csproj .

# Restore NuGet package dependencies
RUN dotnet restore WeatherForecast.Function.csproj

# Copy all remaining source code files
# This copies everything else from your function project folder into /src
COPY . .

# Build and publish the Function App
# The output will be placed in /app/publish
RUN dotnet publish "WeatherForecast.Function.csproj" -c Release -o /app/publish /p:UseAppHost=false

# <--- IMPORTANT: THIS LINE WAS MISSING IN YOUR PREVIOUS SNIPPET --->
FROM build AS publish
# This 'publish' stage is implicitly still in WORKDIR /src from the 'build' stage
# The 'dotnet publish' command already happened in the 'build' stage and put output in /app/publish
# So, this stage serves as the source for the 'COPY --from=publish' in the final stage.

# Stage 2: Create the final Function App image
# Using the stable and available .NET 8.0 Azure Functions runtime image
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0 AS final
WORKDIR /home/site/wwwroot

# Copy the published application from the 'publish' stage
COPY --from=publish /app/publish .

# Set the Azure Functions runtime version (optional, but good practice)
ENV AzureWebJobsDotNetWorkerExtensionVersion="~4"