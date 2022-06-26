using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanThuoc.Models;

namespace WebBanThuoc.Controllers
{
    public class CategoryController : Controller
    {
        public ActionResult Index()
        {
            
            WebBanThuocDB db = new WebBanThuocDB();
            var Categories = db.Categories.Where(x => (x.delete == false  || x.delete == null ) && x.type==1).ToList();
            return View(Categories);
        }

      
     

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
           
            var id = collection["idCategory"];
            if (id.Length > 0)
            {
                WebBanThuocDB db = new WebBanThuocDB();
                Category categoryupdate = db.Categories.Find(int.Parse(id));
                categoryupdate.name = collection["Name"];
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                WebBanThuocDB db = new WebBanThuocDB();
                Category categorynew = new Category();
                categorynew.id = db.Categories.ToList().Last().id+1;
                categorynew.name = collection["Name"];
                categorynew.type = 1;




                categorynew.delete = false;
                db.Categories.Add(categorynew);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
         
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View("Error");
            }
            Category category = new WebBanThuocDB().Categories.Find(id);
          
            return View(category);
        }
        [HttpPost]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                WebBanThuocDB db = new WebBanThuocDB();
                Category categoryupdate = db.Categories.Find(category.id);
                categoryupdate.name = category.name;
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            return View(category);
        }
        [HttpGet]
        public ActionResult Delete(int? id)
        {
           
            if (id == null)
            {
                return View("Error");
            }
            WebBanThuocDB db = new WebBanThuocDB();
            Category category = db.Categories.Find(id);
            category.delete = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        


    }
}