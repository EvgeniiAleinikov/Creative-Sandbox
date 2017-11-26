using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CreativeSandbox.Models;

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
    }
}