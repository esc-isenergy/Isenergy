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

    //[IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators.ModifyingAdministrators")]
    [Authorize]

    public class UsersController : Controller
    {
        //
        // GET: /Users/

        IsEnergyModel.DataMode.ModeUser um = new IsEnergyModel.DataMode.ModeUser();


        private Is_EnergyEntities db = new Is_EnergyEntities();

        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return PartialView("Create");
        }

        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        [HttpPost]
        public ActionResult Create(IsEnergyModel.Users users)
        {
            IsEnergyModel.ResultMode result = new ResultMode();
            string pass = TextBoxExtension.GetValue<string>("Password");
            if (ModelState.IsValid && !string.IsNullOrEmpty(pass))
            {
                result = um.Create(users, pass, AllMode.ControllersMode.UploadGetByte("fileUploadCreat"));
                if (result.Executed)
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
        }

        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        [HttpPost]
        public ActionResult Edit(Users users)
        {
            IsEnergyModel.ResultMode result = new ResultMode();
            if (ModelState.IsValid)
            {
                result = um.Edit(users);
                if (result.Executed)
                {
                    return View("Details", users);
                }
            }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
        }
        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        [HttpPost]
        public ActionResult PrivateEdit(Users users)
        {
            IsEnergyModel.ResultMode result = new ResultMode();
            if (ModelState.IsValid)
            {
                result = um.Edit(users);
                if (result.Executed)
                {
                    return RedirectToAction("PrivateOfficeDetails");
                }
            }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
        }

        [ValidateInput(false)]
        public ActionResult GridViewPartial()
        {
            var model = db.Users.ToList();
            return PartialView("_GridViewPartial", model);
        }

        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        [HttpPost]
        public ActionResult EditNotificationSettings(Users users)
        {
            IsEnergyModel.ResultMode result = new ResultMode();
            if (ModelState.IsValid)
            {
                result = um.EditNotices(users);
                if (result.Executed)
                {
                    return RedirectToAction("PrivateOfficeDetails");
                }
            }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
        }

        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        [HttpPost]
        public ActionResult EditUserNotificationSettings(Users users)
        {
            IsEnergyModel.ResultMode result = new ResultMode();
            if (ModelState.IsValid)
            {
                result = um.EditNotices(users);
                if (result.Executed)
                {
                    return Redirect(String.Format("Details/{0}", users.IdUser));
                }
            }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
        }

        public ActionResult Details(int id = 0)
        {
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            else
            {
                users.Certificates = db.Certificates.Find(users.IdCertificate);
                return View(users);
            }

        }

        [Authorize]
        public ActionResult PrivateOfficeDetails()
        {
            Users user = db.Users.First(s => s.Login == User.Identity.Name);
            return View("PrivateOffice/Details", user);

        }

        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        public bool EditPassword(int id = 0, string str = "")
        {
            IsEnergyModel.ResultMode result = new ResultMode();
            try
            {                
                Users user = db.Users.Find(id);
                result = um.EditPass(user, str);
                return true;
                //return RedirectToAction("Details", new { id = user.IdUser });
            }
            catch { return false; }
        }

        public ActionResult NewCertificateAddStart()
        {

            return PartialView("_LoadCertificate");
        }

        public ActionResult NewPrivateCertificateAddStart()
        {

            return PartialView("PrivateOffice/_LoadCertificate");
        }

        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        [HttpPost]
        public ActionResult NewCertificateAdd(int id)
        {
            IsEnergyModel.ResultMode result = new ResultMode();
            result = um.AddCertificate(id, AllMode.ControllersMode.UploadGetByte("fileUploadEdit"));
            if (result.Executed)
            {
                return RedirectToAction("Details", new { id = id });
            }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
        }

        //
        [Authorize]
        [HttpPost]
        public ActionResult NewPrivateCertificateAdd(int id)
        {
            IsEnergyModel.ResultMode result = new ResultMode();
            result = um.AddCertificate(id, AllMode.ControllersMode.UploadGetByte("fileUploadEdit"));
            if (result.Executed)
            {
                return RedirectToAction("PrivateOfficeDetails");
            }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
        }

        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        [HttpPost]
        public ActionResult Delete(Users users)
        {
            int id = users.IdUser;
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
        public ActionResult GridViewArchivCertificate(int id)
        {
            ViewData["idUser"] = id;
            var model = db.Certificates.Where(s => s.IdUser == id);
            return PartialView("_GridViewArchivCertificate", model.ToList());
        }

        //----------------------- загрузки сертификата из архива-------------------
        [ValidateInput(false)]
        public ActionResult LoadCert(int id)
        {
            Certificates model = db.Certificates.Find(id);

            string file_type = "application/cer";
            string file_name = "cert.cer";
            return File(model.CertificateFile, file_type, file_name);


        }
        //-------------------------------------------------------------------------

    }
}
