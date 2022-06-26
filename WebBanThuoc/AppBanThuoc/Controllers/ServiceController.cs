using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppBanThuoc.Models;
namespace WebBanThuoc.Areas.Customer.Controllers
{
    public class ServiceController : Controller
    {
        // GET: Customer/Service
        public ActionResult Index(int? id, int? page, string sortOrder, string searchString)
        {
            WebBanThuocDB db = new WebBanThuocDB();
            var catergorys = db.Categories.Where(x => x.type == 3).ToList();
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
            var Service = (from l in db.Products.Where(x => x.product_type == 3 && x.quantity > 0)
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
                Service = (from l in Service.Where(x => x.product_type == 3 )
                           select l).OrderBy(x => x.price);
            }
            else
            {
                Service = (from l in Service.Where(x => x.product_type == 3)
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
        public ActionResult Detail()
        {
            return View();
        }

    }
}