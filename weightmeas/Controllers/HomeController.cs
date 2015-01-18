using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
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

            // ===================================================================
            // Check if user already exist. If already exist, then redirect
            // to DisplayError action with errorCode 10001.
            // ===================================================================
            var user = _context.Users.Find(newUser.Username);
            if (user != null) return RedirectToAction("DisplayError", "Error", new {@errorCode = 10001});

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
            var plots = _context.Users.Find(username).WeightPlots.Where(x=>x.PlotStamp>DateTime.Parse("2015-01-01")).OrderByDescending(x => x.PlotStamp);

            var minWeight = plots.Min(x => x.Weight);
            var maxWeight = plots.Max(x => x.Weight);

            minWeight -= 2;
            maxWeight += 2;
            
            foreach(var plot in plots)
            {
                dates.Add(plot.PlotStamp);
                weights.Add(Math.Round(plot.Weight,2));
            }

            var chart = new Chart(540,280);
            chart.AddSeries
                (
                chartType: "line",
                xValue: dates,
                yValues: weights
                );
            chart.SetYAxis("", Math.Round(minWeight,1), Math.Round(maxWeight,1));
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

        public ActionResult SuperTest()
        {
            IWebDriver driver = new PhantomJSDriver();
            driver.Navigate().GoToUrl("http://www.google.com");
            var i = driver.FindElement(By.Name("q"));
            i.SendKeys("Lol");
            i.Submit();
            i = driver.FindElement(By.Name("q"));
            var v = i.GetAttribute("value");
            ViewBag.ResultValue = v;

            return View();
        }

        public ActionResult SetupDemoUser()
        {
            var rand = new Random((int)DateTime.Now.Ticks);

            var demoUser = new User
                {
                    Password = "demo",
                    Username = "demo@demo.com",
                    PrivateToken = TokenFactory.GenerateToken(),
                                  
                };

            // First remove user.
            if (_context.Users.Count(x => x.Username == "demo@demo.com")==1)
            {
                var user = _context.Users.Find(demoUser.Username);
                var weightPlots = new WeightPlot[user.WeightPlots.Count];
                user.WeightPlots.CopyTo(weightPlots,0);

                foreach(var plot in weightPlots)
                {
                    _context.WeightPlots.Remove(plot);
                }
                _context.Users.Remove(user);
            }
            _context.SaveChanges();

            // Add user again.
            _context.Users.Add(demoUser);
            _context.SaveChanges();

            // Get the user back again.
            demoUser = _context.Users.Find(demoUser.Username);

            // Add plots.
            var date = DateTime.Parse("2011-01-01");
            var weight = 110.0;
            double weightLossPerDay = 0.20;
            const double weightLossPerDayDecay = 0.995;
            for(var i=0;i<365;i++)
            {
                var plot = new WeightPlot
                {
                    Weight = weight + (rand.NextDouble()*2),
                    PlotId = Guid.NewGuid(),
                    PlotStamp = date
                };
                
                // Loose weight
                weight = weight - weightLossPerDay + rand.NextDouble()/rand.Next(5,50);
                weightLossPerDay *= weightLossPerDayDecay;
                
                // Append +1 days.
                date = date.AddDays(1);

                // Skip most of the days.
                if (rand.Next(0, 100) > 25) continue;

                demoUser.WeightPlots.Add(plot);
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Demo()
        {
            return View();
        }

    }
}
