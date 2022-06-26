using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanThuoc.Models;

namespace WebBanThuoc.Controllers
{
    public class BrandController : BaseController
    {
        // GET: Brand
        public ActionResult Index()
        {
            WebBanThuocDB db = new WebBanThuocDB();
            return View(db.Brands.Where(x=>x.delete!=true).ToList());
        }
        [HttpGet]
        public ActionResult Delete(long? id)
        {

            if (id == null)
            {
                return View("Error");
            }
            WebBanThuocDB db = new WebBanThuocDB();
            var Brand = db.Brands.Find(id);
            Brand.delete = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult GetById(long? id)
        {

       
            WebBanThuocDB db = new WebBanThuocDB();
            var BrandById = db.Brands.Find(id);
            var Brand = new Brand() { id = id.Value, name=BrandById.name,url_image=BrandById.url_image };
            return Json(new { Brand }, JsonRequestBehavior.AllowGet);
          
        }
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            WebBanThuocDB db = new WebBanThuocDB();
            var id  = collection["idBrand"];
            if (id.Length > 0)
            {
                var BrandUpdate = db.Brands.Find(long.Parse(id));
                BrandUpdate.name = collection["Name"];
                BrandUpdate.url_image = collection["UrlImg"];
                db.SaveChanges();
            }
            else
            {
                Brand brandNew = new Brand();
                brandNew.name = collection["Name"];
                brandNew.url_image = collection["UrlImg"]; 
                brandNew.delete = false;
                db.Brands.Add(brandNew);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

    }
}