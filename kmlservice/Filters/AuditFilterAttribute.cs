using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace kmlservice
{
    public class AuditFilterAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var from = actionContext.Request.Headers.From;

            string controller = actionContext.ControllerContext.ControllerDescriptor.ControllerName;
            string action = actionContext.ActionDescriptor.ActionName;
            string id = actionContext.ActionArguments.ContainsKey("id") ? actionContext.ActionArguments["id"].ToString() : string.Empty;
            string data = JsonConvert.SerializeObject(actionContext.ActionArguments);

            using (var db = new ResIncomeEntities())
            {
                db.Audits.Add(new Audit
                {
                    UserID = from,
                    Action = action,
                    Controller = controller,
                    Created = DateTime.Now,
                    Data = data,
                    ID = id
                });
                db.SaveChanges();
            }
            base.OnActionExecuting(actionContext);
        }
    }
}