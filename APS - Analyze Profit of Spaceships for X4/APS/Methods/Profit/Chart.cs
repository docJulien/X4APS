using System;
using System.Collections.Generic;
using System.Linq;
using APS.Areas.Profit.Models;
using APS.Model;
using BusinessLogic.Models;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using DBContext = APS.Model.DBContext;

namespace APS.Methods.Profit
{
    public class Chart
    {
        internal static IEnumerable<ChartShipVM> ReadShip(string userName)
        {
            using (var db = new DBContext())
            {
                var result = (from t in db.TradeOperations
                        join w in db.Wares on t.ItemSoldId equals w.WareID into joinedWares
                        from trade in joinedWares.DefaultIfEmpty()
                        select new TradeOperationVM()
                        {
                            Time = t.Time,
                            ItemSoldId = t.ItemSoldId,
                            Quantity = t.Quantity,
                            Money = t.Money,
                            OurShipId = t.OurShipId,
                            OurShipName = t.OurShipName,
                            WareMarketAveragePrice = trade.MarketAveragePrice,
                            WareTransportTypeContainer = trade.TransportType == "Container"
                        }).ToList() //todo use iqueryable until the end for perf
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
                    Money = x.Sum(s => s.CorrectedMoney)
                }).OrderBy(x=>x.TimeRounded);
                return result.ToList();
            }
        }

        internal static IEnumerable<ChartWareVM> GetTradeOperation(string userName)
        {
            using var db = new DBContext();
            var result = (from t in db.TradeOperations
                    join w in db.Wares on t.ItemSoldId equals w.WareID into joinedWares
                    from trade in joinedWares.DefaultIfEmpty()
                    select new TradeOperationVM()
                    {
                        Time = t.Time,
                        ItemSoldId = t.ItemSoldId,
                        Quantity = t.Quantity,
                        Money = t.Money,
                        WareMarketAveragePrice = trade.MarketAveragePrice,
                        WareTransportTypeContainer = trade.TransportType == "Container"
                    }).ToList() //todo use iqueryable until the end for perf
                .GroupBy(x =>
                    x.TimeRounded
                )
                .Select(x => new ChartWareVM
                {
                    TimeRounded = x.Key.ToString("D3"),
                    //Ware = x.Key.Ware,
                    EstimatedProfit = x.Sum(s => s.EstimatedProfit),
                    Quantity = x.Sum(s => s.Quantity),
                    Money = x.Sum(s => s.CorrectedMoney)
                });

            return result.ToList();
        }

        internal static void UpdateShip(ShipSummaryVM model)
        {
            using var db = new DBContext();
            Ship s;
            s = db.Ships.Single(u => u.ShipID == model.OurShipId);

            s.Actif = model.Actif;
            db.SaveChanges();
        }

        internal static DataSourceResult GetWorstShips([DataSourceRequest] DataSourceRequest request)
        {
            using (var db = new DBContext())
            {
                List<FilterDescriptor> filters = GetFilters(request.Filters).Select(x=>(FilterDescriptor)x).ToList();

                var filterLastActivity =   int.Parse(filters
                                                      .FirstOrDefault(x =>
                                                          (x).Member == "LastActivity")?.Value
                                                  .ToString()
                                                      ?.Trim() ?? "2"); //DEFAULT 2 HOURS
                var lastActivity = db.TradeOperations.Max(x => x.Time);

                var wares = db.Wares.Select(x => x);
                var source = db.TradeOperations.Where(x =>
                    x.Time >= lastActivity - filterLastActivity * 60*60 && x.Money != 0 && x.Quantity != 0);
                var shipList = db.Ships;

                var filterWare = filters
                        .FirstOrDefault(x => ( x).Member == "ItemSoldId")?.Value.ToString() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(filterWare))
                    source = source.Where(x => x.ItemSoldId == filterWare);
                var filterSector = filters.FirstOrDefault(x => (x).Member == "Sector")
                    ?.Value.ToString() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(filterSector))
                    source = source.Where(x => x.Sector == filterSector);

                return (from allShips in shipList
                        join worstShips in source
                            on allShips.ShipID equals worstShips.OurShipId into joinedShips
                        from worstShip in joinedShips.DefaultIfEmpty()
                        join w in wares on worstShip.ItemSoldId equals w.WareID into joinedWares
                        from worstShipWares in joinedWares.DefaultIfEmpty()
                        group new {worstShip, worstShipWares, allShips} by allShips.ShipID into g
                              select new ShipSummaryVM
                              {
                                  Sector = filterSector,
                                  ItemSoldId = filterWare,
                                  LastActivity = filterLastActivity,
                                  OurShipId = g.Key,
                                  Quantity = g.Sum(s => s.worstShip.Quantity),
                                  EstimatedProfit = g.Sum(s =>
                                      s.worstShip.Money / 100 - (s.worstShipWares.TransportType=="container" ? s.worstShipWares.MarketAveragePrice * s.worstShip.Quantity : 0)),
                                  Actif = g.Max(s => s.allShips.Actif)
                              }).ToDataSourceResult(request);
            }
        }
        private static IEnumerable<IFilterDescriptor> GetFilters(IEnumerable<IFilterDescriptor> filters)
        {
            var result = new List<IFilterDescriptor>();
            if (filters.Any())
            {
                foreach (var filter in filters)
                {
                    if (filter is FilterDescriptor)
                        result.Add((FilterDescriptor)filter);
                    else if (filter is CompositeFilterDescriptor)
                    {
                        result.AddRange(GetFilters(((CompositeFilterDescriptor)filter).FilterDescriptors));
                    }
                }
            }

            return result;
        }
    }
}
