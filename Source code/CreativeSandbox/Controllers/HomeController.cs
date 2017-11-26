using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CreativeSandbox.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace CreativeSandbox.Controllers
{
    public class HomeController : Controller
    {
        RoomContext db = new RoomContext();

        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Sandbox", "Home");
            }
            return View();
        }

        [Authorize]
        public ActionResult Sandbox()
        {
            return View(db.Rooms);
        }

        [Authorize]
        public ActionResult Room(int? id)
        {
            if (id == null)
                return RedirectToAction("Sandbox");

            var room = db.Rooms.Where(r => r.Id == id);

            if (room.Count() == 0)
                return RedirectToAction("Sandbox");
            else
                return View(room);
        }

        [HttpPost]
        public JsonResult Upload()
        {
            Account account = new Account()
            {
                ApiKey = "859224862681241",
                ApiSecret = "ZP00qvTxBFl7yhUvSgV7xcBz6N8",
                Cloud = "wom"
            };

            Cloudinary cloudinary = new Cloudinary(account);

            var upload = Request.Files[0];

            if (upload != null)
            {
                string fileName = System.IO.Path.GetFileName(upload.FileName);

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(fileName, upload.InputStream),
                    UseFilename = true,
                    UniqueFilename = !fileName.Contains("room")
                };

                var urlImage = cloudinary.Upload(uploadParams).Uri.ToString();

                if (fileName.Contains("room"))
                {
                    db.Rooms.Find(Int32.Parse(fileName.Replace("room", "").Replace(".png", ""))).Content = urlImage;
                    db.SaveChanges();
                }

                return Json(urlImage);
            }

            return Json("error");
        }
    }
}