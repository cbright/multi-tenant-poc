using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration.UserSecrets;
using MassTransit;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMassTransit(x =>
            {
                // elided ...
                x.UsingAzureServiceBus((context,cfg) =>
                {
                    cfg.Host(hostContext.Configuration.GetValue<string>("ServicebusConnection"));

                    cfg.ConfigureEndpoints(context);
                
                });
            });

        services.AddHostedService<Worker>();

    })
    .Build();

host.Run();