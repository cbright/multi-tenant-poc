// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MassTransit;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMassTransit(x =>
            {
                x.AddConsumer<SubmitApplicationConsumer>();
                // elided ...
                x.UsingAzureServiceBus((context,cfg) =>
                {
                    cfg.Host(hostContext.Configuration.GetValue<string>("ServicebusConnection"));

                    cfg.ConfigureEndpoints(context);
                
                });
            });

    })
    .Build();

host.Run();