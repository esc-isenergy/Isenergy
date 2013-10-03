using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using WebMatrix.WebData;
using IsEnergyModel;
using DevExpress.Web.Mvc;
using DevExpress.Web.ASPxUploadControl;
using DevExpress.Web.ASPxEditors;


namespace IsEnergy.Controllers
{
    [Authorize]

    public class AccountController : Controller
    {

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /Account/Login
        MembershipProvider domainProvider;

        [HttpPost]
        [AllowAnonymous]
        public Boolean LoginOn(string Login, string Password)
        {
            domainProvider = Membership.Providers["IsEnergyADMembershipProvider"];


            if (domainProvider.ValidateUser(Login, Password)
                || domainProvider.ValidateUser(@"senergy\" + Login, Password)
                || domainProvider.ValidateUser(Login + "@senergy.local", Password))
            {

                Is_EnergyEntities db = new Is_EnergyEntities();
                try
                {
                    Users user = db.Users.First(u => u.Login == Login);
                    FormsAuthentication.SetAuthCookie(Login, true);
                    return true;
                }
                catch
                {
                    return false;
                }

            }
            return false;
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {

            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult GetUserInformation(string Login = "")
        {
            if (Login != "")
            {
               
                Is_EnergyEntities db = new Is_EnergyEntities();
                try
                {
                     Users user = IsEnergyModel.Filters.AuthorizeMeAll.GetCurrentUser(Login);
                     if (user != null)
                     {

                         if (user.MidleName.Count() > 0 | user.LastName.Count() > 0)
                         {
                             char[] abr = user.MidleName.ToCharArray();
                             user.MidleName = abr[0] + ".";
                             abr = user.LastName.ToCharArray();
                             user.LastName = abr[0] + ".";
                         }
                         ViewData["UserListAbonent"] = IsEnergyModel.Filters.AuthorizeMeAll.GetSubscribersUser(user);

                         return PartialView("LogOnPanel", user);
                     }
                }
                catch { }
            }
            return HttpNotFound();
        }

        public ActionResult ChangeOrganization(Users user, int idUser)
        {
            user.IdUser = idUser;
            IsEnergyModel.DataMode.ModeUser um = new IsEnergyModel.DataMode.ModeUser();
            IsEnergyModel.ResultMode result = new ResultMode();
            result = um.ChangeOrganization(user);
                if (result.Executed)
                {
                    return RedirectToAction("Index", "Home");
                }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
        }



    }
}