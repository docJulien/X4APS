using Microsoft.AspNetCore.Mvc;
using APS.Model;
using APS.Middlewares;
using System.Linq;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace APS.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {

            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

           
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            switch (HttpContext.Response.StatusCode)
            {
                case 404:
                    return View("NotFound");
                default:
                    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrorMessage = ErrorInformations.Message });
            }
        }

        public IActionResult GetGraph()
        {
            using (DBContext db = new DBContext())
            {
                var lst = db.Logs.ToList();

                return Json(lst);
            }
        }
    }
}