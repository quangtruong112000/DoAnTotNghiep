using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
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
            var Products = db.Products.Where(x => (x.delete == false || x.delete == null) && x.product_type == 1).ToList();
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
            }
            catch { }
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
                List<string> attributes = new List<string>();

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

                    string outputFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "objects/images/output");
                    string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "objects/images");



                    try
                    {
                        List<string> Img = (List<string>)Session["Img"];
                        foreach (var item in Img)
                        {
                            Debug.WriteLine(item);
                            try
                            {
                                FileInfo file = new FileInfo(item);
                                var extension = file.Name.Split('.');
                                var objectPhoto = GenerateID(24);
                                if (extension.Length > 1)
                                {
                                    objectPhoto += "." + extension[extension.Length - 1];
                                }

                                // Tạo một tên file mới => gen ra 24 ký tự random
                                string originImage = AppDomain.CurrentDomain.BaseDirectory + item.Replace('/', '\\');
                                string fileNew = Path.Combine(imagesFolder, objectPhoto);

                                System.IO.File.Copy(originImage, fileNew, true);


                                string imgPath = Path.Combine(outputFolder, objectPhoto);
                                if (!System.IO.File.Exists(imgPath))
                                {
                                    Debug.WriteLine("objectDetection:" + objectPhoto);

                                    // tìm vật thể
                                    // https://docs.microsoft.com/en-us/dotnet/machine-learning/tutorials/object-detection-model-builder
                                    ObjectDetection objectDetection = new ObjectDetection();
                                    var result = objectDetection.run(objectPhoto);
                                    attributes.AddRange(result);
                                }


                                if (System.IO.File.Exists(imgPath))
                                {

                                    // nhận diện màu sắc
                                    DominantColour dominantColour = new DominantColour();
                                    List<Color> colors = dominantColour.GetDominantColour(objectPhoto, true);
                                    Debug.WriteLine("result count: " + colors.Count);

                                    foreach (Color color in colors)
                                    {
                                        Debug.WriteLine("result color: " + color.Name);

                                        // lấy tên màu sắc
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
                                    Debug.WriteLine("result: " + string.Join(", ", attributes));

                                }
                                else
                                {

                                    // nhận diện màu sắc
                                    DominantColour dominantColour = new DominantColour();
                                    List<Color> colors = dominantColour.GetDominantColour(objectPhoto, false);
                                    Debug.WriteLine("result count: " + colors.Count);

                                    foreach (Color color in colors)
                                    {
                                        Debug.WriteLine("result color: " + color.Name);

                                        // lấy tên màu sắc
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
                                    Debug.WriteLine("result: " + string.Join(", ", attributes));

                                }
                            }
                            catch (ExternalException)
                            {

                            }
                            catch (ArgumentNullException)
                            {

                            }
                            db.Photos.Add(new Photo()
                            {
                                url = item,
                                product_id = product.id,
                            });
                            db.SaveChanges();
                        }

                        Product productCurrent = db.Products.Find(product.id);
                        productCurrent.attributes = String.Join(", ", attributes);
                        db.SaveChanges();

                        Debug.WriteLine(String.Join(", ", attributes));

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
            List<Category> Category = db.Categories.Where(x => x.delete != true && x.type == 1).ToList();
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
                List<string> attributes = new List<string>();


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
                productNew.attributes = String.Join(", ", attributes);

                db.Products.Add(productNew);
                db.SaveChanges();

                string outputFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "objects/images/output");
                string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "objects/images");

                try
                {
                    List<string> Img = (List<string>)Session["Img"];
                    foreach (var item in Img)
                    {
                        Debug.WriteLine(item);
                        try
                        {
                            FileInfo file = new FileInfo(item);
                            var extension = file.Name.Split('.');
                            var objectPhoto = GenerateID(24);
                            if (extension.Length > 1)
                            {
                                objectPhoto += "." + extension[extension.Length - 1];
                            }

                            // Tạo một tên file mới => gen ra 24 ký tự random
                            string originImage = AppDomain.CurrentDomain.BaseDirectory + item.Replace('/', '\\');
                            string fileNew = Path.Combine(imagesFolder, objectPhoto);

                            System.IO.File.Copy(originImage, fileNew, true);


                            string imgPath = Path.Combine(outputFolder, objectPhoto);
                            if (!System.IO.File.Exists(imgPath))
                            {
                                Debug.WriteLine("objectDetection:" + objectPhoto);

                                // tìm vật thể
                                // https://docs.microsoft.com/en-us/dotnet/machine-learning/tutorials/object-detection-model-builder
                                ObjectDetection objectDetection = new ObjectDetection();
                                var result = objectDetection.run(objectPhoto);
                                attributes.AddRange(result);
                            }


                            if (System.IO.File.Exists(imgPath))
                            {
                                Debug.WriteLine("dominantColour:" + objectPhoto);

                                // nhận diện màu sắc
                                DominantColour dominantColour = new DominantColour();
                                List<Color> colors = dominantColour.GetDominantColour(objectPhoto, true);

                                foreach (Color color in colors)
                                {
                                    // lấy tên màu sắc
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
                                Debug.WriteLine("result: " + string.Join(", ", attributes));

                            }
                            else
                            {

                                // nhận diện màu sắc
                                DominantColour dominantColour = new DominantColour();
                                List<Color> colors = dominantColour.GetDominantColour(objectPhoto, false);

                                foreach (Color color in colors)
                                {
                                    // lấy tên màu sắc
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
                                Debug.WriteLine("result: " + string.Join(", ", attributes));

                            }
                        }
                        catch (ExternalException)
                        {

                        }
                        catch (ArgumentNullException)
                        {

                        }




                        db.Photos.Add(new Photo()
                        {
                            url = item,
                            product_id = productNew.id,
                        });
                        db.SaveChanges();
                    }

                    ///// Lưu lại cái attributes phát. ko biết cách lưu. attributes là List => String.Join(", ", attributes) để ra lại string rồi lưu
                    Product productCurrent = db.Products.Find(productNew.id);
                    productCurrent.attributes = String.Join(", ", attributes);

                    db.SaveChanges();

                    Debug.WriteLine("result: " + string.Join(", ", attributes));


                    Session["Img"] = null;
                    return RedirectToAction("Index");

                }
                catch
                {

                }



            }

            List<Brand> Brand = db.Brands.Where(x => x.delete != true).ToList();
            List<Category> Category = db.Categories.Where(x => x.delete != true && x.type == 1).ToList();
            ViewBag.Brand = new SelectList(Brand, "name", "name");
            ViewBag.Category = new SelectList(Category, "id", "name");
            return View(product);
        }


        public void SaveImage(string imageUrl, string filename, ImageFormat format)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(imageUrl);
            Bitmap bitmap; bitmap = new Bitmap(stream);

            if (bitmap != null)
            {
                bitmap.Save(filename, format);
            }

            stream.Flush();
            stream.Close();
            client.Dispose();
        }

        protected string GenerateID(int length)
        {
            string alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string small_alphabets = "abcdefghijklmnopqrstuvwxyz";
            string numbers = "1234567890";

            string characters = numbers + alphabets + small_alphabets + numbers;

            string id = string.Empty;
            for (int i = 0; i < length; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, characters.Length);
                    character = characters.ToCharArray()[index].ToString();
                } while (id.IndexOf(character) != -1);
                id += character;
            }

            return id;
        }

    }
}