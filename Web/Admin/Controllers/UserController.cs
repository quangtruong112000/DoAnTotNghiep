using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanThuoc.Models;
using WebBanThuoc.Models.MD5;
using WebBanThuoc.Models.MD5;

namespace WebBanThuoc.Controllers
{
    public class UserController : BaseController
    {
        // GET: User
        public ActionResult Index()
        {
            var user=new WebBanThuocDB().Employers.Where(x=>x.delete!=true);
            return View(user);
        }
        [HttpGet]
        public ActionResult Delete(string id)
        {
            WebBanThuocDB db = new WebBanThuocDB();
            if (id == null)
            {
                return View("Error");
            }
            if (id != null && id.Length > 0)
            {
                var employer = db.Employers.Find(id);
                if (employer != null)
                {
                    employer.email = "";
                    employer.delete = true;
                }

            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try {
                string uid = collection["uid"];
                string name = collection["Name"];
                string SDT = collection["SDT"];
                string Email = collection["Email"];
                string position = collection["position"];
                DateTime birthday = (collection["birthday"].Length > 0 ? DateTime.Parse(collection["birthday"]) : DateTime.Now);
                WebBanThuocDB db = new WebBanThuocDB();
                if (uid != null && uid.Length>0)
                {
                    var employer = db.Employers.Find(uid);
                    if (employer != null)
                    {

                        employer.name = name;
                        employer.telephone = SDT;
                        employer.email = Email;
                        employer.position = position;
                        employer.birthday = birthday;
                       
                    }

                }
                else
                {
                    var employer = db.Employers.Where(x=>x.email.Equals(Email)).FirstOrDefault();
                    if (employer != null)
                    {
                        return RedirectToAction("Index");
                    }
                     employer = new Employer()
                    {
                        uid = Guid.NewGuid().ToString(),
                        name = name,
                        telephone = SDT,
                        email = Email,
                        position = position,
                        delete = false,
                        password = new MD5().GetMD5("1"),
                        birthday = birthday,


                    };
                    db.Employers.Add(employer);  
                }
                db.SaveChangesAsync();
            } catch { }
         
          
            return RedirectToAction("Index");
        }
        




    }
}