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
            var f = fileUpload.SaveToFolder(typeof(Interviewee).Name, "Data", false);

            var csv = new CsvService();
            csv.BulkImportJob();

            ViewBag.Success = true;
            return View("Index");
        }
    }
}