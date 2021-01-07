using System;
using Kendo.Mvc.UI;
using APS.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using APS.Areas.Profit.Models;
using APS.Methods.QueriesExtensions;
using BusinessLogic;
using Kendo.Mvc.Extensions;
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
            //example of setting filters server side:
            //if (request.Filters == null)
            //    request.Filters = new List<IFilterDescriptor>();
            //request.Filters.Add(new FilterDescriptor("User", FilterOperator.IsEqualTo, User.Identity.Name));
            //if (request.Sorts == null)
            //    request.Sorts = new List<SortDescriptor>();
            //request.Sorts.Clear();
            //request.Sorts.Add(new SortDescriptor("Time", ListSortDirection.Ascending));
            //return Json(CommonMethods.GetDataResult<TradeOperation, TradeOperationVM>(request));
            var p = new Process();
            return Json(p.GlobalTradeOperations.Select(x=>new TradeOperationVM().Map(x)).ToDataSourceResult(request));
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

        //public ActionResult Save(IEnumerable<IFormFile> files)
        //{
        //    Methods.Profit.Upload.Save(files, User, _logger);

        //    // Return an empty string to signify success
        //    return Content("");
        //}

        public async Task<IActionResult> ClearData()
        {
            throw new NotImplementedException();
            //todo delete data
            _logger.LogInformation("Cleared Profit data of " + User.Identity.Name);
            return Json(new[] { true });
        }
    }
}