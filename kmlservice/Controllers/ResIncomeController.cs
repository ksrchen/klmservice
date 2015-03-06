using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Http;
using System.Web;
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
                        item.Coordinate = DbGeography.PointFromText(string.Format("POINT({0} {1})", item.Longitude, item.Latitude), 4326);
                        db.ResIncomes.Add(item);
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
                    var query = from i in db.ResIncomes where i.MLnumber == id 
                                select new ResIncomeSummary
                                {
                                 MLnumber = i.MLnumber,
                                 City = i.City,
                                 Latitude = i.Latitude,
                                 longitude = i.Longitude,
                                 PropertyDescription = i.PropertyDescription,
                                 StreetName = i.StreetName,
                                 StreetNumber = i.StreetNumber,
                                 LotSquareFootage = i.LotSquareFootage,
                                 State = i.State,
                                 PostalCode = i.PostalCode
                             };

                    return Request.CreateResponse<ResIncomeSummary>(HttpStatusCode.OK, query.FirstOrDefault());
                }
            }
            catch (Exception exp)
            {
                Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(exp));
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exp);
            }
        }

        
   

     [HttpPut]
        public HttpResponseMessage Search(SearchRequest request)
        {
            try
            {
                using (var db = new ResIncomeEntities())
                {
                    // db.Database.CommandTimeout = 5 * 60;
                    var g = DbGeography.FromText(request.Polygon);

                    var result = from i in db.ResIncomes.AsNoTracking()
                                 where g.Intersects(i.Coordinate) 
                                 select i;
                                
                    if (!string.IsNullOrWhiteSpace(request.Filters))
                    {
                        result = result.Where(request.Filters, null);
                    }
                    var items = from i in result
                             select new ResIncomeSummary
                             {
                                 MLnumber = i.MLnumber,
                                 City = i.City,
                                 Latitude = i.Latitude,
                                 longitude = i.Longitude,
                                 PropertyDescription = i.PropertyDescription,
                                 StreetName = i.StreetName,
                                 StreetNumber = i.StreetNumber,
                                 LotSquareFootage = i.LotSquareFootage,
                                 State = i.State,
                                 PostalCode = i.PostalCode
                             };

                    return Request.CreateResponse<List<ResIncomeSummary>>(HttpStatusCode.OK, items.ToList()); 
                }
            }

            catch (Exception exp)
            {
                Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(exp));
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exp);
            }

        }
    }


    public class ResIncomeSummary
    {
        public string MLnumber { get; set; }
        public string StreetName { get; set; }
        public string StreetNumber { get; set; }
        public string City { get; set; }
        public string PropertyDescription { get; set; }
        public Nullable<double> longitude { get; set; }
        public Nullable<double> Latitude { get; set; }
        public double? LotSquareFootage { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
    }

    public class SearchRequest
    {
        public string Polygon { get; set; }
        public string Filters { get; set; }
    }
}
   
