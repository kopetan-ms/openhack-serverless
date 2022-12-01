using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Openhack.MS
{
    public class PosSales
    {
        private readonly ILogger _logger;

        public PosSales(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PosSales>();
        }

        // Challenge 7
        // [Function("PosSales")]
        // [CosmosDBOutput(Consts.CosmosDBDatabase, Consts.CosmosDBPoSCollection, ConnectionStringSetting = Consts.CosmosConnectionStringSetting, CreateIfNotExists = true)]
        // public IReadOnlyList<Detail> Run([EventHubTrigger(Consts.EventHubName, Connection = Consts.EventHubConnectionStringSetting)] List<ReceivedEvent> input)
        // {
        //     _logger.LogInformation($"First Event Hubs triggered message: {input[0]}");
            
        //     return input.SelectMany(a=> a.details).ToList().AsReadOnly();
        // }
        
        [Function("PosSales")]
        [ServiceBusOutput(Consts.ServiceBusTopic, Connection = Consts.SerbiceBusConnectionStringSetting, EntityType = EntityType.Topic)]
        public IReadOnlyList<string> Run([EventHubTrigger(Consts.EventHubName, Connection = Consts.EventHubConnectionStringSetting)] List<ReceivedEvent> input)
        {
            _logger.LogInformation($"First Event Hubs triggered message: {input[0]}");
            
            return new List<string> { "Hello", "World" }.AsReadOnly();
        }
    }

    public class SalesMessage
    {
        public int totalItems { get; set; }
        public decimal totalCost { get; set; }
        public string salesNumber { get; set; }
        public DateTime salesDate { get; set; }
        public string storeLocation { get; set; }
        public string receiptUrl { get; set; }
    }
}
