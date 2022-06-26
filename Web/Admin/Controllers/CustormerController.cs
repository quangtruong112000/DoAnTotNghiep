﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanThuoc.Models;
using WebBanThuoc.Models.MD5;

namespace WebBanThuoc.Controllers
{
    public class CustormerController : BaseController
    {
        // GET: Custormer
        public ActionResult Index()
        {
            var Custormer = new WebBanThuocDB().Custormers.Where(x => x.delete != true);
            return View(Custormer);
           
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
                var Custormer = db.Custormers.Find(id);
                if (Custormer != null)
                {
                    Custormer.email = "";
                    Custormer.delete = true;
                }

            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                string uid = collection["uid"];
                string name = collection["Name"];
                string SDT = collection["SDT"];
                string Email = collection["Email"];
                string adress = collection["adress"];
                DateTime birthday =(collection["birthday"].Length>0? DateTime.Parse(collection["birthday"]):DateTime.Now);
                WebBanThuocDB db = new WebBanThuocDB();
                if (uid != null && uid.Length > 0)
                {
                    var Custormer = db.Custormers.Find(uid);
                    if (Custormer != null)
                    {

                        Custormer.name = name;
                        Custormer.telephone = SDT;
                        Custormer.email = Email;
                        Custormer.adress = adress;
                        Custormer.birthday = birthday;

                    }
                }
                else
                {
                    var Custormer = db.Custormers.Where(x => x.email.Equals(Email)).FirstOrDefault();
                    if (Custormer != null)
                    {
                        return RedirectToAction("Index");
                    }
                    Custormer = new Custormer()
                    {
                        uid = Guid.NewGuid().ToString(),
                        name = name,
                        telephone = SDT,
                        email = Email,
                        adress = adress,
                        delete = false,
                        password = new MD5().GetMD5("1"),
                        birthday = birthday,


                    };
                    db.Custormers.Add(Custormer);
                }
                db.SaveChangesAsync();
            }
            catch { }


            return RedirectToAction("Index");
        }


    }
}