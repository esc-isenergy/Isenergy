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
    
    public class LegalEntitiesController : Controller
    {
        IsEnergyModel.DataMode.ModeLegalEntities lem = new IsEnergyModel.DataMode.ModeLegalEntities();
      
        private Is_EnergyEntities db = new Is_EnergyEntities();
        //================Юридические  лица==============================================================

        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult GridViewPartial()
        {
            var model = db.LegalEntities.ToList();
            return PartialView("_GridViewPartial", model);
        }

        public ActionResult Create()
        {
            ViewData["IFNS"] = db.IFNS.ToList();
            ViewData["Regions"] = db.Regions.ToList();
            ViewData["Countries"] = db.Countries.ToList();
            return PartialView("~/Views/LegalEntities/Create/Create.cshtml");
        }

        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        [HttpPost]
        public ActionResult Create(IsEnergyModel.LegalEntities legalEntities,IsEnergyModel.Address Address, string legalEntitiesLogin, string legalEntitiesPassword, HttpPostedFileBase file)
        {
           
            IsEnergyModel.ResultMode result = new ResultMode();
          
            result = lem.Create(User.Identity.Name, legalEntities, legalEntitiesLogin, legalEntitiesPassword,Address, AllMode.ControllersMode.UploadGetByte("fileUploadCreat"));
            if (result.Executed)
            {
                return RedirectToAction("Details", new { id = (result.ObjectResult as LegalEntities).IdOrganization });
            }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
        }

        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
         public ActionResult Details(int id = 0)
         { 
             LegalEntities legalEntities = db.LegalEntities.Find(id);
             ViewData["IdentifierSubscriber"] = legalEntities.IdentifierSubscriber;
             if (legalEntities == null)
            {
                return HttpNotFound();
            }
            else
            {   
                legalEntities.Certificates = db.Certificates.Find(legalEntities.IdCertificate);
                ViewBag.legalEntities = legalEntities;
                return View("Details/Details",legalEntities);
            }

        }

         public ActionResult RequisiteLegalEntitieView(int id)
         {
             ViewData["IFNS"] = db.IFNS.ToList();
             LegalEntities legalEntities = db.LegalEntities.Find(id);
             ViewBag.legalEntities = legalEntities;
             ViewData["legalEntities"] = legalEntities;
             return PartialView("~/Views/LegalEntities/Details/Requisite/Requisite.cshtml");
         }

        [Authorize]
         public ActionResult AddressView(int id, bool readonlytype=false)
         {
             LegalEntities legalEntities = db.LegalEntities.Find(id);
             ViewBag.Address = legalEntities.Address;
             ViewBag.readonlytype = readonlytype;
             ViewData["Regions"] = db.Regions.ToList();
             ViewData["Countries"] = db.Countries.ToList();
             return PartialView("~/Views/Address/Address.cshtml");
         }
        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
         [HttpPost]
         public ActionResult Edit(IsEnergyModel.LegalEntities legalEntities, IsEnergyModel.Address Address)
         {
            
             IsEnergyModel.ResultMode result = new ResultMode();
             result = lem.Edit(User.Identity.Name, legalEntities, Address);
             if (result.Executed)
             {
                 return RedirectToAction("Details", new { id = legalEntities.IdOrganization });
             }
             return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
         }
        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
         [HttpPost]
         public ActionResult Delete(LegalEntities legalEntities)
         {
             int id = legalEntities.IdOrganization;
             IsEnergyModel.ResultMode result = new ResultMode();
             if (id != 0)
             {
                 result = lem.Delete(id);
                 if (result.Executed)
                 {
                     return RedirectToAction("Index");
                 }
             }
             return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });

         }
      


    }  
     

}

