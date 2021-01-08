using System.Collections.Generic;
using System.Linq;
using APS.Areas.Profit.Models;
using APS.Methods.QueriesExtensions;

namespace APS.Methods.Profit
{
    public class Chart
    {
        internal static IEnumerable<ChartShipVM> ReadShip(string userName)
        {
            var p = new BusinessLogic.Process();
            var result = p.GlobalTradeOperations
                .GroupBy(g => new {
                    TimeRounded = g.TimeRounded,
                    Ship = g.OurShipId
                })
                .Select(x => new ChartShipVM
                {
                    TimeRounded = x.Key.TimeRounded.ToString("D3"),
                    Ship = x.Key.Ship,
                    EstimatedProfit = x.Sum(s => s.EstimatedProfit),
                    Quantity = x.Sum(s => s.Quantity),
                    Money = x.Sum(s => s.Money)
                }).OrderBy(x=>x.TimeRounded);
            return result;
        }

        internal static IEnumerable<ChartWareVM> GetTradeOperation(string userName)
        {
            var p = new BusinessLogic.Process();
            return p.GlobalTradeOperations
                .GroupBy(x => new { x.TimeRounded //, Ware = x.ItemSold.Name
                                                  })
                .Select(x=>new ChartWareVM
                {
                    TimeRounded = x.Key.TimeRounded.ToString("D3"),
                    //Ware = x.Key.Ware,
                    EstimatedProfit = x.Sum(s=>s.EstimatedProfit),
                    Quantity = x.Sum(s=>s.Quantity),
                    Money = x.Sum(s=>s.Money)
                });


            //ex in db: (todo)
            //return CommonMethods.GetQuery<UploadData>()
            //    .Where(x => sports.Contains(x.ActivityType) && x.User == userName)
            //    .GroupBy(g => new { g.ActivityType, g.Date.Year, g.Date.Month })
            //    .Select(x => new ReportDataVM
            //    {
            //        ActivityType = x.Key.ActivityType,
            //        Year = x.Key.Year,
            //        Month = x.Key.Month,
            //        trading = x.Sum(s => s.ActivityType == trading.ToString() ? s.Distance / 1000 : 0),
            //        killing = x.Sum(s => s.ActivityType == killing.ToString() ? s.Distance / 1000 : 0),
            //        etc
            //    })
            //    .ToList();
        }
    }

    public class ChartWareVM
    {
        public string TimeRounded { get; internal set; }
        public double EstimatedProfit { get; set; }
        public string Ware { get; internal set; }
        public int Quantity { get; internal set; }
        public int Money { get; set; }
    }
    public class ChartShipVM
    {
        public string TimeRounded { get; internal set; }
        public double EstimatedProfit { get; set; }
        public string Ship { get; internal set; }
        public int Quantity { get; internal set; }
        public int Money { get; set; }
    }
}
