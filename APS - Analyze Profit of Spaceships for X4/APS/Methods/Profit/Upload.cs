using System;
using System.Linq;
using System.Security.Principal;
using APS.Areas.Profit.Models;
using APS.Model;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.Extensions.Logging;

namespace APS.Methods.Profit
{
    public static class Upload
    {
        public static DataSourceResult GetTradeOperation(string userName, DataSourceRequest request)
        {
            using (var db = new DBContext())
            {
                var result = from t in db.TradeOperations
                    join w in db.Wares on t.ItemSoldId equals w.WareID into joinedWares
                    from trade in joinedWares.DefaultIfEmpty()
                    select new TradeOperationVM()
                    {
                        Time = t.Time,
                        ItemSoldId = t.ItemSoldId,
                        Quantity = t.Quantity,
                        Money = t.Money,
                        Faction = t.Faction,
                        OurShipId = t.OurShipId,
                        SoldToName = t.SoldToName,
                        MarketAveragePrice = trade.MarketAveragePrice,
                        Sector = t.Sector,
                        SoldToId = t.SoldToId,
                        OurShipName = t.OurShipName
                    };
                return result.ToDataSourceResult(request);
            }
        }

        internal static void Save(string fileName, IPrincipal User, ILogger _logger)
        {
            using (var db = new DBContext())
            {
                if (!db.Configurations.Any())
                    db.Configurations.AddRange(BusinessLogic.Process.InitialConfigurations());
                
                if (!db.Wares.Any())
                    db.Wares.AddRange(BusinessLogic.Process.InitialWares());

                db.SaveChanges();

                var maxTrade = db.TradeOperations.Any() ? db.TradeOperations.Max(x => x.Time) : 0;

                var result = BusinessLogic.ImportLog.InputDialog(fileName,
                    db.Configurations.ToList(), maxTrade);

                var duplicates = result.GroupBy(x => x.Time)
                    .Where(x => x.Count(x => true) > 1).Select(x => x.Key);

                int step = 0;
                result.Where(x=> duplicates.Contains(x.Time)).Each(
                    x =>
                    {
                        x.Time = NextStepDouble(x.Time, step);
                        step++;
                    }
                );

                db.TradeOperations.AddRange(result.ToList());
                db.SaveChanges();
            }
        }

        private static double NextStepDouble(double value, int step)
        {
            if (step == 0)
                return value;
            long bits = BitConverter.DoubleToInt64Bits(value);
            if (value > 0)
                return BitConverter.Int64BitsToDouble(bits - step);
            else if (value < 0)
                return BitConverter.Int64BitsToDouble(bits + step);
            else
                return -double.Epsilon;
        }
    }
}