using IntervieweeNet45.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntervieweeNet45.Controllers
{
    public class ShippingController : Controller
    {
        private IntervieweeDbContext _db = new IntervieweeDbContext();


        // GET: Shipping
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _LoadProvince()
        {
            var provinceData = _db.Provinces.OrderBy(o => o.provDesc).ToList();
            return PartialView(provinceData);
        }

        public ActionResult _LoadCityMun(int provCode)
        {
            var cityMunData = _db.CityMuns
                .Where(x => x.provCode == provCode)
                .OrderBy(o => o.citymunDesc)
                .ToList();
            return PartialView(cityMunData);
        }
    }
}