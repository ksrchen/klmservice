using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kmlservice.Controllers
{
    [Authorize]
    public class LoadDataController : Controller
    {
        // GET: LoadData
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoadListing(HttpPostedFileBase file)
        {
            ViewBag.message = "Loaded";
            return View("Index");
        }
        [HttpPost]
        public ActionResult LoadAttachment(HttpPostedFileBase file)
        {
            ViewBag.message = "";
            try
            {

                loadAttachment(file.InputStream);
                ViewBag.message = "Loaded";
                return View("Index");
            }
            catch (Exception exp)
            {
                ModelState.AddModelError("", exp.Message);
                return View("Index");
            }

        }


        static void loadAttachment(Stream stream)
        {
            using (var db = new ResIncomeEntities())
            {
                db.Database.ExecuteSqlCommand("delete attachments");

                StreamReader reader = new StreamReader(stream);
                string line = string.Empty;
                int lineCount = 0;
                string[] columnHeader = null;
                while (true)
                {
                    line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        break;
                    }
                    lineCount++;

                    if (lineCount == 1)
                    {
                        columnHeader = line.Split(new char[] { '\t' });
                    }
                    else
                    {
                        var data = line.Split(new char[] { '\t' });

                        var item = new attachment();
                        item.Board = data[Array.IndexOf(columnHeader, "Board")];
                        item.ClassID = data[Array.IndexOf(columnHeader, "ClassID")];
                        item.ClassKey = data[Array.IndexOf(columnHeader, "ClassKey")];
                        item.ClassSourceKey = data[Array.IndexOf(columnHeader, "ClassSourceKey")];
                        item.FileExtension = data[Array.IndexOf(columnHeader, "FileExtension")];
                        item.IsDeleted = data[Array.IndexOf(columnHeader, "IsDeleted")];
                        //  item.MediaDescription = data[Array.IndexOf(columnHeader, "MediaDescription")];
                        item.MediaKey = data[Array.IndexOf(columnHeader, "MediaKey")];
                        item.MediaURL = data[Array.IndexOf(columnHeader, "MediaURL")];
                        item.MediaType = data[Array.IndexOf(columnHeader, "MediaType")];
                        item.MLS_ID = data[Array.IndexOf(columnHeader, "MLS_ID")];
                        item.OfficeCode = data[Array.IndexOf(columnHeader, "OfficeCode")];
                        item.SourceKey = data[Array.IndexOf(columnHeader, "SourceKey")];
                        item.TimestampModified = data[Array.IndexOf(columnHeader, "TimestampModified")];
                        item.TimestampUploaded = data[Array.IndexOf(columnHeader, "TimestampUploaded")];
                        db.attachments.Add(item);
                    }
                }
                db.SaveChanges();
            }
        }
    }
}