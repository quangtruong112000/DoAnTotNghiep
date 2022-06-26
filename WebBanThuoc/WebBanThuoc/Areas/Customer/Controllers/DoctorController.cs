using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBanThuoc.Areas.Customer.Controllers
{
    public class DoctorController : Controller
    {
        // GET: Customer/Doctor
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult IntroDoctor()
        {
            return View();
        }
    }
}