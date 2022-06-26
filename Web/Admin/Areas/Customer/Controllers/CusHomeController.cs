using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBanThuoc.Areas.Customer.Controllers
{
    public class CusHomeController : Controller
    {
        // GET: Customer/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}