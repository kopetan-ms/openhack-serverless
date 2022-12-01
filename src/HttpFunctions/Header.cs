using System;

namespace Openhack.MS
{
    public class Header
    {
        public string salesNumber { get; set; }
        public DateTime dateTime { get; set; }
        public string locationId { get; set; }
        public string locationName { get; set; }
        public string locationAddress { get; set; }
        public string locationPostcode { get; set; }
        public string totalCost { get; set; }
        public string totalTax { get; set; }
        public string receiptUrl { get; set; }
    }
}
