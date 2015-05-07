using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kmlservice.Controllers
{
     [Authorize]
    public class LoadDataController : Controller
    {
        // GET: LoadData
        public ActionResult Index()
        {
            return View();
        }
    }
}