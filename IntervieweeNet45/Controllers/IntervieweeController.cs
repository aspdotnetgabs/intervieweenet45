using IntervieweeNet45.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntervieweeNet45.Controllers
{
    public class IntervieweeController : Controller
    {
        private IntervieweeDbContext db = new IntervieweeDbContext();

        // GET: Interviewee
        public ActionResult Index()
        {
            var data = db.Interviewees.Take(10).ToList();
            return View(data);
        }

        [HttpPost]
        public ActionResult Search(string keyword)
        {
            var data = db.Interviewees.Where(x => x.Company.Contains(keyword)).Take(10).ToList();
            return PartialView(data);
        }
    }
}