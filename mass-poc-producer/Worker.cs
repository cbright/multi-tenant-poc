using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

public class Worker : BackgroundService
{
    readonly IBus _bus;
    readonly ILogger<Worker> _logger;
    private readonly List<string> _dedicatedQueueClients;

    public Worker(IBus bus, ILogger<Worker> logger, IConfiguration configuration)
    {
        _bus = bus;
        _logger = logger;
        _dedicatedQueueClients = configuration.GetSection("DedicatedTenants").Get<string[]>().ToList<string>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tenants = new List<string>{
            "ohbprgfh", //multi-tentant
            "cpoyaqru", //multi-tentant
            "ajnabrsm", //multi-tentant
            "qnaagyep",
            "fterttgd",
            "ojnqoaam",
            "ejafmupe",
            "ucprbgog" //multi-tentant
            };

        while (!stoppingToken.IsCancellationRequested)
        {
            await SimulateMultitenancy(tenants, stoppingToken);

            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task SimulateMultitenancy(IList<string> tenants, CancellationToken stoppingToken)
    {
        foreach(var tenant in tenants)
        {
            var app = new SubmitApplication(){Tenant = tenant};
            await _bus.Publish(app, context => { 
                context.Headers.Set("route-key", calculateRouteKey(tenant));
            }, stoppingToken);
            _logger.LogInformation($"publishing application {app.Id} for {app.Tenant}");
        }
    }

    private string calculateRouteKey(string tenant)
    {
        if (_dedicatedQueueClients.Contains(tenant))
        {
            return tenant;
        }

        return "multitenant";
    }
}