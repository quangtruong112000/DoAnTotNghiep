using AppBanThuoc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBanThuoc.Areas.Customer.Controllers
{
    public class ForumController : Controller
    {
        // GET: Forum
        public ActionResult Index()
        {
            try
            {
                WebBanThuocDB db = new WebBanThuocDB();
            var data = db.fora.Select(x => x).OrderByDescending(y => y.Time).ToList();
            return View(data);
            }
            catch
            {
                return View("Error");
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult savepost(String tinnhan, String image)
        {
            WebBanThuocDB db = new WebBanThuocDB();
            var Custormer = (Custormer)Session["Login"];
            forum forum = new forum();
            if (!tinnhan.Equals(""))
            {
                
                forum.uid = Custormer.uid;
                forum.Message = tinnhan;
                forum.Time = DateTime.Now;
                if (image.Length != 0)
                {
                    forum.image = image;
                }
                
                db.fora.Add(forum);
                db.SaveChanges();

            }
            return Json(Convert.ToString(forum.Id), JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult saveanswer(String tinnhan, String image, String id)
        {
            WebBanThuocDB db = new WebBanThuocDB();
            var Custormer = (Custormer)Session["Login"];
            Detailforum forum = new Detailforum();
            if (!tinnhan.Equals(""))
            {
                
                forum.uid = Custormer.uid;
                forum.Message = tinnhan;
                forum.Time = DateTime.Now;
                forum.idforum = Convert.ToInt32( id);
                if (image.Length != 0)
                {
                    forum.image = image;
                }

                db.Detailforums.Add(forum);
                db.SaveChanges();
                return Json(Convert.ToString(forum.id), JsonRequestBehavior.AllowGet);
            }
            return Json(Convert.ToString(forum.id), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase uploadedFiles)
        {
            string returnImagePath = string.Empty;
            string fileName;
            string Extension;
            string imageName;
            string imageSavePath;
            var Custormer = (Custormer)Session["Login"];
            if (uploadedFiles.ContentLength > 0)
            {
                fileName = Path.GetFileNameWithoutExtension(uploadedFiles.FileName);
                Extension = Path.GetExtension(uploadedFiles.FileName);
                imageName = Custormer.uid + DateTime.Now.ToString("yyyyMMddHHmmss");
                imageSavePath = Server.MapPath("~/Content/image/") + imageName +Extension;

                uploadedFiles.SaveAs(imageSavePath);
                returnImagePath = "/Content/image/" + imageName +Extension;
            }

            return Json(Convert.ToString(returnImagePath), JsonRequestBehavior.AllowGet);
        }





    }
}