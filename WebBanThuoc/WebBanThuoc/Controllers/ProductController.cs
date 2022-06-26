using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanThuoc.Models;

namespace WebBanThuoc.Controllers
{
    public class ProductController : BaseController
    {
        // GET: Product
        public ActionResult Index()
        {
            WebBanThuocDB db = new WebBanThuocDB();
            var Products = db.Products.Where(x => (x.delete == false || x.delete == null )&& x.product_type==1).ToList();
            return View(Products);
        }
        
   
        public ActionResult Edit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return View("Error");
                }
                WebBanThuocDB db = new WebBanThuocDB();
                Product productNew = db.Products.Where(x => x.id == id).FirstOrDefault();
                List<Brand> Brand = db.Brands.Where(x => x.delete != true).ToList();
                List<Category> Category = db.Categories.Where(x => x.delete != true && x.type == 1).ToList();
                ViewBag.Brand = new SelectList(Brand, "name", "name");
                ViewBag.Category = new SelectList(Category, "id", "name");
                List<string> photo = new List<string>();
                foreach (var item in productNew.Photos)
                {
                    photo.Add(item.url);
                }
                Session["Img"] = photo;
                return View(productNew);
            } catch { }
            return RedirectToAction("Create");
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Product product)
        {
            WebBanThuocDB db = new WebBanThuocDB();
            if (ModelState.IsValid)
            {
                Product productNew = db.Products.Find(product.id);
                if (productNew != null)
                {
                    productNew.product_brand = product.product_brand;
                    productNew.name = product.name;
                    productNew.category_id = product.category_id;
                    productNew.delete = false;
                    productNew.price = product.price;
                    productNew.discount = product.discount;
                    productNew.unit = product.unit;
                    productNew.description = product.description;
                    productNew.quantity = product.quantity;
                  //  db.Products.Add(productNew);
                    var photo = db.Photos.Where(x => x.product_id == product.id);
                    db.Photos.RemoveRange(photo);
                    db.SaveChanges();
                    try
                    {
                        List<string> Img = (List<string>)Session["Img"];
                        foreach (var item in Img)
                        {
                            db.Photos.Add(new Photo()
                            {
                                url = item,
                                product_id = product.id,
                            });
                            db.SaveChanges();
                        }
                        Session["Img"] = null;
                        return RedirectToAction("Index");
                    }
                    catch { }
                }
            }
            List<Brand> Brand = db.Brands.Where(x => x.delete != true).ToList();
            List<Category> Category = db.Categories.Where(x => x.delete != true && x.type == 1).ToList();
            ViewBag.Brand = new SelectList(Brand, "name", "name");
            ViewBag.Category = new SelectList(Category, "id", "name");
            return View(product);
        }

        public ActionResult Delete(long id)
        {
            WebBanThuocDB db = new WebBanThuocDB();
            Product productNew = db.Products.Find(id);
            if (productNew != null)
            {
                productNew.delete = true;
                db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        public ActionResult Create()
        {
            WebBanThuocDB db = new WebBanThuocDB();
            List<Brand> Brand = db.Brands.Where(x => x.delete != true).ToList();
            List<Category> Category = db.Categories.Where(x => x.delete != true && x.type==1 ).ToList();
            ViewBag.Brand = new SelectList(Brand, "name", "name");
            ViewBag.Category = new SelectList(Category, "id", "name");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Product product)
        {
            WebBanThuocDB db = new WebBanThuocDB();
            if (ModelState.IsValid)
            {
                Product productNew = new Product();
                productNew.product_brand = product.product_brand;
                productNew.name = product.name;
                productNew.category_id = product.category_id;
                productNew.delete = false;
                productNew.price = product.price;
                productNew.discount = product.discount;
                productNew.unit = product.unit;
                productNew.description = product.description;
                productNew.quantity = product.quantity;
                productNew.product_type = 1;
             
                db.Products.Add(productNew);
                db.SaveChanges();
                try {
                    List<string> Img =(List<string>) Session["Img"];
                    foreach (var item in Img)
                    {
                        db.Photos.Add(new Photo()
                        {
                            url=item,
                            product_id=productNew.id,
                        });
                        db.SaveChanges();
                    }
                    Session["Img"] = null;
                    return RedirectToAction("Index");
                } catch {  }
                
            }
      
            List<Brand> Brand = db.Brands.Where(x => x.delete != true).ToList();
            List<Category> Category = db.Categories.Where(x => x.delete != true && x.type==1).ToList();
            ViewBag.Brand = new SelectList(Brand, "name", "name");
            ViewBag.Category = new SelectList(Category, "id", "name");
            return View(product);
        }

    }
}