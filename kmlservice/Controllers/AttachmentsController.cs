using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace kmlservice.Controllers
{
    public class AttachmentsController : ApiController
    {
        public HttpResponseMessage Post([FromBody] List<attachment> dataSet)
        {
            try
            {
                using (var db = new ResIncomeEntities())
                {
                    foreach (var item in dataSet)
                    {
                        db.attachments.Add(item);
                    }
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (DbEntityValidationException e)
            {
                Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(e));
                return Request.CreateResponse(HttpStatusCode.OK, e.EntityValidationErrors);
            }
            catch (Exception exp)
            {
                Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(exp));
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exp);
            }
        }

        [HttpGet]
        public HttpResponseMessage Get(string id)
        {
            try
            {
                using (var db = new ResIncomeEntities())
                {
                    var query = from i in db.attachments
                                where i.ClassKey == id orderby i.MediaOrder 
                                select new attachment
                                {
                                    ID = i.ID,
                                    MediaURL = i.MediaURL
                                    
                                };

                    return Request.CreateResponse<attachment>(HttpStatusCode.OK, query.FirstOrDefault());
                }
            }
            catch (Exception exp)
            {
                Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(exp));
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exp);
            }
        }

    }
}
