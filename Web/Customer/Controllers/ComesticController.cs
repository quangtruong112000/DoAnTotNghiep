using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AppBanThuoc.Models;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using AppBanThuoc.ObjectDetection;
using System.IO;
using System.Web.UI.WebControls;
using System.Web;
using AppBanThuoc.DominantColour;
using System.Drawing;
using RestSharp;
using System.Diagnostics;
using LinqKit;

namespace WebBanThuoc.Areas.Customer.Controllers
{
    public class ComesticController : Controller
    {
        // GET: Customer/Comestic
        public ActionResult Index(int? id, int? page, string sortOrder, string searchString, string objectPhoto)
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
            ViewBag.objectPhoto = objectPhoto;

            List<string> attributes = new List<string>();


            if (objectPhoto != null && objectPhoto.Length > 0)
            {
                string outputFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets/images/output");
                string imgPath = Path.Combine(outputFolder, objectPhoto);
                if (!System.IO.File.Exists(imgPath))
                {
                    // https://docs.microsoft.com/en-us/dotnet/machine-learning/tutorials/object-detection-model-builder
                    ObjectDetection objectDetection = new ObjectDetection();
                    var result = objectDetection.run(objectPhoto);
                    attributes.AddRange(result);
                }

                if (System.IO.File.Exists(imgPath))
                {
                    //https://michaeldavidson.me/technology/2015/10/06/finding-dominant-colours-in-images.html
                    DominantColour dominantColour = new DominantColour();
                    List<Color> colors = dominantColour.GetDominantColour(objectPhoto, true);

                    foreach (Color color in colors)
                    {
                        var client = new RestClient("https://www.thecolorapi.com/id?hex=" + color.Name.Substring(2));
                        client.Timeout = -1;
                        var request = new RestRequest(Method.GET);
                        IRestResponse response = client.Execute(request);
                        JObject json = JObject.Parse(response.Content);

                        if (json != null)
                        {
                            var nameColor = json["name"]["value"].ToString().ToLower().Replace(' ', '_');
                            attributes.Add(nameColor);
                        }
                    }
                }
                else
                {
                    DominantColour dominantColour = new DominantColour();
                    List<Color> colors = dominantColour.GetDominantColour(objectPhoto, false);

                    foreach (Color color in colors)
                    {
                        var client = new RestClient("https://www.thecolorapi.com/id?hex=" + color.Name.Substring(2));
                        client.Timeout = -1;
                        var request = new RestRequest(Method.GET);
                        IRestResponse response = client.Execute(request);
                        JObject json = JObject.Parse(response.Content);

                        if (json != null)
                        {
                            var nameColor = json["name"]["value"].ToString().ToLower().Replace(' ', '_');
                            attributes.Add(nameColor);
                        }
                    }
                }

            }

            Debug.WriteLine(String.Join(", ", attributes));


            // 3. Tạo truy vấn, lưu ý phải sắp xếp theo trường nào đó, ví dụ OrderBy
            // theo LinkID mới có thể phân trang.
            var Service = (from l in db.Products.Where(x => x.product_type == 1 && x.quantity > 0)
                           select l);


            if (!String.IsNullOrEmpty(searchString))
            {
                Service = Service.Where(s => s.name.Contains(searchString) || s.price.Value.ToString().Contains(searchString));
            }

            if (attributes.Count > 0)
            {
                var predicate = PredicateBuilder.New<Product>();

                foreach (string attr in attributes)
                {
                    predicate = predicate.Or(s => s.attributes.Contains(attr));
                }

                Service = Service.Where(predicate);

            }

            if (id != null)
            {
                Service = Service.Where(x => x.category_id == id).OrderBy(x => x.id);
                ViewBag.url = id;
                ViewBag.idCategory = id;
            }

