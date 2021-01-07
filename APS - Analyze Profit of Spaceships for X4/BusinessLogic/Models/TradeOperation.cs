using System.Linq;
using System.Xml;

//HUUUUGE CREDIT TO magictrip https://github.com/magictripgames/X4MagicTripLogAnalyzer
namespace BusinessLogic.Models
{
    public class TradeOperation
    {
        public double Time { get; set; }
        public Ship OurShip { get; set; }
        public string OurShipName => OurShip.ShipName;
        public string OurShipId => OurShip.ShipID;

        public string FullLogEntry { get; set; }
        public Ware ItemSold { get; set; }
        public string ItemSoldId => ItemSold.WareID;
        public int Quantity { get; set; }
        public Ship SoldTo { get; set; }
        public string SoldToName => SoldTo.ShipName;
        public string SoldToId => SoldTo.ShipID;
        public string Faction { get; set; }
        public int Money { get; set; }
        public double PricePerItem => (Money / Quantity);

        public double EstimatedProfit
        {
            get
            {
                double estimatedSoldPrice = 0;
                //I am disconsidering Ware price when it's a primary product (stored on Solid or Liquid storage)
                if ("Container".Equals(ItemSold.TransportType))
                {
                    estimatedSoldPrice = ItemSold.MarketMinimumPrice;
                }
                return (PricePerItem - estimatedSoldPrice) * Quantity;
            }
        }

        public double PartialSumByShip { get; set; }
        public double PartialSumByWare { get; set; }

        //todo dead code remove?
        //public TradeOperation(double time, string shipId, string fullLogEntry, string shipName, int quantity, string product, string soldToId, string soldToName, string faction, int money)
        //{
        //    if ("Please load the XML".Equals(shipId))
        //    {
        //        return;
        //    }
        //    this.Time = time;
        //    Ship ourShip = Ship.GetShip(shipId);
        //    //this is to ensure the last name of the ship sticks
        //    OurShip.ShipName = shipName;

        //    this.OurShip = ourShip;
        //    this.FullLogEntry = fullLogEntry;
        //    this.Quantity = quantity;

        //    Ware itemSold = Ware.GetWare(product);
        //    this.ItemSold = itemSold;

        //    this.SoldTo = Ship.GetSoldTo(soldToId);
        //    this.SoldTo.ShipName = soldToName;
        //    this.Faction = faction;
        //    this.Money = money;
        //    ourShip.AddTradeOperation(this);
        //    PartialSumByShip = OurShip.GetListOfTradeOperations().Sum(x => x.Money);
        //    PartialSumByWare = 0; 


        //    itemSold.AddTradeOperation(this);
        //}

        //public TradeOperation()
        //{
        //    //This is required for the desserialization process
        //}

        public TradeOperation(double time)
        {
            this.Time = time;
        }

        public void WriteToLog()
        {
            //Console.WriteLine("\t{");
            //Console.WriteLine(string.Format("\t   {0} : {1}", "Time", this.Time));
            //Console.WriteLine(string.Format("\t   {0} : {1}", "ShipId", this.OurShip.ShipID));
            //Console.WriteLine(string.Format("\t   {0} : {1}", "ShipName", this.OurShip.ShipName));
            //Console.WriteLine(string.Format("\t   {0} : {1}", "Quantity", this.Quantity));
            //Console.WriteLine(string.Format("\t   {0} : {1}", "Product", this.ItemSold.Name));
            //Console.WriteLine(string.Format("\t   {0} : {1}", "SoldToID", this.SoldTo.ShipID));
            //Console.WriteLine(string.Format("\t   {0} : {1}", "SoldToName", this.SoldTo.ShipName));
            //Console.WriteLine(string.Format("\t   {0} : {1}", "Faction", this.Faction));
            //Console.WriteLine(string.Format("\t   {0} : {1}", "Money", this.Money));
            //Console.WriteLine(string.Format("\t   {0} : {1}", "FullLogEntry", this.FullLogEntry));
            //Console.WriteLine("\t}");
        }

        public void ParseTextEntry(XmlReader logEntry, Process p)
        {
            this.FullLogEntry = logEntry.Value;
            this.OurShip = Ship.GetShip(getShipID(logEntry.Value, p));
            this.OurShip.ShipName = getShipName(logEntry.Value, this.OurShip.ShipID, p);
            this.Quantity = getQtdSold(logEntry.Value, p);
            this.ItemSold = Ware.GetWare(getProduct(logEntry.Value, this.Quantity, p));
            this.SoldTo = Ship.GetSoldTo(getDestinationID(logEntry.Value, p));
            this.SoldTo.ShipName = getSoldToName(logEntry.Value, this.SoldTo.ShipID, p);
            
        }

