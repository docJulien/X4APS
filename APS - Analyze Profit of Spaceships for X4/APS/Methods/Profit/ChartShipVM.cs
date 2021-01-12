namespace APS.Methods.Profit
{
    public class ChartShipVM
    {
        public string TimeRounded { get; internal set; }
        public double EstimatedProfit { get; set; }
        public string Ship { get; internal set; }
        public int Quantity { get; internal set; }
        public int Money { get; set; }
    }
}