            //else
            //{
            //    var catergory = catergorys.ToList().Last();
            //    Service = (from l in db.Products.Where(x => x.category_id == catergory.id)
            //               select l);
            //    ViewBag.url = catergory.id;
            //    ViewBag.idCategory = catergory.id;
            //}

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
            ViewBag.Brand = db.Brands.Where(x => x.delete != true).ToList();
            return View(Service.ToPagedList(pageNumber, pageSize));
        }
        [ChildActionOnly]
        public ActionResult Cart()
        {
            try
            {
                var Custormer = (Custormer)Session["Login"];
                WebBanThuocDB db = new WebBanThuocDB();
                return PartialView(db.Carts.Where(x => x.uid.Equals(Custormer.uid)).Take(10).ToList());
            }
            catch { }
            return PartialView(new List<Cart>());
        }
        public ActionResult Detail(int? id)
        {
            try
            {

                if (id == null)
                {
                    return View("Error");
                }
                WebBanThuocDB db = new WebBanThuocDB();
                Product product = db.Products.Where(x => x.id == id).FirstOrDefault();
                product.Category.Products = db.Products.Where(x => x.id != id && x.category_id == product.category_id).Take(20).ToList();
                ViewBag.Evaluate = db.Evaluates.Where(x => x.idproduct == id).ToList();
                return View(product);

            }
            catch
            {

                return View("Error");

            }
        }
        public ActionResult GetAllOrder(int? id)
        {
            try
            {
                if (id == null) id = 1;

                ViewBag.Status = id;
                var Custormer = (Custormer)Session["Login"];
                WebBanThuocDB db = new WebBanThuocDB();
                var Carts = db.VoucherOrders.Where(x => x.uid.Equals(Custormer.uid) && x.status == (id ?? 1)).ToList();
                ViewBag.Use = Custormer;
                return View(Carts);
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }

        }
        [HttpPost]
        public ActionResult DetailOrder(FormCollection collection)
        {
            try
            {
                var value = collection["flexRadioDefault"];
                var id = collection["idorder"];



                WebBanThuocDB db = new WebBanThuocDB();
                long Id = long.Parse(id);
                VoucherOrder VoucherOrder = db.VoucherOrders.Find(Id);
                VoucherOrder.statusCancel = int.Parse(value);
                VoucherOrder.status = 6;
                db.SaveChanges();

                return Redirect("/Comestic/GetAllOrder/6");
            }
            catch { return View("Error"); }
        }

        [HttpPost]
        [ValidateInput(false)]
        public void savevalute(String id, String idproduct, String point, String mesage)
        {
            WebBanThuocDB db = new WebBanThuocDB();
            var Custormer = (Custormer)Session["Login"];
            var check = db.Evaluates.Where(x => x.idorderdetail.ToString().Equals(id)).ToList();
            if (check.Count >= 1)
            {

                check[0].point = Convert.ToInt32(point);
                check[0].Message = mesage;
                db.SaveChanges();

            }
            else
            {
                Evaluate evaluate = new Evaluate();
                evaluate.idorderdetail = Convert.ToInt32(id);
                evaluate.idproduct = Convert.ToInt32(idproduct);
                evaluate.Message = mesage;
                evaluate.point = Convert.ToInt32(point);
                evaluate.uid = Custormer.uid;
                db.Evaluates.Add(evaluate);
                db.SaveChanges();
            }

        }

