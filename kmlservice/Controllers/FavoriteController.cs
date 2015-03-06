﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace kmlservice.Controllers
{
    public class FavoriteController : ApiController
    {

        [HttpGet]
        public HttpResponseMessage Get(string id)
        {
            try
            {
                using (var db = new ResIncomeEntities())
                {
                    var query = from f in db.Favoriates
                                where f.UserID == id
                                select new ResIncomeSummary
                            {
                                MLnumber = f.ResIncome.MLnumber,
                                City = f.ResIncome.City,
                                Latitude = f.ResIncome.Latitude,
                                longitude = f.ResIncome.Longitude,
                                PropertyDescription = f.ResIncome.PropertyDescription,
                                StreetName = f.ResIncome.StreetName,
                                StreetNumber = f.ResIncome.StreetNumber,
                                LotSquareFootage = f.ResIncome.LotSquareFootage,
                                State = f.ResIncome.State,
                                PostalCode = f.ResIncome.PostalCode
                            };

                    return Request.CreateResponse(HttpStatusCode.OK, query.ToList());
                }
            }
            catch (Exception exp)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exp);
            }
        }


        public HttpResponseMessage Post([FromBody] Favoriate favoriate)
        {
            try
            {
                using (var db = new ResIncomeEntities())
                {
                    db.Favoriates.Add(favoriate);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, favoriate);
                }
            }
            catch (Exception exp)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exp);
            }
        }

        public HttpResponseMessage Delete(string userId, string MLNumber)
        {
            try
            {
                using (var db = new ResIncomeEntities())
                {
                    var item = db.Favoriates.FirstOrDefault(p => p.UserID == userId && p.MLNumber == MLNumber);
                    if (item != null)
                    {
                        db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                        db.SaveChanges();
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, "ok");
                }
            }
            catch (Exception exp)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exp);
            }
        }
    }
}