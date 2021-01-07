﻿using System;

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
        public string OurShipID { get; set; }
        public string ItemSoldId { get; set; }
        public string SoldToName { get; set; }
        public string SoldToId { get; set; }
        public int Quantity { get; set; }
        public string Faction { get; set; }
        public double InventorySpaceUsed { get; set; }
        public int Money { get; set; }
        public double PricePerItem { get { return (Money / Quantity); } }
        public double EstimatedProfit { get; set; }
    }
}