using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using IsEnergyModel;
using System.Web.Security;
using DevExpress.Web.ASPxUploadControl;

namespace IsEnergy.Controllers
{
    [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators.ModifyingAdministrators,Administrators.MainAdministrators")]
    public class AdministratorsController : Controller
    {
        IsEnergyModel.DataMode.ModeAdministrators um = new IsEnergyModel.DataMode.ModeAdministrators();
        private Is_EnergyEntities db = new Is_EnergyEntities();
        //
        // GET: /Administrators/

        public ActionResult Index()
        {
            return View();
        }
        //
        // GET: /Administrators/Details/5

        public ActionResult Details(int id = 0)
        {
            Administrators administrators = db.Administrators.Find(id);
            ViewData["Users"] = db.Users.ToList();
            ViewData["Subscribers"] = db.Subscribers.ToList();

            if (administrators == null)
            {
                return HttpNotFound();
            }
            return View(administrators);
        }

        //
        // GET: /Administrators/Create

        public ActionResult Create()
        {
            ViewData["Users"] = db.Users.ToList();
            ViewData["Subscribers"] = db.Subscribers.ToList();
            return PartialView("Create");
        }

        //
        // POST: /Administrators/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAdd(Administrators administrators)
        {
            IsEnergyModel.ResultMode result = new ResultMode();
            if (ModelState.IsValid)
            {
                result = um.Create(administrators);
                if (result.Executed)
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
        }

        //
        // POST: /Administrators/Edit/5

        [HttpPost]
        public ActionResult Edit(Administrators administrators)
        {
            IsEnergyModel.ResultMode result = new ResultMode();
            if (ModelState.IsValid)
            {
                result = um.Edit(administrators);
                if (result.Executed)
                {
                    return View("Details", administrators);
                }
            }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
 
            
        }

        //
        // POST: /Administrators/Delete/5

        [HttpPost]
        public ActionResult Delete(Administrators administrators)
        {
            int id = administrators.IdAdministrator;
            IsEnergyModel.ResultMode result = new ResultMode();
            if (id!=0)
            {
                result = um.Delete(id);
                if (result.Executed)
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [ValidateInput(false)]
        public ActionResult GridViewPartial()
        {
            var model = db.Administrators;
            return PartialView("_GridViewPartial", model.ToList());
        }

        [ValidateInput(false)]
        public ActionResult DetailsPartial(Administrators administrators)
        {
            ViewData["Users"] = db.Users.ToList();
            ViewData["Subscribers"] = db.Subscribers.ToList();

            return PartialView("_DetailsPartial", administrators);
        }
    }
}
