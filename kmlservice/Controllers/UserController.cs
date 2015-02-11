using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace kmlservice.Controllers
{
    public class UserController : ApiController
    {
        public HttpResponseMessage Post([FromBody] User user)
        {
            try
            {
                using (var db = new ResIncomeEntities())
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (DbEntityValidationException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, e.EntityValidationErrors);
            }
            catch (Exception exp)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exp);
            }
        }

        [HttpGet]
        public HttpResponseMessage Login(string userID, string password)
        {
            try
            {
                using (var db = new ResIncomeEntities())
                {
                    var item = db.Users.FirstOrDefault(p => p.UserID == userID && p.Password == password);
                    if (item != null)
                    {
                        item.LastSignOn = DateTimeOffset.Now;
                        db.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, e.EntityValidationErrors);
            }
            catch (Exception exp)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exp);
            }
        }
    }
}
