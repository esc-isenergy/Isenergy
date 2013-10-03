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
            //[IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
    public class PhysicalPersonsController : Controller
    {
        //
        // GET: /PhysicalPersons/

        IsEnergyModel.DataMode.ModePhysicalPersons um = new IsEnergyModel.DataMode.ModePhysicalPersons();
        IsEnergyModel.DataMode.ModeAddress adr = new IsEnergyModel.DataMode.ModeAddress();

        private Is_EnergyEntities db = new Is_EnergyEntities();

        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /PhysicalPersons/Details/5

        public ActionResult Details(int id = 0)
        {
            PhysicalPersons physicalpersons = db.PhysicalPersons.Find(id);
            if (physicalpersons == null)
            {
                return HttpNotFound();
            }
            ViewData["IFNS"] = db.IFNS.ToList();
            ViewData["Regions"] = db.Regions.ToList();
            ViewData["Countries"] = db.Countries.ToList();
            return View("Details/Details", physicalpersons);
        }

        //
        // GET: /PhysicalPersons/Create

        public ActionResult Create()
        {
            ViewData["IFNS"] = db.IFNS.ToList();
            ViewData["Regions"] = db.Regions.ToList();
            ViewData["Countries"] = db.Countries.ToList();
            return PartialView("Create/Create");
        }


        //
        // POST: /PhysicalPersons/Create
        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        [HttpPost]
        public ActionResult CreateAdd(PhysicalPersons physicalpersons, string PhysicalPersonsLogin, string PhysicalPersonsPassword, HttpPostedFileBase file)
        {
            IsEnergyModel.ResultMode result = new ResultMode();
            result = um.Create(User.Identity.Name, physicalpersons, PhysicalPersonsLogin, PhysicalPersonsPassword, physicalpersons.Address, AllMode.ControllersMode.UploadGetByte("fileUploadCreat"));
            if (result.Executed)
            {
                return RedirectToAction("Details", new { id = (result.ObjectResult as PhysicalPersons).IdPhysicalPerson });
            }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
        }


        //
        // POST: /PhysicalPersons/Edit/5
        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        [HttpPost]
        public ActionResult Edit(PhysicalPersons physicalpersons, Address Address)
        {
            IsEnergyModel.ResultMode result = new ResultMode();
            result = um.Edit(User.Identity.Name, physicalpersons, Address);
            if (result.Executed)
            {
                return RedirectToAction("Details", new { id = physicalpersons.IdPhysicalPerson });
            }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
        }

        [Authorize]
        public ActionResult EditAddress(int id, bool readonlytype=false)
        {
            PhysicalPersons physicalperson = db.PhysicalPersons.Find(id);
            ViewBag.Address = physicalperson.Address;
            ViewBag.readonlytype = readonlytype;
            ViewData["Regions"] = db.Regions.ToList();
            return PartialView("~/Views/Address/Address.cshtml");
        }


        //
        // POST: /PhysicalPersons/Delete/5
        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        [HttpPost]
        public ActionResult Delete(PhysicalPersons physicalpersons)
        {
            int id = physicalpersons.IdPhysicalPerson;
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

        public ActionResult GridViewPartial()
        {
            var model = db.PhysicalPersons;
            return PartialView("_GridViewPartial", model.ToList());
        }

        public ActionResult AccessPartial()
        {
            return PartialView("Access/_AccessPartial");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


    }
}
