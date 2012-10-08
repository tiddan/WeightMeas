using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using weightmeas.Models;

namespace weightmeas.Controllers
{
    /// <summary>
    /// Home controller.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// The database context.
        /// </summary>
        protected Context _context = new Context();

        #region Render views

        /// <summary>
        /// Render 'Index' View
        /// Display welcome page to the web site.
        /// </summary>
        /// <returns>Returns 'Index' page.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Render 'Home' View
        /// </summary>
        /// <param name="privateToken">The private token.</param>
        /// <returns></returns>
        public ActionResult Home(string privateToken)
        {
            var user = LoginUsingToken(privateToken);
            if (user == null) RedirectToAction("Index");

            return View(user);
        }

        /// <summary>
        /// Render 'Register' View
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Render 'RegisterWeight' View.
        /// </summary>
        /// <param name="privateToken">The private token.</param>
        /// <returns></returns>
        public ActionResult RegisterWeight(string privateToken)
        {
            var user = LoginUsingToken(privateToken);
            if (user == null) RedirectToAction("Index");

            var plot = new WeightPlot { PrivateToken = user.PrivateToken };
            return View(plot);
        }

        #endregion

        #region Form actions

        /// <summary>
        /// Register new user.
        /// </summary>
        /// <param name="newUser">The new user to register.</param>
        /// <returns></returns>
        public ActionResult RegisterExecute(User newUser)
        {
            // Create the private token.
            newUser.PrivateToken = TokenFactory.GenerateToken();

            // TODO: Maybe we need to check if the private key exist already.

            // Execute register.
            _context.Users.Add(newUser);
            _context.SaveChanges();

            LoginUsingToken(newUser.PrivateToken);

            // Redirect to Home screen.
            return RedirectToAction("Home", "Home");
        }

        /// <summary>
        /// Login a user.
        /// </summary>
        /// <param name="loggedInUser">The user to log in.</param>
        /// <returns></returns>
        public ActionResult LoginExecute(User loggedInUser)
        {
            var privateToken = _context.Users.Find(loggedInUser.Username).PrivateToken;

            var validUsers = _context.Users.Count(x => x.Username == loggedInUser.Username && x.Password == loggedInUser.Password);

            if (validUsers == 1)
            {
                Session.Add("PrivateToken", privateToken);
                return RedirectToAction("Home", new { @username = loggedInUser.Username });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Log out user.
        /// </summary>
        /// <returns></returns>
        public ActionResult LogoutExecute()
        {
            Session.Remove("PrivateToken");
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Execute register weight action.
        /// </summary>
        /// <param name="newPlot">The plot to add.</param>
        /// <returns></returns>
        public ActionResult RegisterWeightExecute(WeightPlot newPlot)
        {
            newPlot.PlotStamp = DateTime.Now;
            newPlot.PlotId = Guid.NewGuid();

            var user = _context.Users.First(x => x.PrivateToken == newPlot.PrivateToken);
            if (user == null) return Content("Lol");

            user.WeightPlots.Add(newPlot);
            _context.SaveChanges();
            return RedirectToAction("Home", new { @privateToken = newPlot.PrivateToken });
        }

        #endregion

        /// <summary>
        /// Render weight graph.
        /// </summary>
        /// <param name="username">The username to display graph for.</param>
        /// <returns></returns>
        public ActionResult MyChart(string username)
        {
            var dates = new List<DateTime>();
            var weights = new List<double>();
            var plots = _context.Users.Find(username).WeightPlots.OrderByDescending(x => x.PlotStamp);

            var minWeight = plots.Min(x => x.Weight);
            var maxWeight = plots.Max(x => x.Weight);

            minWeight -= 2;
            maxWeight += 2;
            
            foreach(var plot in plots)
            {
                dates.Add(plot.PlotStamp);
                weights.Add(plot.Weight);
            }

            var chart = new Chart(width: 540, height: 280);
            chart.AddSeries
                (
                chartType: "line",
                xValue: dates,
                yValues: weights
               );
            chart.SetYAxis("Kg", minWeight, maxWeight);
            chart.Write("png");
            

            return null;
        }

        /// <summary>
        /// Login using the private token.
        /// </summary>
        /// <param name="privateToken">The private token.</param>
        /// <returns></returns>
        private User LoginUsingToken(string privateToken)
        {
            if(Session["PrivateToken"]!=null)
            {
                privateToken = (string) Session["PrivateToken"];
                var user = _context.Users.First(x => x.PrivateToken == privateToken);
                return user;
            }

            if (privateToken == null || privateToken == String.Empty)
            {
                return null;
            }

            var validUsers = _context.Users.Count(x => x.PrivateToken == privateToken);

            if(validUsers==1)
            {
                var user = _context.Users.First(x => x.PrivateToken == privateToken);
                Session.Add("PrivateToken",user.PrivateToken);
                return user;
            }

            return null;
        }

        

        

    }
}
