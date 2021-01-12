using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BusinessLogic.Models;
using Newtonsoft.Json;

namespace BusinessLogic
{
    public static class Process
    {
        public static List<Configuration> InitialConfigurations()
        {
            var Configurations = new List<Configuration>();
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

            return Configurations;
            
        }

        public static List<Ware> InitialWares()
        { 
            string applicationPath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            var directory = Path.GetDirectoryName(applicationPath).Remove(0, 6);
            List<Ware> TempWares = new List<Ware>();
            string waresFileName = directory + @"\Wares.json";
            using (StreamReader r = new StreamReader(waresFileName))
            {
                string json = r.ReadToEnd();
                TempWares = JsonConvert.DeserializeObject<List<Ware>>(json);
                Console.WriteLine(TempWares.Count());
            }

            return TempWares;
        }
    }
}
