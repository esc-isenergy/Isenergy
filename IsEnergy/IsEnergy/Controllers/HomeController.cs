using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IsEnergyModel;

namespace IsEnergy.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]

        public ActionResult Index()
        {
           
           return View();	
        }

        public ActionResult GetError()
        {

            ResultMode result = new ResultMode();
            result.Executed = false;
            result.StrError = "Fwee3fgwegwgweg qwfqgfqwgqg wfqwfqwfFwee3fgfFwee3fqwfFwee3fgwegwgweg qwfqgfqwgqg wfqwfqwfFwee3fgwegwgweg qwfqgfqwgqg wfqwfqwfFwee3fgwegwgweg qwfqgfqwgqg wfqwfqwfFwee3fgwegwgweg qwfqgfqwgqg wfqwfqwfFwee3fgwegwgweg qwfqgfqwgqg wfqwfqwfFwee3fgwegwgweg qwfqgfqwgqg wfqwfqwfFwee3fgwegwgweg qwfqgfqwgqg wfqwfqwfFwee3fgwegwgweg qwfqgfqwgqg wfqwfqwfFwee3fgwegwgweg qwfqgfqwgqg wfqwfqwfFwee3fgwegwgweg qwfqgfqwgqg wfqwfqwfмFwee3fgwegwgweg qwfqgfqwgqg wfqwfqwfFwee3fgwegwgweg qwfqgfqwgqg wfqwfqwfFwee3fgwegwgweg qwfqgfqwgqg wfqwfqwfFwee3fgwegwgweg qwfqgfqwgqg wfqwfqwf";
            result.StrResult = "Не удалось добавить пльзователя";
            return View("~/Views/Shared/ErrorView.cshtml", result);
        }
       

	}
}