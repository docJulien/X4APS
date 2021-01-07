using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BusinessLogic.Models;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace BusinessLogic
{
    public class Process
    {
        public class Configuration
        {
            public string Key { get; set; }
            public string Value { get; set; }

        }
        
        public  List<TradeOperation> GlobalTradeOperations = new List<TradeOperation>();
        public  List<Ware> GlobalWares = new List<Ware>();
        //idk why we need this todo public  List<Ship> ShipsWithTradeOperations = new List<Ship>();
        // ?? todo what public  List<Ship> DestinationsWithTradeOperations = new List<Ship>();
        public List<Configuration> Configurations = new List<Configuration>();

        public Process()
        {
            var fileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Remove(0, 6) + @"\X4LogAnalyzerTempXML.json";

            DeserializeConfigurations();
            DeserializeWares();
            DeserializeTradeOperations(fileName);
        }

        private void DeserializeTradeOperations(string saveFile)
        {
            if (File.Exists(saveFile))
            {
                using (StreamReader r = new StreamReader(saveFile))
                {
                    var json = r.ReadToEnd();
                    GlobalTradeOperations = JsonConvert.DeserializeObject<List<TradeOperation>>(json);
                    //foreach (TradeOperation tradeOp in GlobalTradeOperations)
                    //{
                    //    AddTradeOperationToShipList(tradeOp);
                    //    AddTradeOperationToWareList(tradeOp);
                    //    //tradeOp.OurShip.AddTradeOperation(tradeOp);
                    //}
                }
            }
            Console.WriteLine(GlobalTradeOperations.Count());
        }



        public void DeserializeConfigurations()
        {
            var applicationPath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            var directory = System.IO.Path.GetDirectoryName(applicationPath).Remove(0, 6);
            string configurationsFileName = directory + @"\Configurations.json";
            using (StreamReader r = new StreamReader(configurationsFileName))
            {
                string json = r.ReadToEnd();
                Configurations = JsonConvert.DeserializeObject<List<Configuration>>(json);
                if (Configurations.Count == 0)
                {
                    Configurations.Add(new Configuration() { Key = "LastSaveGameLoaded", Value = @" %userprofile%\Documents\Egosoft\X4\[REPLACE_BY_USERID]\save\quicksave.xml.gz" });
                }
                //GlobalWares = JsonConvert.DeserializeObject<List<Ware>>(json);
                Console.WriteLine(Configurations.Count());
            }
        }

        public void SaveConfigurations()
        {
            string applicationPath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            var directory = System.IO.Path.GetDirectoryName(applicationPath).Remove(0, 6);
            using (StreamWriter file = File.CreateText(directory + @"\Configurations.json"))
            {
               JsonSerializer serializer = new JsonSerializer();
                if (Configurations.FirstOrDefault(x => x.Key.Equals("LastSaveGameLoaded")) == null)
                {
                    Configurations.Add(new Configuration() { Key = "LastSaveGameLoaded", Value = @"%userprofile%\Documents\Egosoft\X4\[REPLACE_BY_USERID]\save\quicksave.xml.gz" });
                }
                if (Configurations.FirstOrDefault(x => x.Key.Equals("SoldTranslation")) == null)
                {
                    Configurations.Add(new Configuration() { Key = "SoldTranslation", Value = @"sold" });
                }
                if (Configurations.FirstOrDefault(x => x.Key.Equals("TradeCompletedTranslation")) == null)
                {
                    Configurations.Add(new Configuration() { Key = "TradeCompletedTranslation", Value = @"Trade Completed" });
                }
                if (Configurations.FirstOrDefault(x => x.Key.Equals("InTranslation")) == null)
                {
                    Configurations.Add(new Configuration() { Key = "InTranslation", Value = @"in" });
                }
                if (Configurations.FirstOrDefault(x => x.Key.Equals("ToTranslation")) == null)
                {
                    Configurations.Add(new Configuration() { Key = "ToTranslation", Value = @"to" });
                }
                
                //serialize object directly into file stream
                serializer.Serialize(file, Configurations);
                file.Close();
            }
        }

        public void DeserializeWares()
        {
            string applicationPath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            var directory = System.IO.Path.GetDirectoryName(applicationPath).Remove(0, 6);
            List<Ware> TempWares = new List<Ware>();
            string waresFileName = directory + @"\Wares.json";
            using (StreamReader r = new StreamReader(waresFileName))
            {
                string json = r.ReadToEnd();
                TempWares = JsonConvert.DeserializeObject<List<Ware>>(json);
                Console.WriteLine(TempWares.Count());
            }
            foreach (Ware ware in TempWares)
            {
                Ware globalWare = GlobalWares.FirstOrDefault(x => x.Name.Equals(ware.Name));
                if (globalWare == null)
                {
                    GlobalWares.Add(ware);
                }
            }
            //GlobalWares = TempWares;
        }

        //public void AddTradeOperationToShipList(TradeOperation tradeOp)
        //{
        //    Ship ship = ShipsWithTradeOperations.FirstOrDefault(x => x.FullShipname.Equals(tradeOp.OurShip.FullShipname));
        //    if (ship == null)
        //    {
        //        ship = tradeOp.OurShip;
        //        ShipsWithTradeOperations.Add(ship);
        //    }
        //    tradeOp.PartialSumByShip = ship.GetListOfTradeOperations().Sum(x => x.Money) + tradeOp.Money;
        //    ship.AddTradeOperation(tradeOp);
            
        //}

        //public void AddTradeOperationToWareList(TradeOperation tradeOp)
        //{
        //    Ware ware = GlobalWares.FirstOrDefault(x => x.Name.Equals(tradeOp.ItemSold.Name));
        //    //if (ware == null)
        //    //{
        //    //    ware = tradeOp.ItemSold;
        //    //    ShipsWithTradeOperations.Add(ship);
        //    //}
        //    ware.AddTradeOperation(tradeOp);
        //}
    }
}
