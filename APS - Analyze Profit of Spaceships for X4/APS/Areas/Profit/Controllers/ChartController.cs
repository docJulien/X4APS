using APS.Methods.Profit;
using APS.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

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
    }
}