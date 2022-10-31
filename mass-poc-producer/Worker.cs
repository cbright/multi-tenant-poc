using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Contracts;
using Microsoft.Extensions.Logging;

public class Worker : BackgroundService
{
    readonly IBus _bus;
    readonly ILogger<Worker> _logger;

    public Worker(IBus bus, ILogger<Worker> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tenants = new List<string>{
            "ohbprgfh",
            "cpoyaqru",
            "ajnabrsm",
            "qnaagyep",
            "fterttgd",
            "ojnqoaam",
            "ejafmupe",
            "ucprbgog"
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
            await _bus.Publish(app, stoppingToken);
            _logger.LogInformation($"publishing application {app.Id} for {app.Tenant}");
        }
    }
}