using System.Collections.Generic;
using System.Linq;
using APS.Areas.Profit.Models;
using APS.Methods.Common;
using APS.Model;
using BusinessLogic.Models;
using Kendo.Mvc.UI;

namespace APS.Methods.Profit
{
    public class Chart
    {
        internal static IEnumerable<ChartShipVM> ReadShip(string userName)
        {
            using (var db = new DBContext())
            {
                var result = db.TradeOperations.ToList()
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
                return result.ToList();
            }
        }

        internal static IEnumerable<ChartWareVM> GetTradeOperation(string userName)
        {
            using (var db = new DBContext())
            {
                var result = db.TradeOperations.ToList() //todo use iqueryable db entities but I'm getting an error here...
                    .GroupBy(x =>
                        x.TimeRounded
                    )
                    .Select(x => new ChartWareVM
                    {
                        TimeRounded = x.Key.ToString("D3"),
                        //Ware = x.Key.Ware,
                        EstimatedProfit = x.Sum(s => s.EstimatedProfit),
                        Quantity = x.Sum(s => s.Quantity),
                        Money = x.Sum(s => s.Money)
                    });

                return result.ToList();
            }
        }
    }
}
