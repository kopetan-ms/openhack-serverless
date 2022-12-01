using System;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Openhack.MS
{
    public class RegularReceipts
    {
        private readonly ILogger _logger;
        private readonly BlobContainerClient _container;

        public RegularReceipts(ILoggerFactory loggerFactory, BlobContainerClient container)
        {
            _logger = loggerFactory.CreateLogger<RegularReceipts>();
            _container = container;
        }

        [Function("RegularReceipts")]
        public async Task Run([ServiceBusTrigger(Consts.ServiceBusTopic, Consts.RegularSubscription, Connection = Consts.SerbiceBusConnectionStringSetting)] SalesMessage mySbMsg)
        {
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message with total items: {mySbMsg.totalItems}");

            var result = new RegularBlob
            {
                Items = mySbMsg.totalItems,
                TotalCost = mySbMsg.totalCost,
                SalesNumber = mySbMsg.salesNumber,
                SalesDate = mySbMsg.salesDate,
                Store = mySbMsg.storeLocation
            };

            var serializedPayload = JsonSerializer.Serialize(result);
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(serializedPayload));
            
            string blobName = "sample-blob";
            var blob = _container.GetBlobClient(blobName);
            await blob.UploadAsync(stream);
        }

    }

    public class RegularBlob
    {
        public int Items { get; set; }
        public decimal TotalCost { get; set; }
        public string SalesNumber { get; set; }
        public DateTime SalesDate { get; set; }
        public string Store { get; set; }
    }
}
