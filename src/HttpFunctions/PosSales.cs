using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Openhack.MS
{
    public class PosSales
    {
        private readonly ILogger _logger;
        private readonly ServiceBusSender _sender;

        public PosSales(ILoggerFactory loggerFactory, ServiceBusClient client)
        {
            _logger = loggerFactory.CreateLogger<PosSales>();
             _sender = client.CreateSender(Consts.ServiceBusTopic);
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
        public async Task Run([EventHubTrigger(Consts.EventHubName, Connection = Consts.EventHubConnectionStringSetting)] List<ReceivedEvent> input)
        {
            _logger.LogInformation($"First Event Hubs triggered message: {input[0]}");
            var messages =
              input.Where( i => !string.IsNullOrEmpty(i.header.receiptUrl))
                    .Select( i => 
                    {
                        var messageBody = 
                            JsonSerializer.Serialize( 
                                    new SalesMessage {
                                        totalItems = i.details.Length,
                                        totalCost = decimal.Parse(i.header.totalCost),
                                        salesNumber = i.header.salesNumber,
                                        salesDate = i.header.dateTime,
                                        storeLocation = i.header.locationId,
                                        receiptUrl = i.header.receiptUrl
                                });
                        var message = new ServiceBusMessage(messageBody);
                        message.ApplicationProperties.Add("totalCost",decimal.Parse(i.header.totalCost));
                        return message;
                    });
                    
            await _sender.SendMessagesAsync(messages);
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
