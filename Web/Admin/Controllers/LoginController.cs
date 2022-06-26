using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanThuoc.Models;
using WebBanThuoc.Models.Login;
using WebBanThuoc.Models.MD5;

namespace WebBanThuoc.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Employer employer)
        {
            WebBanThuocDB db = new WebBanThuocDB();
            if (ModelState.IsValid)
            {
                try
                {
                    var pass= new MD5().GetMD5(employer.password);
                  var emloyer=  db.Employers.Where(x => x.email.ToLower().Equals(employer.email.ToLower()) && x.password.Equals(pass)).FirstOrDefault();
                    if (emloyer != null)
                    {
                        User user = new User()
                        {
                            Uid=emloyer.uid,
                            UseName=emloyer.name,
                            position= (emloyer.position.Equals("Quản Lý")),
                        };
                      Session["Login"]= user;
                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError("Erol", "Tên đăng nhập hoặc mật khẩu không đúng");


                }
                catch { }

            }
            return View("Index",employer);
        }
         
    }
}