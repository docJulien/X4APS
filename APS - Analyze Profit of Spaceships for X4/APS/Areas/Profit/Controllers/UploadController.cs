using Kendo.Mvc.UI;
using APS.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using APS.Areas.Profit.Models;
using APS.Model;
using Kendo.Mvc.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace APS.Areas.Profit.Controllers
{
    [Area("Profit")]
    [Authorize(Roles = Helpers.ConstanteRole.Administrateur)]
    public class UploadController : Controller
    {
        private readonly ILogger _logger;
        public UploadController(ILogger<UploadController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// </summary>
        /// <param name="request"></param>        
        [ExceptionMessages(ResourceKey = "Read")]
        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(APS.Methods.Profit.Upload.GetTradeOperation(User.Identity.Name, request));
        }

        public IActionResult Index()
        {
            {
                var model = new UploadModel()
                {
                    AllowedExtensions = new string[] { "json" }
                };


                return View(model);
            }
        }

        public ActionResult Save(string filePath)
        {
            Methods.Profit.Upload.Save(filePath, User, _logger);

            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult ClearData()
        {
            using (var db = new DBContext())
            {
                //todo user specific data, truncate will not work if we have a FK
                db.Database.ExecuteSqlRaw($"TRUNCATE TABLE TradeOperations");
                db.SaveChangesAsync();
            }
            _logger.LogInformation("Cleared Profit data of " + User.Identity.Name);
            return Json(new[] { true });
        }
    }
}