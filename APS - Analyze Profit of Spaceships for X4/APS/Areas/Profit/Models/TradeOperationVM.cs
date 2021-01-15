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

        public decimal WareMarketAveragePrice { get; set; }
        public bool WareTransportTypeContainer { get; set; }

        public decimal EstimatedProfit =>
            CorrectedMoney - (WareTransportTypeContainer ? WareMarketAveragePrice * Quantity : 0);
        public int CorrectedMoney =>
            Money / 100 ;
    }
}