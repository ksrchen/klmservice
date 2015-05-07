using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace kmlservice.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index(int dateFilter=0, bool csv=false)
        {
            using (var db = new ResIncomeEntities())
            {
                DateTimeOffset start = DateTimeOffset.Now;

                switch(dateFilter)
                {
                    case 0:
                        start = start.AddDays(-1);
                        break;
                    case 1:
                        start = start.AddDays(-7);
                        break;
                    case 2:
                        start = start.AddMonths(-1);
                        break;
                    case 3:
                        start = start.AddMonths(-3);
                        break;
                    case 4:
                        start = start.AddMonths(-6);
                        break;

                }

                var query = from a in db.Audits
                            join u in db.Users on a.UserID equals u.UserID
                            where a.Controller == "ResIncome" && 
                            a.Action == "Get" &&
                            a.Created >= start 
                            orderby a.Created descending 
                            select new Record { @User = u.FirstName + " " + u.LastName, @Email = u.UserID,  @MLSNumber = a.ID, @Created = a.Created };

                if (csv)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("sep=|");
                    sb.AppendLine("User|Email|MLS #|Date");
                    foreach (var item in query)
                    {
                        sb.AppendFormat("{0}|{1}|{2}|{3}\r\n",
                            item.User,
                            item.Email,
                            item.MLSNumber,
                            item.Created
                        );
                    }
                    var content = sb.ToString();
                    return File(Encoding.UTF8.GetBytes(content), "application/vnd.ms-excel");
                    
                }
                else
                {
                    return View(query.ToList());
                }
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
