using IntervieweeNet45.Models;
using IntervieweeNet45.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntervieweeNet45.Controllers
{
    public class CsvController : Controller
    {
        // GET: Csv
        public ActionResult Index()
        {
            ViewBag.Success = false;
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase fileUpload)
        {
            if(fileUpload != null)
            {
                var f = fileUpload.SaveToFolder(typeof(Interviewee).Name, "Data", false);

                var csv = new CsvService();
                csv.BulkImportJob(1, ",");

                ViewBag.Success = true;
                return View("Index");
            }
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UploadProv(HttpPostedFileBase fileUpload)
        {
            if (fileUpload != null)
            {
                var f = fileUpload.SaveToFolder(typeof(Province).Name, "Data", false);

                var csv = new CsvService();
                csv.BulkImportJob(2, "|");

                ViewBag.Success = true;
                return View("Index");
            }
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UploadCity(HttpPostedFileBase fileUpload)
        {
            if (fileUpload != null)
            {
                var f = fileUpload.SaveToFolder(typeof(CityMun).Name, "Data", false);

                var csv = new CsvService();
                csv.BulkImportJob(3, "|");

                ViewBag.Success = true;
                return View("Index");
            }
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UploadBrgy(HttpPostedFileBase fileUpload)
        {
            if (fileUpload != null)
            {
                var f = fileUpload.SaveToFolder(typeof(Barangay).Name, "Data", false);

                var csv = new CsvService();
                csv.BulkImportJob(4, "|");

                ViewBag.Success = true;
                return View("Index");
            }
            else
                return RedirectToAction("Index");

        }
    }
}