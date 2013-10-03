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
    [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators.ModifyingOperators,Administrators.MainAdministrators")]
    public class OperatorsController : Controller
    {
        //
        // GET: /Operators/

        IsEnergyModel.DataMode.ModeOperators um = new IsEnergyModel.DataMode.ModeOperators();

            private Is_EnergyEntities db = new Is_EnergyEntities();
            //
            // GET: /Operators/

            public ActionResult Index()
            {
                return View();
            }
            //
            // GET: /Operators/Details/5

            public ActionResult Details(int id = 0)
            {
                Operators operators = db.Operators.Find(id);
                ViewData["Users"] = db.Users.ToList();
                ViewData["Subscribers"] = db.Subscribers.ToList();

                if (operators == null)
                {
                    return HttpNotFound();
                }
                return View(operators);
            }
            
            //
            // GET: /Operators/Create

            public ActionResult Create()
            {
                ViewData["Users"] = db.Users.ToList();
                ViewData["Subscribers"] = db.Subscribers.ToList();
                return PartialView("Create");
            }

            //
            // POST: /Operators/Create

            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult CreateAdd(Operators operators)
            {
                IsEnergyModel.ResultMode result = new ResultMode();
                if (ModelState.IsValid)
                {
                    result = um.Create(operators);
                    if (result.Executed)
                    {
                        return RedirectToAction("Index");
                    }
                }
                return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
            }

            //
            // POST: /Operators/Edit/5

            [HttpPost]
            public ActionResult Edit(Operators operators)
            {
                IsEnergyModel.ResultMode result = new ResultMode();
                if (ModelState.IsValid)
                {
                    result = um.Edit(operators);
                    if (result.Executed)
                    {
                        return View("Details", operators);
                    }
                }
                return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
 
            }

            //
            // POST: /Operators/Delete/5

            [HttpPost]
            public ActionResult Delete(Operators operators)
            {
                int id = operators.IdOperator;
                IsEnergyModel.ResultMode result = new ResultMode();
                if (id != 0)
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
                var model = db.Operators;
                return PartialView("_GridViewPartial", model.ToList());
            }

            [ValidateInput(false)]
            public ActionResult DetailsPartial(Operators operators)
            {
                ViewData["Users"] = db.Users.ToList();
                ViewData["Subscribers"] = db.Subscribers.ToList();

                return PartialView("_DetailsPartial", operators);
            }
        
    }
}
