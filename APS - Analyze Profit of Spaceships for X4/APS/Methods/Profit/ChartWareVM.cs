namespace APS.Methods.Profit
{
    public class ChartWareVM
    {
        public string TimeRounded { get; internal set; }
        public double EstimatedProfit { get; set; }
        public string Ware { get; internal set; }
        public int Quantity { get; internal set; }
        public int Money { get; set; }
    }
}