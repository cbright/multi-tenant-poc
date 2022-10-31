using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MassTransit;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMassTransit(x =>
            {
                // elided ...
                x.UsingAzureServiceBus((context,cfg) =>
                {
                    cfg.Host("Endpoint=sb://mass-poc.servicebus.windows.net/;SharedAccessKeyName=poc;SharedAccessKey=P6fGMFCmXbmNNPlVWhszI6XF9Mg6U2SjP93zioXukAk=");

                    cfg.ConfigureEndpoints(context);
                
                });
            });

        services.AddHostedService<Worker>();

    })
    .Build();

host.Run();