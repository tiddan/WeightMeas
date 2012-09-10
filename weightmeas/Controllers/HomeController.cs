using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using weightmeas.Models;

namespace weightmeas.Controllers
{
    public class HomeController : Controller
    {
        private Context _context = new Context();

        public HomeController()
        {
            _context.Database.CreateIfNotExists();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult RegisterExecute(User newUser)
        {
            // Create the private token.
            newUser.PrivateToken = TokenFactory.GenerateToken();

            // TODO: Maybe we need to check if the private key exist already.

            // Execute register.
            _context.Users.Add(newUser);
            _context.SaveChanges();

            // Redirect to Home screen.
            return RedirectToAction("Home", "Home", new { @username = newUser.Username });
        }

        [HttpPost]
        public ActionResult LoginExecute(string username, string cleartextpassword)
        {
            var cookie = new HttpCookie("Username") {Value = username};
            this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            return Home(username);
        }

        public ActionResult LogoutExecute()
        {
            this.ControllerContext.HttpContext.Response.Cookies.Remove("Username");
            return Index();
        }

        public ActionResult Home(string username)
        {
            // Check if user is logged in.
            var cookie = this.ControllerContext.HttpContext.Response.Cookies.Get("Username");
            return cookie.Value == String.Empty ? Index() : View();
        }

        public ActionResult RegisterWeight(string privateToken)
        {
            var token = new WeightPlot {PrivateToken = privateToken};
            return View(token);
        }

        public ActionResult RegisterWeightExecute(WeightPlot newPlot)
        {
            newPlot.PlotStamp = DateTime.Now;
            newPlot.PlotId = Guid.NewGuid();
            
            var user = _context.Users.First(x => x.PrivateToken == newPlot.PrivateToken);
            if (user == null) return Content("Lol");

            user.WeightPlots.Add(newPlot);
            _context.SaveChanges();
            return Content("Weight saved");
        }

    }
}
