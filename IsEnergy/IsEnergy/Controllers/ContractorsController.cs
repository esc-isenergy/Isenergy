using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using IsEnergyModel;
using System.Web.Security;
using DevExpress.Web.ASPxUploadControl;
using System.Data;

namespace IsEnergy.Controllers
{
    public class ContractorsController : Controller
    {
        //
        // GET: /Contractors/
        IsEnergyModel.Is_EnergyEntities db = new IsEnergyModel.Is_EnergyEntities();
        IsEnergyModel.DataMode.ModeContractors um = new IsEnergyModel.DataMode.ModeContractors();

        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult GridViewPartial()
        {
            Users user = db.Users.First(s => s.Login == User.Identity.Name);
            Subscribers subscr = db.Subscribers.FirstOrDefault(y => y.IdentifierSubscriber == user.IdentifierSubscriberDefault);
            var model1 = db.Contractors.Where(s => s.IdentifierSubscriber == subscr.IdentifierSubscriber);
            var model = model1.Where(s => s.State == 1);
            return PartialView("_GridViewPartial", model.ToList());
        }

        public ActionResult DeleteContractor(int IdContractor)
        {
            IsEnergyModel.ResultMode result = new ResultMode();
            result = um.DelContractors(IdContractor);
            if (result.Executed)
                {
                    Users user = db.Users.First(s => s.Login == User.Identity.Name);
                    Subscribers subscr = db.Subscribers.FirstOrDefault(y => y.IdentifierSubscriber == user.IdentifierSubscriberDefault);
                    var model1 = db.Contractors.Where(s => s.IdentifierSubscriber == subscr.IdentifierSubscriber);
                    var model = model1.Where(s => s.State == 1);
                    return PartialView("_GridViewPartial", model.ToList());
                }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
        }


        public ActionResult AddContractor()
        {
            ViewData["Subscribers"] = db.Subscribers.ToList();
            return PartialView("AddContractor");
        }

        [HttpPost]
        public ActionResult AddContractorCreate(Contractors contractors)
        {
            IsEnergyModel.ResultMode result = new ResultMode();
            if (ModelState.IsValid)
            {
                result = um.AddContractors(User.Identity.Name, contractors.IdentifierSubscriberContractor, contractors.InvitationText);
                if (result.Executed)
                {
                    Users user = db.Users.First(s => s.Login == User.Identity.Name);
                    Subscribers subscr = db.Subscribers.FirstOrDefault(y => y.IdentifierSubscriber == user.IdentifierSubscriberDefault);
                    var model1 = db.Contractors.Where(s => s.IdentifierSubscriberContractor == subscr.IdentifierSubscriber);
                    var model = model1.Where(s => s.State == 0);
                    return PartialView("_GridViewPartialInputInvitation", model.ToList());
                  
                }
            }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });

        }

        [ValidateInput(false)]
        public ActionResult GridViewPartialInputInvitation()
        {
            Users user = db.Users.First(s => s.Login == User.Identity.Name);
            Subscribers subscr = db.Subscribers.FirstOrDefault(y => y.IdentifierSubscriber == user.IdentifierSubscriberDefault);
            var model1 = db.Contractors.Where(s => s.IdentifierSubscriberContractor == subscr.IdentifierSubscriber);
            var model = model1.Where(s => s.State == 0);
            return PartialView("_GridViewPartialInputInvitation", model.ToList());
        }

      
        [HttpPost, ValidateInput(false)]
        public ActionResult ApplyContractors(int IdContractor)
        {
            IsEnergyModel.ResultMode result = new ResultMode();
            result = um.ApplyContractors(IdContractor);
            if (result.Executed)
            {
                return RedirectToAction("GridViewPartialInputInvitation");
            }

            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult FailContractors(int IdContractor)
        {
            IsEnergyModel.ResultMode result = new ResultMode();
            result = um.FailContractors(IdContractor);
            if (result.Executed)
            {
                return RedirectToAction("GridViewPartialInputInvitation");
            }

            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
        }

        [ValidateInput(false)]
        public ActionResult GridViewPartialOutputInvitation()
        {
            
            Users user = db.Users.First(s => s.Login == User.Identity.Name);
            Subscribers subscr = db.Subscribers.FirstOrDefault(y => y.IdentifierSubscriber == user.IdentifierSubscriberDefault);
            var model1 = db.Contractors.Where(s => s.IdentifierSubscriber == subscr.IdentifierSubscriber);
            var model = model1.Where(s => s.State == 0);
            return PartialView("_GridViewPartialOutputInvitation", model.ToList());
        }

        
        [HttpPost, ValidateInput(false)]
        public ActionResult CancelContractors(int IdContractor)
        {
            IsEnergyModel.ResultMode result = new ResultMode();
            result = um.CancelContractors(IdContractor);
            if (result.Executed)
            {
                


                Users user = db.Users.First(s => s.Login == User.Identity.Name);
                Subscribers subscr = db.Subscribers.FirstOrDefault(y => y.IdentifierSubscriber == user.IdentifierSubscriberDefault);
                var model1 = db.Contractors.Where(s => s.IdentifierSubscriber == subscr.IdentifierSubscriber);
                var model = model1.Where(s => s.State == 0);
                return PartialView("_GridViewPartialOutputInvitation", model.ToList());
            }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
        }
    }
}
