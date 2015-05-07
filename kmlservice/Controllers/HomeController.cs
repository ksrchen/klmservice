using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kmlservice.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var db = new ResIncomeEntities())
            {
                var query = from a in db.Audits
                            join u in db.Users on a.UserID equals u.UserID
                            where a.Controller == "ResIncome" && 
                            a.Action == "Get" 
                            orderby a.Created descending 
                            select new Record { @User = u.FirstName + " " + u.LastName, @Email = u.UserID,  @MLSNumber = a.ID, @Created = a.Created };
                            
                return View(query.ToList());
            }
        }
    }

    public class Record
    {
        public string User { get; set; }
        public string MLSNumber { get; set; }
        public string Email { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}
