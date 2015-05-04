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
                                select new ResIncomeDetail
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
                                 PostalCode = i.PostalCode,
                                 ListingKey = ((int) i.ListingKey).ToString(),
                                 ListingAgentFirstName = i.LA_FirstName,
                                 ListingAgentLastName = i.LA_LastName,
                                 ListingOffice = i.LO_Name,
                                 ROI = i.ROI.HasValue? i.ROI.Value: 0,
                                 ListPrice = i.ListPrice.HasValue? i.ListPrice.Value: 0,
                                 NumberOfUnits = i.NumberUnits.HasValue? i.NumberUnits.Value : 0 
                             };

                    var item = query.FirstOrDefault();
                    if (item != null)
                    {
                        item.MediaURLs = new List<string>();
                        foreach (var q in db.attachments.Where(p => p.ClassKey == item.ListingKey && p.MediaType == "IMAGE"))
                        {
                            item.MediaURLs.Add(q.MediaURL);
                        }
                    }

                    var expense = db.vwExpenses.FirstOrDefault(p => ((int) p.ListingKey).ToString() == item.ListingKey);
                    if (expense != null)
                    {
                        item.GrossIncome = expense.GrossIncome;
                        item.Mortage = expense.Mortage;
                        item.PropetyTax = expense.PropertyTax;
                        item.PropertyManagementFee = expense.PropertyManagement;
                        item.Insurance = (double?) expense.Insurance;
                        item.Downpayment = expense.DownPayment;
                    }

                    return Request.CreateResponse<ResIncomeSummary>(HttpStatusCode.OK, item);
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

                    var result = from i in db.vwResIncomeSummaries.AsNoTracking()
                                 where g.Intersects(i.Coordinate) 
                                 select i;
                                
                    if (!string.IsNullOrWhiteSpace(request.Filters))
                    {
                        result = result.Where(request.Filters, null);
                    }
                    result = result.Take(100);

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
                                 PostalCode = i.PostalCode,
                                 MediaURL = i.MediaURL,
                                 ListingKey = ((int) i.ListingKey).ToString(),
                                 ROI = i.ROI.HasValue? i.ROI.Value :0,
                                 ListPrice = i.ListPrice.HasValue? i.ListPrice.Value: 0,
                                 NumberOfUnits = i.NumberUnits.HasValue? i.NumberUnits.Value: 0,
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
        public String ListingKey { get; set; }
        public String MediaURL { get; set; }
        public double ROI { get; set; }
        public double ListPrice { get; set; }
        public double NumberOfUnits { get; set; }
        
    }
    public class ResIncomeDetail : ResIncomeSummary
    {
        public List<String> MediaURLs { get; set; }
        public string ListingAgentFirstName { get; set; }
        public string ListingAgentLastName { get; set; }
        public string ListingOffice { get; set; }

        public double? Mortage { get; set; }
        public double? PropetyTax { get; set; }
        public double? PropertyManagementFee { get; set; }
        public double? Insurance { get; set; }
        public double? Downpayment { get; set; }
        public double? GrossIncome { get; set; }
            
    }
    public class SearchRequest
    {
        public string Polygon { get; set; }
        public string Filters { get; set; }
    }
}
   
