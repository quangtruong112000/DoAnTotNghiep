using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppBanThuoc.Models;
namespace WebBanThuoc.Areas.Customer.Controllers
{
    public class CusHomeController : Controller
    {
        // GET: Customer/Home
        public ActionResult Index()
        {
            WebBanThuocDB db = new WebBanThuocDB();
            var product = db.Products.Where(x => x.delete != true && x.discount>0 && x.quantity>1).Take(10).ToList();
            return View(product);
        }
    }
}