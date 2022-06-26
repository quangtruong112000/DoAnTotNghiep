﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebBanThuoc.Models;
using WebBanThuoc.Models.GiaoHangTietKiem;

namespace WebBanThuoc.Controllers
{
    public class OrderController : BaseController
    {
        // GET: Order
        public ActionResult Index()
        {
            var order = new WebBanThuocDB().VoucherOrders.Where(x => x.delete != true).ToList();
            return View(order);
        }

        public  JsonResult CancelOrderTransport(string id)
        {

            WebBanThuocDB db = new WebBanThuocDB();
            try
            {
                var order = db.VoucherOrders.Where(x => x.Code!=null&&x.Code.Equals(id)).FirstOrDefault(); 
                order.status = 6;
                db.SaveChanges();
            }
            catch
            {
                return Json(new { success=true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }
        

        public ActionResult FollowOrder()
        {
            return View();
        }
        public async Task<JsonResult> GetOrder()
        {
            var order = new WebBanThuocDB().VoucherOrders.Where(x => x.delete != true && x.status==3 ).ToList();
            List<object> data = new List<object>();
            foreach (var item in order)
            {
                var httpClient = new HttpClient();
                //   httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
                httpClient.DefaultRequestHeaders.Add("Token", "891732705c99a1458BC0cc06732B2F2B77437363");
                HttpResponseMessage response = await httpClient.GetAsync("https://services.giaohangtietkiem.vn/services/shipment/v2/" + item.Code.Trim());
                string a = await response.Content.ReadAsStringAsync();
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                object routes_list = json_serializer.DeserializeObject(a);
                data.Add(routes_list);
            }
           
            return Json(new { data }, JsonRequestBehavior.AllowGet);
          
        }

      

        public ActionResult EditOrder(long? id)
        {
            WebBanThuocDB db = new WebBanThuocDB();
            if (id == null)
            {
                return View("Error");
            }
            var order = db.VoucherOrders.Where(x => x.id==id).FirstOrDefault();
            Session["order"] = order;
            return View(order);
        }
        public ActionResult OrderDetail(string id)
        {
           
            WebBanThuocDB db = new WebBanThuocDB();
            if (id == null)
            {
                return View("Error");
            }
            var order = db.VoucherOrders.Where(x => x.Code!=null&&x.Code.Equals(id)).FirstOrDefault();
            if(order!=null) return View("Error");
            Session["order"] = order;
            return View("EditOrder",order);
        }
        public async Task<JsonResult>  Transport(long? id)
        {
            WebBanThuocDB db = new WebBanThuocDB();
            try
            {
                var order = db.VoucherOrders.Where(x => x.id == id).FirstOrDefault();
                CreateOrder createOrder = new CreateOrder();
                createOrder.order = new order()
                {
                    id = Guid.NewGuid().ToString(),
                    pick_name = "Nhà Thuốc Phạm Trọng Trường",
                    pick_address = "455 Lê Văn Việt, Quận 9",
                    pick_province = "TP. Hồ Chí Minh",
                    pick_district = "Quận 9",
                    pick_ward = "Phường Tăng Nhơn Phú A",
                    pick_tel = "0353573456",
                    tel = order.telephone,
                    name = order.Name,
                    address = order.adrees,
                    province = order.province,
                    district = order.district,
                    ward = order.ward,
                    hamlet = "Khác",
                    is_freeship = "0",
                    pick_money = 0,
                    note = order.note,
                    value = (int)(order.grossAmount - order.discountAmount).Value,
                    transport = "road",
                    pick_option = "",
                    deliver_option = "xfast",
                    pick_session = 2,
           
                };
                createOrder.order.tags.Add(1);
                foreach (var item in order.VoucherOrderDetails)
                {
                    createOrder.products.Add(new product{
                        name=item.Product.name,
                        weight=item.Product.weight,
                        quantity=item.quantity,
                        product_code=item.product_id
                    });
                }
                var httpClient = new HttpClient();
                var json = JsonConvert.SerializeObject(createOrder);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Add("Token", "891732705c99a1458BC0cc06732B2F2B77437363");
                HttpResponseMessage response = await httpClient.PostAsync("https://services.giaohangtietkiem.vn/services/shipment/order/?ver=1.6.3", httpContent);
                string a = await response.Content.ReadAsStringAsync();
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                object routes_list = json_serializer.DeserializeObject(a);

                return Json(new { routes_list }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
           
        }

        public async Task<JsonResult> UpdateStust(long id, string label,string estimated_deliver_time)
        {
            WebBanThuocDB db = new WebBanThuocDB();
            var order = db.VoucherOrders.Where(x => x.id == id).FirstOrDefault();
            if (order != null)
            {
                order.status = 3;
                order.Code = label;
                order.deliver_time = estimated_deliver_time;
                await db.SaveChangesAsync();
                return Json(new { data = id }, JsonRequestBehavior.AllowGet);
            }
            
            return Json(new { data=0}, JsonRequestBehavior.AllowGet);
        }
        
                    public ActionResult Delete(long? id)
        {
            WebBanThuocDB db = new WebBanThuocDB();
            try
            {
                var order = db.VoucherOrders.Where(x => x.id == id).FirstOrDefault();
                order.status = 6;
                order.delete = true;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {

            }
            return View("Error");
        }

        public ActionResult CancellOrder(long? id)
        {
            WebBanThuocDB db = new WebBanThuocDB();
            try {
                var order = db.VoucherOrders.Where(x => x.id == id).FirstOrDefault();
                order.status = 6;
                db.SaveChanges();
                return RedirectToAction("Index");
            } catch
            {

            }
            return View("Error");
        }

   
        public  ActionResult UpdateStatust(int? id)
        {

            WebBanThuocDB db = new WebBanThuocDB();
            try
            {
                var order = db.VoucherOrders.Where(x => x.id == id).FirstOrDefault();
                if (order.status == 3)
                {
                    order.status = 4;
                    db.SaveChanges();
                }
                else
                {
                    
                    order.status = 2;
                    db.SaveChanges();
                    foreach (var item in order.VoucherOrderDetails)
                    {
                        var product = db.Products.Find(item.id);
                        product.quantity = product.quantity - item.quantity;
                        db.SaveChanges();
                    }
                }
              
                return View("EditOrder",order);
            }
            catch
            {
                return View("Error");
            }
        }
        [HttpPost]
        public async Task<ActionResult> EditOrder(VoucherOrder orders)
        {
            WebBanThuocDB db = new WebBanThuocDB();
            try
            {
                var order = db.VoucherOrders.Where(x => x.id == orders.id).FirstOrDefault();
                order.adrees = orders.adrees;
                order.Name = orders.Name;
                order.telephone = orders.telephone;
               

             await   db.SaveChangesAsync();

                return View("EditOrder", order);
            }
            catch
            {
                return View("Error");
            }

          
        }

    }
}