// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Contracts;
using MassTransit;
using Azure.Messaging.ServiceBus.Administration;


var multiTenant = false;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMassTransit(x =>
            {
                x.AddConsumer<SubmitApplicationConsumer>();

                // elided ...
                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(hostContext.Configuration.GetValue<string>("ServicebusConnection"));

                    cfg.ReceiveEndpoint($"{typeof(SubmitApplication).Name}{ (!multiTenant ? "_ohbprgfh" : string.Empty)}_queue", e => {

                        e.Subscribe<SubmitApplication>($"{typeof(SubmitApplication).Name}{ (!multiTenant ? "_ohbprgfh" : string.Empty)}",cfg => {
                            {
                                var filter = new CorrelationRuleFilter();
                                filter.ApplicationProperties.Add("tenant", "ohbprgfh");
                                cfg.Rule = new CreateRuleOptions("tenant_routing_rule", filter);
                             }
                        });
                        e.ConfigureConsumer<SubmitApplicationConsumer>(context);

                    });


                
                    cfg.ConfigureEndpoints(context);
                
                });
            });

    })
    .Build();

host.Run();