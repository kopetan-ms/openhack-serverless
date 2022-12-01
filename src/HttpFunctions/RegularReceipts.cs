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
        [BlobOutput($"{Consts.BlobStoragePath}/{rund-guid}-output.txt")]
        public RegularBlob Run([ServiceBusTrigger(Consts.ServiceBusTopic, Consts.RegularSubscription, Connection = Consts.SerbiceBusConnectionStringSetting)] SalesMessage mySbMsg)
        {
            _logger.LogInformation($"C# ServiceBus topic trigger function processed message with total items: {mySbMsg.totalItems}");

            var blob = new RegularBlob();

            blob.Items = mySBMsg.totalItems;
            blob.TotalCost = mySBMsg.totalCost;
            blob.SalesNumber = mySBMsg.salesNumber;
            blob.SalesDate = mySBMsg.salesDate;
            blob.Store = mySBMsg.storeLocation;

            return blob;
        }

    }

    public class RegularBlob
    {
        public int Items { get; set; }
        public string TotalCost { get; set; }
        public string SalesNumber { get; set; }
        public string SalesDate { get; set; }
        public string Store { get; set; }
    }
}
