namespace APS.Methods.Profit
{
    public class ChartWareVM
    {
        public string TimeRounded { get; internal set; }
        public decimal EstimatedProfit { get; set; }
        public string Ware { get; internal set; }
        public int Quantity { get; internal set; }
        public int Money { get; set; }
    }

    public class ShipSummaryVM
    { 
        public int LastActivity { get; internal set; }
        public string ItemSoldId { get; internal set; }
        public string Sector { get; set; }
        public string OurShipId { get; set; }
        public decimal EstimatedProfit { get; set; }
        public int Quantity { get; internal set; }
    }
}