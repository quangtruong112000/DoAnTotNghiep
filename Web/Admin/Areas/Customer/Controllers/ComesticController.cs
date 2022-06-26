using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanThuoc.Models;

namespace WebBanThuoc.Areas.Customer.Controllers
{
    public class ComesticController : Controller
    {
        // GET: Customer/Comestic
        public ActionResult Index(int? id, int? page, string sortOrder, string searchString)
        {
            WebBanThuocDB db = new WebBanThuocDB();
            var catergorys = db.Categories.Where(x => x.type == 1).ToList();
            // 1. Tham số int? dùng để thể hiện null và kiểu int
            // page có thể có giá trị là null và kiểu int.

            if (sortOrder == null)
            {

                sortOrder = "asc";
            }
            ViewBag.sortOrder = sortOrder;
            // 2. Nếu page = null thì đặt lại là 1.
            if (page == null) page = 1;
            ViewBag.searchValue = searchString;
            // 3. Tạo truy vấn, lưu ý phải sắp xếp theo trường nào đó, ví dụ OrderBy
            // theo LinkID mới có thể phân trang.
            var Service = (from l in db.Products.Where(x => x.product_type == 1)
                           select l);


            if (!String.IsNullOrEmpty(searchString))
            {
                Service = Service.Where(s => s.name.Contains(searchString) || s.price.Value.ToString().Contains(searchString));
            }

            if (id != null)
            {
                Service = Service.Where(x => x.category_id == id).OrderBy(x => x.id);
                ViewBag.url = id;
                ViewBag.idCategory = id;
            }
            else
            {
                var catergory = catergorys.ToList().Last();
                Service = (from l in db.Products.Where(x => x.category_id == catergory.id)
                           select l);
                ViewBag.url = catergory.id;
                ViewBag.idCategory = catergory.id;
            }
            if (sortOrder == "asc")
            {
                Service = (from l in Service.Where(x => x.product_type == 1)
                           select l).OrderBy(x => x.price);
            }
            else
            {
                Service = (from l in Service.Where(x => x.product_type == 1)
                           select l).OrderByDescending(x => x.price);
            }
            // 4. Tạo kích thước trang (pageSize) hay là số Link hiển thị trên 1 trang
            int pageSize = 10;
            ViewBag.catergory = catergorys;
            // 4.1 Toán tử ?? trong C# mô tả nếu page khác null thì lấy giá trị page, còn
            // nếu page = null thì lấy giá trị 1 cho biến pageNumber.
            int pageNumber = (page ?? 1);
            ViewBag.page = page;
            ViewBag.pageSize = pageSize;
            //https://localhost:44383/Customer/Service
            // 5. Trả về các Link được phân trang theo kích thước và số trang.
            return View(Service.ToPagedList(pageNumber, pageSize));
        }
        [ChildActionOnly]
        public ActionResult Cart()
        {
            WebBanThuocDB db = new WebBanThuocDB();
            return PartialView(db.Carts.Where(x=>x.uid.Equals("6eab3c96-6c62-4996-8499-12e2b81d25ae")).ToList());
        }
            public ActionResult Detail(int? id)
        {
            try {

                if (id == null)
                {
                    return View("Error");
                }
                WebBanThuocDB db = new WebBanThuocDB();
                Product product = db.Products.Where(x => x.id == id).FirstOrDefault();
                product.Category.Products = db.Products.Where(x => x.id != id && x.category_id==product.category_id).Take(20).ToList();
                return View(product);

            } catch {
               
                    return View("Error");
                
            }
           

     
        }

     
        public ActionResult AddCart(int? id)
        {
            try
            {

                if (id == null)
                {
                    return View("Error");
                }
                WebBanThuocDB db = new WebBanThuocDB();
                Product product = db.Products.Where(x => x.id == id).FirstOrDefault();

                Cart cart = db.Carts.Where(x => x.product_id == product.id).FirstOrDefault();

                if (cart != null)
                {
                    cart.quantity++;
                    var price = (product.discount != null) ? product.price - product.discount : product.price;
                    cart.uid = "6eab3c96-6c62-4996-8499-12e2b81d25ae";
                    cart.intomoney = cart.quantity * (price);
                    db.SaveChanges();
                }
                else
                {
                     cart = new Cart();
                    cart.quantity=1;
                    cart.product_id = id;
                   var price = (product.discount != null) ? product.price - product.discount : product.price;
                    cart.intomoney = cart.quantity * (price);
                    cart.uid = "6eab3c96-6c62-4996-8499-12e2b81d25ae";
                    db.Carts.Add(cart);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch
            {

                return View("Error");

            }
        }
        
    }
}