using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebBanThuoc.Models;

namespace WebBanThuoc.Controllers
{
    public class ServiceController : BaseController
    {
        // GET: Service
        public ActionResult Index()
        {
            WebBanThuocDB db = new WebBanThuocDB();
            var Categories = db.Categories.Where(x => (x.delete == false || x.delete == null) && x.type == 3).ToList();
            return View(Categories);
        }
        public ActionResult GetAll()
        {
            WebBanThuocDB db = new WebBanThuocDB();
            var Products = db.Products.Where(x => (x.delete == false || x.delete == null) && x.product_type == 3).ToList();
            return View(Products);
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
                categorynew.id = db.Categories.ToList().Last().id + 1;
                categorynew.name = collection["Name"];
                categorynew.type = 3;
                categorynew.delete = false;
                db.Categories.Add(categorynew);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
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
        [HttpGet]
        public ActionResult CreateService()
        {
            WebBanThuocDB db = new WebBanThuocDB();

            List<Category> Category = db.Categories.Where(x => x.delete != true && x.type == 3).ToList();
            ViewBag.Category = new SelectList(Category, "id", "name");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateService(Product product)
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
                productNew.product_type = 3;

                db.Products.Add(productNew);
                db.SaveChanges();
                try
                {
                    List<string> Img = (List<string>)Session["Img"];
                    foreach (var item in Img)
                    {
                        db.Photos.Add(new Photo()
                        {
                            url = item,
                            product_id = productNew.id,
                        });
                        db.SaveChanges();
                    }
                    Session["Img"] = null;
                    return RedirectToAction("GetAll");
                }
                catch { }
            }

            List<Category> Category = db.Categories.Where(x => x.delete != true &&x.type==3).ToList();

            ViewBag.Category = new SelectList(Category, "id", "name");
            return View(product);
        }

        public ActionResult EditService(long? id)
        {
            try
            {
                if (id == null)
                {
                    return View("Error");
                }
                WebBanThuocDB db = new WebBanThuocDB();
                Product productNew = db.Products.Where(x => x.id == id).FirstOrDefault();

                List<Category> Category = db.Categories.Where(x => x.delete != true && x.type == 3).ToList();
 
                ViewBag.Category = new SelectList(Category, "id", "name");
                List<string> photo = new List<string>();
                foreach (var item in productNew.Photos)
                {
                    photo.Add(item.url);
                }
                Session["Img"] = photo;
                return View(productNew);
            }
            catch { }
            return RedirectToAction("Create");
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditService(Product product)
        {
            WebBanThuocDB db = new WebBanThuocDB();
            if (ModelState.IsValid)
            {
                Product productNew = db.Products.Find(product.id);
                if (productNew != null)
                {
                    productNew.name = product.name;
                    productNew.category_id = product.category_id;
                    productNew.delete = false;
                    productNew.price = product.price;
                    productNew.discount = product.discount;
                    productNew.unit = product.unit;
                    productNew.description = product.description;
                    productNew.quantity = product.quantity;
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
                        return RedirectToAction("GetAll");
                    }
                    catch { }
                }
            }
         
            List<Category> Category = db.Categories.Where(x => x.delete != true && x.type == 3).ToList();

            ViewBag.Category = new SelectList(Category, "id", "name");
            return View(product);
        }

      

    }
}