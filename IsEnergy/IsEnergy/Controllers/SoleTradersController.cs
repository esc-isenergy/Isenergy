using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;

using DevExpress.Web.ASPxRoundPanel;
using System.ComponentModel.DataAnnotations;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxPopupControl;
using DevExpress.Web.ASPxTreeList;
using DevExpress.Web.ASPxTabControl;
using DevExpress.Web.ASPxUploadControl;

using IsEnergy;
using IsEnergyModel;

namespace IsEnergy.Controllers

{
    //[IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
    public class SoleTradersController : Controller
    {
        IsEnergyModel.DataMode.ModeSoleTraders stm = new IsEnergyModel.DataMode.ModeSoleTraders();
        private Is_EnergyEntities db = new Is_EnergyEntities();
         
        //
        // GET: /SoleTraders/
        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult GridViewPartial()
        {
            var model = db.SoleTraders.ToList();
            return PartialView("_GridViewPartial", model);
        }
        public ActionResult Create()
        {
            ViewData["IFNS"] = db.IFNS.ToList();
            ViewData["Regions"] = db.Regions.ToList();
            ViewData["Countries"] = db.Countries.ToList();
            return PartialView("~/Views/SoleTraders/Create/Create.cshtml");
        }
        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        [HttpPost]
        public ActionResult Create(IsEnergyModel.SoleTraders SoleTraders, IsEnergyModel.Address Address, string SoleTradersLogin, string SoleTradersPassword, HttpPostedFileBase file)
        {

            IsEnergyModel.ResultMode result = new ResultMode();

            result = stm.Create(User.Identity.Name, SoleTraders, SoleTradersLogin, SoleTradersPassword, Address, AllMode.ControllersMode.UploadGetByte("fileUploadCreat"));
            if (result.Executed)
            {
                return RedirectToAction("Details", new { id = (result.ObjectResult as SoleTraders).IdSoleTrader }); ;
            }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
        }
        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        public ActionResult Details(int id = 0)
        {
            SoleTraders SoleTraders = db.SoleTraders.Find(id);
            ViewData["IdentifierSubscriber"] = SoleTraders.IdentifierSubscriber;
            if (SoleTraders == null)
            {
                return HttpNotFound();
            }
            else
            {
                SoleTraders.Certificates = db.Certificates.Find(SoleTraders.IdCertificate);
                ViewBag.SoleTraders = SoleTraders;
                return View("Details/Details", SoleTraders);
            }

        }

        public ActionResult RequisiteSoleTradersView(int id)
        {
            ViewData["IFNS"] = db.IFNS.ToList();
            SoleTraders SoleTraders = db.SoleTraders.Find(id);
            ViewBag.SoleTraders = SoleTraders;
            ViewData["SoleTraders"] = SoleTraders;
            return PartialView("~/Views/SoleTraders/Details/Requisite/Requisite.cshtml");
        }
        [Authorize]
        public ActionResult AddressView(int id, bool readonlytype = false)
        {
            SoleTraders SoleTraders = db.SoleTraders.Find(id);
            ViewBag.Address = SoleTraders.Address;
            ViewBag.readonlytype = readonlytype;
            ViewData["Regions"] = db.Regions.ToList();
            ViewData["Countries"] = db.Countries.ToList();
            return PartialView("~/Views/Address/Address.cshtml");
        }
        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        [HttpPost]
        public ActionResult Edit(IsEnergyModel.SoleTraders SoleTraders, IsEnergyModel.Address Address)
        {

            IsEnergyModel.ResultMode result = new ResultMode();
            result = stm.Edit(User.Identity.Name, SoleTraders, Address);
            if (result.Executed)
            {
                return RedirectToAction("Details", new { id = SoleTraders.IdSoleTrader });
            }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
        }
        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        [HttpPost]
        public ActionResult Delete(SoleTraders SoleTraders)
        {
            int id = SoleTraders.IdSoleTrader;
            IsEnergyModel.ResultMode result = new ResultMode();
            if (id != 0)
            {
                result = stm.Delete(id);
                if (result.Executed)
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });

        }
          
    }
}
