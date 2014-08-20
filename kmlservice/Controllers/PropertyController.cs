using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Spatial;
using System.Text;
using System.Web.Http;
using System.Xml;

namespace kmlservice.Controllers
{
    public class PropertyController : ApiController
    {
        public HttpResponseMessage Post([FromBody] List<Location> locations)
        {
            try
            {
                using (var db = new test1Entities())
                {
                    foreach (var item in locations)
                    {
                        item.location1 = DbGeography.PointFromText(string.Format("POINT({0} {1})", item.Lng, item.Lat), 4326);
                        db.Locations.Add(item);
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
        public HttpResponseMessage Get(string polygon)        
        {
            try
            {
                string query = string.Format(@"
declare @g geography;
SET @g = geography::STPolyFromText('{0}', 4326);

select top 200 p.propertyId, p.description, p.location 
from [dbo].Location p 
where @g.STContains(p.location)=1 
", polygon);

                XmlDocument xDoc = new XmlDocument();
                XmlDeclaration xDec = xDoc.CreateXmlDeclaration("1.0", "utf-8", null);

                XmlElement rootNode = xDoc.CreateElement("kml");
                rootNode.SetAttribute("xmlns", @"http://www.opengis.net/kml/2.2");
                xDoc.InsertBefore(xDec, xDoc.DocumentElement);
                xDoc.AppendChild(rootNode);
                XmlElement docNode = xDoc.CreateElement("Document");
                rootNode.AppendChild(docNode);

                using (var db = new test1Entities())
                {
                   // db.Database.CommandTimeout = 5 * 60;
                    var g = DbGeography.FromText(polygon);

                    var result = from i in db.Locations.AsNoTracking() 
                                 where g.Intersects(i.location1)
                                 select new Result
                                 {
                                     description = i.description,
                                     location = i.location1,
                                     propertyId = i.propertyId
                                 };
                    //var result = db.Database.SqlQuery<Result>(query, 0).ToList();
                    foreach (var item in result.Take(200))
                    {
                        XmlElement placeNode = xDoc.CreateElement("Placemark");
                        docNode.AppendChild(placeNode);

                        XmlElement idNode = xDoc.CreateElement("id");
                        XmlText idText = xDoc.CreateTextNode(item.propertyId.ToString());
                        placeNode.AppendChild(idNode);
                        idNode.AppendChild(idText);

                        XmlElement descNode = xDoc.CreateElement("description");
                        XmlText descText = xDoc.CreateTextNode(item.description);
                        placeNode.AppendChild(descNode);
                        descNode.AppendChild(descText);

                        XmlElement pointNode = xDoc.CreateElement("Point");
                        placeNode.AppendChild(pointNode);

                        XmlElement coordNode = xDoc.CreateElement("coordinates");
                        XmlText coordText = xDoc.CreateTextNode(string.Format("{0},{1}",
                            item.location.StartPoint.Longitude.GetValueOrDefault(),
                            item.location.StartPoint.Latitude.GetValueOrDefault()));
                        pointNode.AppendChild(coordNode);
                        coordNode.AppendChild(coordText);
                    }
                }

                return new HttpResponseMessage
                {
                    Content = new StringContent(
                        xDoc.InnerXml,
                        Encoding.UTF8,
                        "application/xml"),
                };
            }
            catch (Exception exp)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exp);
            }
        }
    }
    public class Result
    {
        public int propertyId { get; set; }
        public string description { get; set; }
        public DbGeography location { get; set; }
    }
}
