using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Openhack.MS
{
    public class PriorityReceips
    {
        private readonly ILogger _logger;

        public PriorityReceips(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PriorityReceips>();
        }

        [Function("PriorityReceips")]
        public void Run([ServiceBusTrigger(Consts.ServiceBusTopic, Consts.PrioritySubscription, Connection = Consts.SerbiceBusConnectionStringSetting)] string mySbMsg)
        {
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
        }
    }
}
