using System;
using System.Linq;
using System.Web.Mvc;
using CreativeSandbox.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace CreativeSandbox.Controllers
{
    public class HomeController : Controller
    {
        private readonly RoomContext _db = new RoomContext();

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
            return View(_db.Rooms);
        }

        [Authorize]
        public ActionResult Room(int? id)
        {
            if (id == null)
                return RedirectToAction("Sandbox");
            var room = _db.Rooms.Where(r => r.Id == id);
            if (!room.Any())
                return RedirectToAction("Sandbox");
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
                    UniqueFilename = fileName != null && !fileName.Contains("room")
                };
                var urlImage = cloudinary.Upload(uploadParams).Uri.ToString();
                if (fileName != null && fileName.Contains("room"))
                {
                    _db.Rooms.Find(Int32.Parse(fileName.Replace("room", "").Replace(".png", ""))).Content = urlImage;
                    _db.SaveChanges();
                }
                return Json(urlImage);
            }
            return Json("error");
        }
    }
}