        private static string getDestinationID(string logEntry, Process p)
        {
            int position;
            string configEntry = p.Configurations.Where(x => x.Key.Equals("InTranslation")).FirstOrDefault().Value;
            configEntry = " " + configEntry.Trim() + " ";
            if (logEntry.Contains(configEntry))
            {
                position = logEntry.IndexOf(configEntry, 0);
                return logEntry.Substring(position - 7, 7);
            }
            else
            {
                return "";
            }
        }

        private static string getSoldToName(string logEntry, string soldToId, Process p)
        {
            string configEntryIn = p.Configurations.Where(x => x.Key.Equals("InTranslation")).FirstOrDefault().Value;
            configEntryIn = " " + configEntryIn.Trim() + " ";
            string configEntryTo = p.Configurations.Where(x => x.Key.Equals("ToTranslation")).FirstOrDefault().Value;
            configEntryTo = " " + configEntryTo.Trim() + " ";
            int numberToSum = configEntryTo.Length;
            if (logEntry.Contains("bought"))
            {
                //configEntryIn = " bought ";
                configEntryTo = " from ";
                numberToSum = 6;
            }
            int endPosition, startPosition;
            if (logEntry.Contains(configEntryIn))
            {
                endPosition = logEntry.IndexOf(soldToId, 0) - 1;
                startPosition = logEntry.IndexOf(configEntryTo, 0) + numberToSum;
                return logEntry.Substring(startPosition, endPosition - startPosition);
            }
            else
            {
                return "";
            }
        }

        private static string getShipID(string logEntry, Process p)
        {
            string configEntry = p.Configurations.Where(x => x.Key.Equals("SoldTranslation")).FirstOrDefault().Value;
            configEntry = " " + configEntry.Trim() + " ";
            if (logEntry.Contains("bought"))
            {
                configEntry = " bought ";
            }
            int position;
            if (logEntry.Contains(configEntry))
            {
                position = logEntry.IndexOf(configEntry, 0);
                return logEntry.Substring(position - 7, 7);
            }
            return "";
        }

        private static string getShipName(string logEntry, string shipID, Process p)
        {
            string configEntry = p.Configurations.Where(x => x.Key.Equals("SoldTranslation")).FirstOrDefault().Value;
            configEntry = " " + configEntry.Trim() + " ";
            if (logEntry.Contains("bought"))
            {
                configEntry = " bought ";
            }
            int position;
            if (logEntry.Contains(configEntry))
            {
                position = logEntry.IndexOf(shipID, 0);
                return logEntry.Substring(0, position - 1);
            }
            return "";
        }

        private static int getQtdSold(string logEntry, Process p)
        {
            string configEntry = p.Configurations.Where(x => x.Key.Equals("SoldTranslation")).FirstOrDefault().Value;
            configEntry = " " + configEntry.Trim() + " ";
            int numberToSum = configEntry.Length;
            if (logEntry.Contains("bought"))
            {
                configEntry = " bought ";
                numberToSum = 8;
            }
            int position, positionEndQtd;
            if (logEntry.Contains(configEntry))
            {
                position = logEntry.IndexOf(configEntry, 0) + numberToSum;
                positionEndQtd = logEntry.IndexOf(" ", position);
                return int.Parse(logEntry.Substring(position, positionEndQtd - position));
            }
            return 0;
        }

        private static string getProduct(string logEntry, int qtdSold, Process p)
        {
            string configEntrySold = p.Configurations.Where(x => x.Key.Equals("SoldTranslation")).FirstOrDefault().Value;
            configEntrySold = " " + configEntrySold.Trim() + " ";
            string configEntryTo = p.Configurations.Where(x => x.Key.Equals("ToTranslation")).FirstOrDefault().Value;
            configEntryTo = " " + configEntryTo.Trim() + " ";
            int numberToSum = configEntrySold.Length;
            if (logEntry.Contains("bought"))
            {
                configEntrySold = " bought ";
                configEntryTo = " from ";
                numberToSum = 8;
            }
            int position, positionStartProduct;
            if (logEntry.Contains(configEntryTo))
            {
                positionStartProduct = logEntry.IndexOf((configEntrySold), 0) + numberToSum + qtdSold.ToString().Length + 1;
                position = logEntry.IndexOf(configEntryTo, positionStartProduct);
                string wareName = logEntry.Substring(positionStartProduct, position - positionStartProduct);
                if (wareName.Contains("Cr)"))
                {
                    wareName = wareName.Substring(0, wareName.IndexOf("(")).Trim();
                }
                return wareName;
            }
            else
            {
                return "";
            }
        }


    }
}


