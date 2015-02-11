using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace kmlservice.Controllers
{
    public class ResIncomeController : ApiController
    {

        public HttpResponseMessage Post([FromBody] List<ResIncome> dataSet)
        {
            try
            {
                using (var db = new ResIncomeEntities())
                {
                    foreach (var item in dataSet)
                    {
                        item.Coordinate = DbGeography.PointFromText(string.Format("POINT({0} {1})", item.longitude, item.Latitude), 4326);
                        db.ResIncomes.Add(item);
                    }
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
        public HttpResponseMessage Get(string id)
        {
            try
            {
                using (var db = new ResIncomeEntities())
                {
                    var item = db.ResIncomes.FirstOrDefault(p => p.MLnumber == id);
                    return Request.CreateResponse<ResIncome>(HttpStatusCode.OK, item);
                }
            }
            catch (Exception exp)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exp);
            }
        }

        [HttpGet]
        public HttpResponseMessage Search(string polygon, string filters = null)
        {
            try
            {
                using (var db = new ResIncomeEntities())
                {
                    // db.Database.CommandTimeout = 5 * 60;
                    var g = DbGeography.FromText(polygon);

                    var result = from i in db.vwResIncomeSummaries.AsNoTracking()
                                 where g.Intersects(i.Coordinate) 
                                 select new vwResIncomeSummary
                                 {
                                     MLnumber = i.MLnumber,
                                     City = i.City,
                                     Latitude = i.Latitude,
                                     longitude = i.longitude,
                                     PropertyDescription = i.PropertyDescription,
                                     StreetName = i.StreetName,
                                     StreetNumber = i.StreetNumber,
                                 };
                    return Request.CreateResponse<List<vwResIncomeSummary>>(HttpStatusCode.OK, result.ToList()); 
                }
            }

            catch (Exception exp)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exp);
            }

        }
    }
}
   
