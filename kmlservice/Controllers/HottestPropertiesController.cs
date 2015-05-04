﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace kmlservice.Controllers
{
    public class HottestPropertiesController : ApiController
    {

        public HttpResponseMessage Get()
        {
            try
            {
                using (var db = new ResIncomeEntities())
                {
                    

                    var result = (from i in db.vwResIncomeSummaries.AsNoTracking() orderby i.ROI descending
                                 select i).Take(10);

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
                                    ListingKey = ((int)i.ListingKey).ToString(),

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
}