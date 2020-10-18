using System;
using System.Collections.Generic;

namespace DeveloperCase.Models
{
    public partial class Sales
    {
        public double OrderId { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string ItemType { get; set; }
        public string SalesChannel { get; set; }
        public string OrderPriority { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? ShipDate { get; set; }
        public double? UnitSold { get; set; }
        public double? UnitPrice { get; set; }
        public double? UnitCost { get; set; }
        public double? TotalRevenue { get; set; }
        public double? TotalCost { get; set; }
        public double? TotalProfit { get; set; }
    }
}
