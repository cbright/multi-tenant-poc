// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Contracts;
using MassTransit;
using Azure.Messaging.ServiceBus.Administration;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<SubmitApplicationConsumer>();

            
            x.UsingAzureServiceBus((context, cfg) =>
            {
                cfg.Host(hostContext.Configuration.GetValue<string>("ServicebusConnection"));

                var instanceTenant = hostContext.Configuration.GetValue<string>("instance_tenant");

                cfg.ReceiveEndpoint($"{typeof(SubmitApplication).Name}{(!string.IsNullOrWhiteSpace(instanceTenant) ? "-" + instanceTenant : string.Empty )}-queue", e => {

                    e.Subscribe<SubmitApplication>($"{typeof(SubmitApplication).Name}{ (!string.IsNullOrWhiteSpace(instanceTenant) ? "-" + instanceTenant : string.Empty)}", cfg => {
                        {
                            if (!string.IsNullOrWhiteSpace(instanceTenant))
                            {
                                var filter = new CorrelationRuleFilter();
                                filter.ApplicationProperties.Add("tenant", instanceTenant);
                                cfg.Rule = new CreateRuleOptions("tenant-routing-rule", filter);
                            }
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


