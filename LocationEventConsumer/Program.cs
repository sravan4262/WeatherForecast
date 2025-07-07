using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

var builder = Host.CreateApplicationBuilder(args);

// Configuration is automatically loaded from appsettings.json

// Bind ServiceBus connection string from config
var serviceBusConnectionString = builder.Configuration.GetSection("ServiceBus").GetValue<string>("ConnectionString");

builder.Services.AddSingleton(serviceProvider =>
{
    if (string.IsNullOrEmpty(serviceBusConnectionString))
        throw new InvalidOperationException("ServiceBus connection string is not configured.");

    return new ServiceBusClient(serviceBusConnectionString);
});

builder.Services.AddHostedService<LocationInsertedEventConsumerWorker>();

var host = builder.Build();
host.Run();
