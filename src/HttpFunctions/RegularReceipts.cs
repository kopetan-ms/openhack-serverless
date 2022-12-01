using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Openhack.MS
{
    public class RegularReceipts
    {
        private readonly ILogger _logger;

        public RegularReceipts(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RegularReceipts>();
        }

        [Function("RegularReceipts")]
       public void Run([ServiceBusTrigger(Consts.ServiceBusTopic, Consts.RegularSubscription, Connection = Consts.SerbiceBusConnectionStringSetting)] string mySbMsg)
        {
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
        }
    }
}
