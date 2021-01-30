using System;
using System.Threading.Tasks;
using APS.Areas.Gestion.Models;
using APS.Methods.Gestion;
using APS.Methods.Profit;
using APS.Middlewares;
using APS.Model;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace APS.Areas.Profit.Controllers
{
    [Area("Profit")]
    [Authorize(Roles = Helpers.ConstanteRole.Administrateur)]
    public class ChartController : Controller
    {
        private readonly ILogger _logger;
        public ChartController(ILogger<ChartController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            {
                return View();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>        
        [ExceptionMessages(ResourceKey = "ReadTradeOperation")]
        public IActionResult ReadTradeOperation()
        {
            return Json(Chart.GetTradeOperation(User.Identity.Name));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>        
        [ExceptionMessages(ResourceKey = "ReadShip")]
        public IActionResult ReadShip()
        {
            return Json(Chart.ReadShip(User.Identity.Name));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>        
        [ExceptionMessages(ResourceKey = "ReadShipPerformance")]
        public IActionResult ReadShipPerformance([DataSourceRequest] DataSourceRequest request)
        {
            return Json(Chart.GetWorstShips(request));
        }

        /// <summary>
        /// Sauvegarde les informations du profil utilisateur entrées dans le formulaire pour la modification
        /// </summary>
        /// <param name="request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [ExceptionMessages(ResourceKey = "Update")]
        public async Task<IActionResult> UpdateShipPerformance([DataSourceRequest] DataSourceRequest request, ShipSummaryVM model)
        {
            Chart.UpdateShip(model);
            _logger.LogInformation("update of ship data" + JsonConvert.SerializeObject(model) + " by " + User.Identity.Name);

            return Json(Chart.GetWorstShips(request));
        }
    }
}