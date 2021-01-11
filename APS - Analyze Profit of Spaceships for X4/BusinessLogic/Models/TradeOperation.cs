using System;

//HUUUUGE CREDIT TO magictrip https://github.com/magictripgames/X4MagicTripLogAnalyzer
namespace BusinessLogic.Models
{
    public class TradeOperation
    {
        public TradeOperation()
        { //used in deserialisation todo remove when not using json files and use bd issue #1
        }

        public TradeOperation(string fullLogEntry)
        {
            //debug this.FullLogEntry = fullLogEntry;   
            // text="Magpie MCY-890 sold 350 Silicon to TEL Silicon Refinery I VUU-215 in Eighteen Billion for 52493 Cr."
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
                //todo!! if (SoldToName == "Container")
                //todo!!    estimatedSoldPrice = ItemSold.MarketMinimumPrice;
                //}

                return (PricePerItem - estimatedSoldPrice) * Quantity;
            }

        }
    }
}

/////OLD CODE BELOW 10 minutes processing time (now down to 10 seconds)
    //    }
    //public static class TradeOperations {

    //    public static string getDestinationID(string logEntry, Process p)
    //    {
    //        int position;
    //        string configEntry = p.Configurations.Where(x => x.Key.Equals("InTranslation")).FirstOrDefault().Value;
    //        configEntry = " " + configEntry.Trim() + " ";
    //        if (logEntry.Contains(configEntry))
    //        {
    //            position = logEntry.IndexOf(configEntry, 0);
    //            return logEntry.Substring(position - 7, 7);
    //        }
    //        else
    //        {
    //            return "";
    //        }
    //    }

    //    public static string getSoldToName(string logEntry, string soldToId, Process p)
    //    {
    //        string configEntryIn = p.Configurations.Where(x => x.Key.Equals("InTranslation")).FirstOrDefault().Value;
    //        configEntryIn = " " + configEntryIn.Trim() + " ";
    //        string configEntryTo = p.Configurations.Where(x => x.Key.Equals("ToTranslation")).FirstOrDefault().Value;
    //        configEntryTo = " " + configEntryTo.Trim() + " ";
    //        int numberToSum = configEntryTo.Length;
    //        if (logEntry.Contains("bought"))
    //        {
    //            //configEntryIn = " bought ";
    //            configEntryTo = " from ";
    //            numberToSum = 6;
    //        }
    //        int endPosition, startPosition;
    //        if (logEntry.Contains(configEntryIn))
    //        {
    //            endPosition = logEntry.IndexOf(soldToId, 0) - 1;
    //            startPosition = logEntry.IndexOf(configEntryTo, 0) + numberToSum;
    //            return logEntry.Substring(startPosition, endPosition - startPosition);
    //        }
    //        else
    //        {
    //            return "";
    //        }
    //    }

    //    public static string getShipID(string logEntry, Process p)
    //    {
    //        string configEntry = p.Configurations.Where(x => x.Key.Equals("SoldTranslation")).FirstOrDefault().Value;
    //        configEntry = " " + configEntry.Trim() + " ";
    //        if (logEntry.Contains("bought"))
    //        {
    //            configEntry = " bought ";
    //        }
    //        int position;
    //        if (logEntry.Contains(configEntry))
    //        {
    //            position = logEntry.IndexOf(configEntry, 0);
    //            return logEntry.Substring(position - 7, 7);
    //        }
    //        return "";
    //    }

    //    public static string getShipName(string logEntry, string shipID, Process p)
    //    {
    //        string configEntry = p.Configurations.Where(x => x.Key.Equals("SoldTranslation")).FirstOrDefault().Value;
    //        configEntry = " " + configEntry.Trim() + " ";
    //        if (logEntry.Contains("bought"))
    //        {
    //            configEntry = " bought ";
    //        }
    //        int position;
    //        if (logEntry.Contains(configEntry))
    //        {
    //            position = logEntry.IndexOf(shipID, 0);
    //            return logEntry.Substring(0, position - 1);
    //        }
    //        return "";
    //    }

    //    public static int getQtdSold(string logEntry, Process p)
    //    {
    //        string configEntry = p.Configurations.Where(x => x.Key.Equals("SoldTranslation")).FirstOrDefault().Value;
    //        configEntry = " " + configEntry.Trim() + " ";
    //        int numberToSum = configEntry.Length;
    //        if (logEntry.Contains("bought"))
    //        {
    //            configEntry = " bought ";
    //            numberToSum = 8;
    //        }
    //        int position, positionEndQtd;
    //        if (logEntry.Contains(configEntry))
    //        {
    //            position = logEntry.IndexOf(configEntry, 0) + numberToSum;
    //            positionEndQtd = logEntry.IndexOf(" ", position);
    //            return int.Parse(logEntry.Substring(position, positionEndQtd - position));
    //        }
    //        return 0;
    //    }

    //    public static string getProduct(string logEntry, int qtdSold, Process p)
    //    {
    //        string configEntrySold = p.Configurations.Where(x => x.Key.Equals("SoldTranslation")).FirstOrDefault().Value;
    //        configEntrySold = " " + configEntrySold.Trim() + " ";
    //        string configEntryTo = p.Configurations.Where(x => x.Key.Equals("ToTranslation")).FirstOrDefault().Value;
    //        configEntryTo = " " + configEntryTo.Trim() + " ";
    //        int numberToSum = configEntrySold.Length;
    //        if (logEntry.Contains("bought"))
    //        {
    //            configEntrySold = " bought ";
    //            configEntryTo = " from ";
    //            numberToSum = 8;
    //        }
    //        int position, positionStartProduct;
    //        if (logEntry.Contains(configEntryTo))
    //        {
    //            positionStartProduct = logEntry.IndexOf((configEntrySold), 0) + numberToSum + qtdSold.ToString().Length + 1;
    //            position = logEntry.IndexOf(configEntryTo, positionStartProduct);
    //            string wareName = logEntry.Substring(positionStartProduct, position - positionStartProduct);
    //            if (wareName.Contains("Cr)"))
    //            {
    //                wareName = wareName.Substring(0, wareName.IndexOf("(")).Trim();
    //            }
    //            return wareName;
    //        }
    //        else
    //        {
    //            return "";
    //        }
    //    }


    


