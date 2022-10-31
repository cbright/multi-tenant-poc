
using Contracts;
using System.Threading.Tasks;
using MassTransit;

class SubmitApplicationConsumer :
    IConsumer<SubmitApplication>
{
    ILogger<SubmitApplicationConsumer> _logger;

    public SubmitApplicationConsumer(ILogger<SubmitApplicationConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SubmitApplication> context)
    {

        _logger.LogInformation($"Processing application {context.Message.Id} from tenant {context.Message.Tenant}.");

        await Task.CompletedTask;
 
    }
}