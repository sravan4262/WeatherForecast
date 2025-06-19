CREATE TABLE [dbo].[WeatherForecast](
    [Id] INT IDENTITY(1,1) PRIMARY KEY, 
    [LocationId] INT NOT NULL,
    [Temperature] FLOAT NOT NULL,
    [WindSpeed] FLOAT NOT NULL,
    [WeatherCode] INT NOT NULL,
    [Timezone] NVARCHAR(50) NOT NULL,
    
    CONSTRAINT [FK_WeatherForecast_Locations_LocationId] FOREIGN KEY ([LocationId])
    REFERENCES [dbo].[Location] ([Id]) ON DELETE CASCADE -- ON DELETE CASCADE for cascading deletes as configured in EF Core
);