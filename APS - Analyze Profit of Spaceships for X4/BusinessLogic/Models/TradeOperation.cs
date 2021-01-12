using System;
using System.ComponentModel.DataAnnotations;

//HUUUUGE CREDIT TO magictrip https://github.com/magictripgames/X4MagicTripLogAnalyzer
namespace BusinessLogic.Models
{
    public class TradeOperation
    {
        public TradeOperation()
        {
        }

        public TradeOperation(string fullLogEntry)
        {
            // textexample="Magpie MCY-890 sold 350 Silicon to TEL Silicon Refinery I VUU-215 in Eighteen Billion for 52493 Cr."
            var sold = fullLogEntry.IndexOf(" sold ");
            var qty = sold + 6;
            if (sold == -1)
            {
                sold = fullLogEntry.IndexOf(" bought ");
                qty = sold + 8;
            }

            var shipname = fullLogEntry.LastIndexOf(' ', sold - 1, sold);
            var ware = fullLogEntry.IndexOf(' ', qty);
            var to = fullLogEntry.IndexOf(" to ", sold);
            var tofaction = fullLogEntry.IndexOf(' ', to + 4);
            var inlocation = fullLogEntry.IndexOf(" in ", to);
            var toid = fullLogEntry.LastIndexOf(' ', inlocation - 5, inlocation - tofaction);
            var formoney = fullLogEntry.IndexOf(" for ", inlocation);

            OurShipName = fullLogEntry.Substring(0, shipname);
            OurShipId = fullLogEntry.Substring(shipname + 1, sold - shipname).TrimEnd();
            Quantity = int.Parse(fullLogEntry.Substring(qty, ware - qty).Trim());
            ItemSoldId = fullLogEntry.Substring(ware + 1, to - ware - 1);
            Faction = fullLogEntry.Substring(to + 4, tofaction - to - 4);
            if (toid - tofaction - 1 < 1)
                SoldToName = Faction;
            else
                SoldToName = fullLogEntry.Substring(tofaction + 1, toid - tofaction - 1);
            SoldToId = fullLogEntry.Substring(toid + 1, inlocation - toid - 1);
            Sector = fullLogEntry.Substring(inlocation + 4, formoney - inlocation - 4);
        }

        [Key]
        public double Time { get; set; }

        public int TimeRounded
        {
            get
            {
                TimeSpan span = TimeSpan.FromSeconds(Time);
                return (int) span.TotalHours;
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
        public double PricePerItem => (Quantity == 0 ? 0 : Money / Quantity);

        public double EstimatedProfit
        {
            get
            {
                double estimatedSoldPrice = 0;
                //I am disconsidering Ware price when it's a primary product (stored on Solid or Liquid storage)
                //if ("Container".Equals(ItemSold.TransportType))
                //{
                //todo not sure what the OP intended with this:
                //if (SoldToName == "Container")
                //todo!!    estimatedSoldPrice = ItemSold.MarketMinimumPrice;
                //}

                return (PricePerItem - estimatedSoldPrice) * Quantity;
            }

        }
    }
}
