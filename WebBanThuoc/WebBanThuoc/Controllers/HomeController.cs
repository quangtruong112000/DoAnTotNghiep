using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebBanThuoc.Models;

namespace WebBanThuoc.Controllers
{
    public class HomeController : BaseController
    {
     
        public ActionResult Index()
        {       
            return View();
        }
        private DateTime MondayOfWeek(DateTime date)
        {

            var dayOfWeek = date.DayOfWeek;

            if (dayOfWeek == DayOfWeek.Sunday)
            {
                return date.AddDays(-6);
            }
            int offset = dayOfWeek - DayOfWeek.Monday;
            return date.AddDays(-offset);
        }
        public JsonResult statisticalWeek()
        {
            DateTime fromdate = MondayOfWeek(DateTime.Now); 
            DateTime todate = fromdate.AddDays(6);
            WebBanThuocDB db = new WebBanThuocDB();
            var voucherOrder = db.VoucherOrders.Where(x => x.status > 1 && x.status != 6 && x.delete != true && x.createdate != null && x.createdate.Value>=fromdate && x.createdate.Value <= todate).ToList();
            var product = db.Products.ToList();
            var voucherOrderDetail = db.VoucherOrderDetails.ToList();

            var voucherOrders = (from o in voucherOrder 
                                 select new
                                 {
                                   date=o.createdate.Value.ToShortDateString(),
                                    o.grossAmount,
                                 }
                               ).ToList();
            var data = (from p in voucherOrders
                       group p by new { p.date }
                       into gr
                       select new
                       {
                           date = gr.Key.date,
                           grossAmount = gr.Sum(x => x.grossAmount)
                       }
                      ).ToList();
            List<string> arr = new List<string>();
            arr = data.Select(x => x.date).ToList();
            List<long> arr1 = new List<long>();
            arr1 = data.Select(x => ((long)x.grossAmount.Value)).ToList();
            return Json(new { arr,arr1 }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult statistical()
        {
            DateTime fromdate = MondayOfWeek(DateTime.Now);
            DateTime todate = fromdate.AddDays(6);
            WebBanThuocDB db = new WebBanThuocDB();
            var voucherOrder = db.VoucherOrders.Where(x => x.status > 1 && x.status != 6 && x.delete != true && x.createdate != null && x.createdate.Value >= fromdate && x.createdate.Value <= todate).ToList();
            var product = db.Products.ToList();
            var voucherOrderDetail = db.VoucherOrderDetails.ToList();
            var productDetail = (from o in voucherOrder
                       join od in voucherOrderDetail on o.id equals od.voucherId
                       join p in product on od.product_id equals p.id
                      
                       select new
                       {
                           p.id,
                           Name=p.name,
                           sl=p.quantity,
                       }
                                 ).ToList();
            var arr = (from p in productDetail
                       group p by new { p.Name, p.id }
                       into gr
                       select new
                       {
                           name = gr.Key.Name,
                           sl = gr.Sum(x => x.sl)
                       }
                      ).ToList().Take(5) ;

            return Json(new { arr }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Category(string id)
        {
            WebBanThuocDB db = new WebBanThuocDB();
            var Categories = db.Categories.Select(x => x).ToList();
            return View(Categories);
        }

   
    }
}