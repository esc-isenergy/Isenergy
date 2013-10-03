using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IsEnergy.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult Error(string StrError, string StrResult)
        {
            ViewBag.StrResult = StrResult;
            ViewBag.StrError = StrError;
            return View();
        }
        

    }
}
