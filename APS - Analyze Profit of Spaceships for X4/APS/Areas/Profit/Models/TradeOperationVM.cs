using System;

namespace APS.Areas.Profit.Models
{
    public sealed class TradeOperationVM
    {
        public double Time { get; set; }

        public int TimeRounded
        {
            get
            {
                TimeSpan span = TimeSpan.FromSeconds(Time);
                return (int)span.TotalHours;
            }
        }

        public string OurShipName { get; set; }
        public string OurShipId { get; set; }
        public string ItemSoldId { get; set; }
        public int Quantity { get; set; }
        public string SoldToName { get; set; }
        public string SoldToId { get; set; }
        public string Sector { get; set; }
        public string Faction { get; set; }
        public int Money { get; set; }
        public decimal PricePerItem => Money == 0 ? 0 : (Quantity == 0 ? 0 : (Money / 100) / Quantity);

        public decimal MarketAveragePrice { get; set; }

        public decimal EstimatedProfit =>
            //wut is this? //I am disconsidering Ware price when it's a primary product (stored on Solid or Liquid storage)
            //if ("Container".Equals(ItemSold.TransportType))
            //{
            //todo!! if (SoldToName == "Container")
            //todo!!    estimatedSoldPrice = ItemSold.MarketMinimumPrice;
            //}
            (PricePerItem - MarketAveragePrice) * Quantity;
    }
}