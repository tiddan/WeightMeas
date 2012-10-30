using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace weightmeas.Controllers
{
    public class ErrorController : Controller
    {
        private Dictionary<int, string> _errorCodes;

        private Dictionary<int, string> ErrorCodes
        {
            get
            {
                return _errorCodes ?? (_errorCodes = new Dictionary<int, string>()
                    {
                        {10001, "Cannot register user, because username is already taken."},
                    });
            }
        }

        /// <summary>
        /// Render an error
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public ActionResult DisplayError(int errorCode)
        {
            ViewData["ErrorCode"] = errorCode;
            ViewData["ErrorMessage"] = ErrorCodes[errorCode];
            return View();
        }

    }
}
