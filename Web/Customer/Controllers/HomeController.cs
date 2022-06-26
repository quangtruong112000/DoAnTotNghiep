using AppBanThuoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppBanThuoc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        

        public ActionResult UseInfor()
        {
            Custormer custormer = new Custormer();
            try {
                custormer = (Custormer)Session["Login"];
            } catch { }
      
        
            return PartialView(custormer);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}