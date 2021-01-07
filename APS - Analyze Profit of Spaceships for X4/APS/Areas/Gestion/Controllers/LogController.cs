using Kendo.Mvc.UI;
using APS.Model;
using APS.Methods.Common;
using APS.Areas.Gestion.Models;
using APS.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Kendo.Mvc;

namespace APS.Areas.Gestion.Controllers
{
    [Area("Gestion")]
    [Authorize(Roles = Helpers.ConstanteRole.Administrateur)]
    public class LogController : Controller
    {
        public LogController()
        {
        }

        public IActionResult Index()
        {
            ViewData["levels"] = new List<Level>()
            {
                new Level() { Name = string.Empty },
                new Level() { Name = "Info" },
                new Level() { Name = "Debug" },
                new Level() { Name = "Warn" },
                new Level() { Name = "Error" },
                new Level() { Name = "Fatal" }
            };
            return View();
        }

        /// <summary>
        /// Appel Ajax de grille kendo principale de ce controlleur
        /// </summary>
        /// <param name="request"></param>        
        [ExceptionMessages(ResourceKey = "Read")]
        public IActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            FilterDescriptor descriptor;
            bool FoundAMessageFilter = false;
            //enlever les messages répétitifs
            foreach (IFilterDescriptor fu in request.Filters)
            {
                if (fu.GetType().IsAssignableFrom(typeof(FilterDescriptor)))
                {
                    descriptor = (FilterDescriptor)fu;
                    if (descriptor.Member.Equals("Message")) {

                        FoundAMessageFilter = true;
                        if  (descriptor.Operator == FilterOperator.DoesNotContain)
                            request.Filters.Remove(fu);
                    }
                }
            }
            descriptor = new FilterDescriptor { Member = "Message", Operator = FilterOperator.DoesNotContain, Value = "CreateNewTickets" };
            request.Filters.Add(descriptor);
            if (!FoundAMessageFilter) {
                descriptor = new FilterDescriptor { Member = "Message", Operator = FilterOperator.DoesNotContain, Value = "ApiGetPeseeSuccessLog" };
                request.Filters.Add(descriptor);
            }

            return Json(CommonMethods.GetDataResult<Log,LogVM>(request));
        }

    }
}