        public ActionResult ListOrder()
        {
            try
            {
                var Custormer = (Custormer)Session["Login"];
                WebBanThuocDB db = new WebBanThuocDB();
                var Carts = db.Carts.Where(x => x.uid.Equals(Custormer.uid)).ToList();
                ViewBag.Use = Custormer;
                return View(Carts);
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult DetailOrder(long? id)
        {
            try
            {

                if (id == null)
                {
                    return View("Error");
                }
                WebBanThuocDB db = new WebBanThuocDB();
                VoucherOrder VoucherOrder = db.VoucherOrders.Where(x => x.id == id).FirstOrDefault();

                return View(VoucherOrder);
            }
            catch { return View("Error"); }
        }
        public ActionResult DeleteCart(long? id)
        {
            try
            {

                if (id == null)
                {
                    return View("Error");
                }
                WebBanThuocDB db = new WebBanThuocDB();
                Cart cart = db.Carts.Find(id);
                db.Carts.Remove(cart);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch { return View("Error"); }
        }

        public ActionResult AddCart(int? id, int? quantity)
        {
            try
            {

                if (id == null)
                {
                    return View("Error");
                }
                if (quantity == null)
                {
                    quantity = 1;
                }
                var Custormer = (Custormer)Session["Login"];
                WebBanThuocDB db = new WebBanThuocDB();


                Product product = db.Products.Where(x => x.id == id).FirstOrDefault();

                Cart cart = db.Carts.Where(x => x.product_id == product.id && x.uid.Equals(Custormer.uid)).FirstOrDefault();

                if (cart != null)
                {

                    cart.quantity += quantity;
                    var price = (product.discount != null && product.discount > 0) ? product.price * ((decimal)product.discount / (decimal)100) : product.price;
                    cart.uid = Custormer.uid;
                    cart.intomoney = cart.quantity * (price);
                    db.SaveChanges();
                }
                else
                {
                    cart = new Cart();
                    cart.quantity = quantity;
                    cart.product_id = id;
                    var price = (product.discount != null && product.discount > 0) ? product.price * ((decimal)product.discount / (decimal)100) : product.price;
                    cart.intomoney = cart.quantity * (price);
                    cart.uid = Custormer.uid;
                    db.Carts.Add(cart);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }

        string serectkey = "sSFqWXSq4QXW9MSr5SDAaGNBjL7FCrIk";
        public async Task<ActionResult> ThanhToan(FormCollection collection)
        {
            var Custormer = (Custormer)Session["Login"];
            WebBanThuocDB db = new WebBanThuocDB();
            var Carts = db.Carts.Where(x => x.uid.Equals(Custormer.uid)).ToList();
            var Name = collection["Name"];
            var Email = collection["Email"];
            var SDT = collection["SDT"];
            var region = collection["region"];
            var district = collection["district"];
            var ward = collection["ward"];
            var Adress = collection["Adress"];
            var Note = collection["Note"];
            var shipe = collection["ship"];
            var RadioPayment = collection["flexRadioPayment"];

            VoucherOrder voucherOrder = new VoucherOrder();
            voucherOrder.Name = Name;
            voucherOrder.email = Email;
            voucherOrder.province = region;
            voucherOrder.district = district;
            voucherOrder.ward = ward;
            voucherOrder.telephone = SDT;
            voucherOrder.adrees = Adress;
            voucherOrder.note = Note;
            voucherOrder.discountAmount = Carts.Sum(x => x.Product.discount);
            voucherOrder.grossAmount = Carts.Sum(x => x.Product.price * x.quantity).Value;
            voucherOrder.uid = Custormer.uid;
            voucherOrder.delete = false;
            voucherOrder.paymentmethods = 0;
            voucherOrder.createdate = DateTime.Now;
            voucherOrder.status = 1;
            voucherOrder.shiper = 0;
            var payment = Carts.Sum(x => x.intomoney);
            if (shipe.Length > 0)
            {
                payment = decimal.Parse(shipe) + payment;
                voucherOrder.shiper = decimal.Parse(shipe);
            }
            if (RadioPayment != null && RadioPayment.Equals("momo"))
            {
                voucherOrder.paymentmethods = 1;
                string partnerCode = "MOMODDI520210624";
                string accessKey = "xc5ROj205YDvqrBn";
                string orderId = Guid.NewGuid().ToString();
                string requestId = Guid.NewGuid().ToString();
                string extraData = "";
                string orderInfo = "Tên Khách Hàng : " + Custormer.name;
                string amount = "" + payment;
                string redirectUrl = "http://pttkhdt.utc2.ga/Comestic/returnUrl";
                string ipnUrl = "http://pttkhdt.utc2.ga/Comestic/notifyurl";
                string requestType = "captureWallet";
                //Before sign HMAC SHA256 signature
                string rawHash = "accessKey=" + accessKey +
                    "&amount=" + amount +
                    "&extraData=" + extraData +
                    "&ipnUrl=" + ipnUrl +
                    "&orderId=" + orderId +
                    "&orderInfo=" + orderInfo +
                    "&partnerCode=" + partnerCode +
                    "&redirectUrl=" + redirectUrl +
                    "&requestId=" + requestId +
                    "&requestType=" + requestType
                    ;


                MoMoSecurity crypto = new MoMoSecurity();
                //sign signature SHA256
                string signature = crypto.signSHA256(rawHash, serectkey);


                //build body json request
                JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "partnerName", "Test" },
                { "storeId", "MomoTestStore" },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderId },
                { "orderInfo", orderInfo },
                { "redirectUrl", redirectUrl },
                { "ipnUrl", ipnUrl },
                { "lang", "en" },
                { "extraData", extraData },
                { "requestType", requestType },
                { "signature", signature }

            };
                var httpClient = new HttpClient();

                var httpContent = new StringContent(message.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync("https://test-payment.momo.vn/v2/gateway/api/create", httpContent);
                string a = await response.Content.ReadAsStringAsync();
                JObject jmessage = JObject.Parse(a);
                Session["Order"] = voucherOrder;
                return Redirect(jmessage.GetValue("payUrl").ToString());

            }
            else
            {
                db.VoucherOrders.Add(voucherOrder);
                db.SaveChanges();

                foreach (var item in Carts)
                {
                    var isDiscountVoucher = (item?.Product?.discount != null) ? item?.Product?.discount * item.quantity : 0;
                    db.VoucherOrderDetails.Add(new VoucherOrderDetail()
                    {
                        product_id = item.product_id,
                        quantity = item.quantity,
                        grossAmount = item.Product.price.Value * item.quantity.Value,
                        discountAmount = isDiscountVoucher,
                        voucherId = voucherOrder.id


                    });
                    db.SaveChanges();

                }
            }

            ViewBag.Message = "Thanh toán thành công!";
            return View("returnUrl");
        }

        public ActionResult returnUrl()
        {
            try
            {
                string param = Request.QueryString.ToString().Substring(0, Request.QueryString.ToString().IndexOf("signature") - 1);
                param = Server.UrlDecode(param);
                MoMoSecurity crypto = new MoMoSecurity();
                //string serectkey = ConfigurationManager.AppSettings["serectkey"].ToString();
                string serectKey = serectkey.ToString();
                string signature = crypto.signSHA256(param, serectKey);
                //if (signature != Request["signature"].ToString())
                //{
                //    ViewBag.Message = "Thông tin không hợp lệ!";
                //    return View();
                //}
                if (Request.QueryString["message"].Equals("Successful."))
                {
                    ViewBag.Message = "Thanh toán thành công!";
                    var Custormer = (Custormer)Session["Login"];
                    WebBanThuocDB db = new WebBanThuocDB();
                    var Carts = db.Carts.Where(x => x.uid.Equals(Custormer.uid)).ToList();
                    VoucherOrder voucherOrder = (VoucherOrder)Session["Order"];
                    voucherOrder.status = 2;
                    db.VoucherOrders.Add(voucherOrder);
                    db.SaveChanges();

                    foreach (var item in Carts)
                    {
                        var isDiscount = (item?.Product?.discount != null) ? item?.Product?.discount * item.quantity : 0;
                        db.VoucherOrderDetails.Add(new VoucherOrderDetail()
                        {
                            product_id = item.product_id,
                            quantity = item.quantity,
                            grossAmount = item.Product.price.Value * item.quantity.Value,
                            discountAmount = isDiscount,
                            voucherId = voucherOrder.id

                        });
                        db.SaveChanges();
                        var product = db.Products.Find(item.id);
                        product.quantity = product.quantity - item.quantity;
                        db.SaveChanges();
                    }

                    db.Carts.RemoveRange(Carts);
                    db.SaveChanges();
                    return View();
                }
                else
                {
                    ViewBag.Message = "Thanh toán thất bại!";
                }
                ViewBag.Message = "Thanh toán thất bại!";
                return View("ErollUrl");
            }
            catch
            {
                return RedirectToAction("Index");
            }

        }
        public JsonResult notifyurl()
        {
            return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    string fileName = Guid.NewGuid().ToString("N");
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        // Get the complete folder path and store the file inside it.  

                        var extension = file.FileName.Split('.');
                        if (extension.Length > 1)
                        {
                            fileName += "." + extension[1];
                        }

                        var fname = Path.Combine(Server.MapPath("~/assets/images/"), fileName);
                        file.SaveAs(fname);
                        break;
                    }

                    // Returns message that successfully uploaded  
                    return Json(new { result = true, message = "File Uploaded Successfully!", filename = fileName });

                }
                catch (Exception ex)
                {
                    return Json(new { result = false, message = "Error occurred. Error details: " + ex.Message });
                }
            }
            else
            {
                return Json(new { result = false, message = "No files selected." });

            }
        }
    }
}