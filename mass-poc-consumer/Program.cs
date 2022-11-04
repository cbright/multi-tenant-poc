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

                //x.SetKebabCaseEndpointNameFormatter();

                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(hostContext.Configuration.GetValue<string>("ServicebusConnection"));

                    var tenantSubscription = new List<string>();
                    tenantSubscription = context.GetService<IConfiguration>().GetSection("DedicatedTenants").Get<string[]>().ToList();

                    if(!tenantSubscription.Any())
                    {
                        throw new Exception("No subscription defined. At a minimum \"multitenant\" must be passed as configuration for \"DedicatedTenants\" array section.");
                    }

                    foreach(var subscriptionIdentifier in tenantSubscription)
                    {
                        cfg.ReceiveEndpoint($"{typeof(SubmitApplication).Name}_{subscriptionIdentifier}_queue", e =>
                        {

                            e.Subscribe<SubmitApplication>($"{typeof(SubmitApplication).Name}_{subscriptionIdentifier}", sub =>
                            {
                                //This must be mutually exclusive or duplicate messages will be delivered.
                                var filter = new CorrelationRuleFilter();
                                filter.ApplicationProperties.Add("route-key", subscriptionIdentifier);
                                sub.Rule = new CreateRuleOptions("tenant_routing_rule", filter);
                                
                            });

                            e.ConfigureConsumeTopology = false;
                            e.ConfigureConsumer<SubmitApplicationConsumer>(context);

                        });
                    }

                   // cfg.ConfigureEndpoints(context);
                
                });
            });

    })
    .Build();

host.Run();