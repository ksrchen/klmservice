using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kmlservice.Controllers
{
    public class PropertyProfileController : Controller
    {
        // GET: PropertyProfile
        public ActionResult Index(string id)
        {
            using (var db = new ResIncomeEntities())
            {
                var item = db.ResIncomes.FirstOrDefault(p => p.MLnumber == id);
                if (item != null)
                {
                    return View(item);

                }
                else
                {
                    return View("NotFound");
                }
            }
        }
    }
}