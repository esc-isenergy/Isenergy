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
    //[Authorize]
    public class SubscriberController : Controller
    {
        //
        // GET: /Subscriber/

        IsEnergyModel.Is_EnergyEntities db = new IsEnergyModel.Is_EnergyEntities();
        IsEnergyModel.DataMode.ModeSubscribers subs = new IsEnergyModel.DataMode.ModeSubscribers();

        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult GridViewPartial()
        {
            var model = db.Subscribers;
            return PartialView("_GridViewPartial", model.ToList());
        }

        public ActionResult SuperMegaRedirectToAction(Subscribers subscribers)
        {
            if (subscribers.TypeElementAbonent == 1)
            {
                LegalEntities leg = db.LegalEntities.FirstOrDefault(m => m.IdentifierSubscriber == subscribers.IdentifierSubscriber);
                return RedirectToAction("Details", "LegalEntities", new { id = leg.IdOrganization });
            }
            else if (subscribers.TypeElementAbonent == 2)
            {
                SoleTraders soleTr = db.SoleTraders.FirstOrDefault(m => m.IdentifierSubscriber == subscribers.IdentifierSubscriber);
                return RedirectToAction("Details", "SoleTraders", new { id = soleTr.IdSoleTrader });
            }
            else if (subscribers.TypeElementAbonent == 3)
            {
                PhysicalPersons phys = db.PhysicalPersons.FirstOrDefault(m => m.IdentifierSubscriber == subscribers.IdentifierSubscriber);
                return RedirectToAction("Details", "PhysicalPersons", new { id = phys.IdPhysicalPerson });
            }
            else
                return RedirectToAction("Error", "Error", new { StrError = "Ошибка", StrResult = "Ссылка не работает" });

        }


        //Сервисы
        public ActionResult Changes(string IdentifierSubscriber)
        {
            Subscribers subscriber = db.Subscribers.Find(IdentifierSubscriber);

            if (subscriber != null) return PartialView("Access/_AccessPartial", subscriber);
            else return RedirectToAction("Error", "Error", new { StrError = "Ошибка", StrResult = "Ссылка не работает" });
        }
        
        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        [HttpPost]
        public ActionResult SaveChanges(Subscribers subscribers)
        {
            IsEnergyModel.ResultMode result = new ResultMode();
            result = subs.SaveService(User.Identity.Name, subscribers);
            if (!result.Executed)
            {
                return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
            }
            return SuperMegaRedirectToAction(subscribers);

        }

        //История
        [Authorize]
        public ActionResult History(string IdentifierSubscriber)
        {
            ViewData["IdentifierSubscriber"] = IdentifierSubscriber;
            var subscribeLog = db.SubscribesLog.Where(u => u.IdentifierSubscriber == IdentifierSubscriber).ToList();
            if (subscribeLog != null) return PartialView("_GridViewHistoryPartial", subscribeLog);
            else return RedirectToAction("Error", "Error", new { StrError = "Ошибка", StrResult = "Ссылка не работает" });
        }

        //Сотрудники
        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators,AppointChiefSubscriber")]
        public ActionResult CoWorker(string IdentifierSubscriber)
        {
            ViewData["IdentifierSubscriber"] = IdentifierSubscriber;
            return PartialView("CoWorker/CoWorkers");
        }

        [ValidateInput(false)]//Список сотрудников
        public ActionResult GridUserList(string IdentifierSubscriber)
        {
            ViewData["IdentifierSubscriber"] = IdentifierSubscriber;
            List<SubscribersUserList> model = new List<SubscribersUserList>();
            ICollection<SubscribersSubdivision> subscribersSubdivisionCol = db.Subscribers.Find(IdentifierSubscriber).SubscribersSubdivision;
            foreach (SubscribersSubdivision subscribersSubdivision in subscribersSubdivisionCol)
            {
                model.AddRange(db.SubscribersUserList.Where(s => s.IdSubdivision == subscribersSubdivision.idSubdivision));
            }
            return PartialView("CoWorker/_GridSubscriberUserList", model);
        }

        //Добавление пользователя в подразделение
        public ActionResult AddUserStart(string IdentifierSubscriber)
        {
            ViewData["Users"] = db.Users.ToList();
            ViewData["Subdivision"] = db.SubscribersSubdivision.Where(s => s.IdentifierSubscriber == IdentifierSubscriber).ToList();
            ViewData["IdentifierSubscriber"] = IdentifierSubscriber;
            return PartialView("CoWorker/AddUser");
        }

        //[IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        [HttpPost]
        public ActionResult AddUser(SubscribersUserList subscribersUserList, string IdentifierSubscriber)
        {
            subscribersUserList.IdUser = ComboBoxExtension.GetValue<int>("IdUserAddUser");
            subscribersUserList.IdSubdivision = ComboBoxExtension.GetValue<int>("IdSubdivisionAddUser");

            IsEnergyModel.ResultMode result = new ResultMode();
            result = subs.CreateUserList(User.Identity.Name, subscribersUserList);
            if (!result.Executed)
            {
                return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
            }
            return RedirectToAction("GridUserList", new { IdentifierSubscriber = IdentifierSubscriber });
        }

        //Создать пользователя в подразделении
        public ActionResult AddNewUserForSubStart(string IdentifierSubscriber)
        {
            ViewData["Subdivision"] = db.SubscribersSubdivision.Where(s => s.IdentifierSubscriber == IdentifierSubscriber).ToList();
            ViewData["IdentifierSubscriber"] = IdentifierSubscriber;
            return PartialView("CoWorker/AddNewUserForSub");
        }
       // [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators,AppointChiefSubscriber")]
        [HttpPost]
        public ActionResult AddNewUserForSub(SubscribersUserList subscribersUserList, string IdentifierSubscriber)
        {
            IsEnergyModel.ResultMode result = new ResultMode();
            string pass = TextBoxExtension.GetValue<string>("Users_Password");
            result = subs.CreateNewUserList(User.Identity.Name, subscribersUserList.Users, pass, subscribersUserList.IdSubdivision, AllMode.ControllersMode.UploadGetByte("fileUploadCreat"));
            if (!result.Executed)
            {
                return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
            }
            return RedirectToAction("GridUserList", new { IdentifierSubscriber = IdentifierSubscriber });
        }

        //[IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators,AppointChiefSubscriber")]
        public string DetailsFromUserList(int id = 0)
        {
            SubscribersUserList subscribersUserList = db.SubscribersUserList.Find(id);

            if (subscribersUserList == null)
            {
                return "HttpNotFound";
            }
            string str = String.Format("/Users/Details/{0}", subscribersUserList.IdUser);
            return str;
        }

        //Убрать из списка пользователей для подразделений
        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators,AppointChiefSubscriber")]
        [Authorize]
        public ActionResult DeleteFromUserList(int id = 0, string IdentifierSubscriber = null)
        {
            IsEnergyModel.ResultMode result = new ResultMode();
            result = subs.DeleteUserList(User.Identity.Name, id);
            if (!result.Executed)
            {
                return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
            }
            return RedirectToAction("GridUserList", new { IdentifierSubscriber = IdentifierSubscriber });
        }

        //Назначить главным.
       [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators,AppointChiefSubscriber")]
        [Authorize]      
        public ActionResult AppointChiefUser(int id = 0, string IdentifierSubscriber = null)
        {
            IsEnergyModel.ResultMode result = new ResultMode();

            result = subs.AppointChiefUser(User.Identity.Name, id);
            if (!result.Executed)
            {
                return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });
            }
            return RedirectToAction("GridUserList", new { IdentifierSubscriber = IdentifierSubscriber });
        }

        //Подразделения
        [ValidateInput(false)]
        public ActionResult TreeListSubdivision(string identifierSubscriber)
        {
            ViewData["IdentifierSubscriber"] = identifierSubscriber;
            var model = db.SubscribersSubdivision.Where(u => u.IdentifierSubscriber == identifierSubscriber);
            return PartialView("Subdivision/_TreeListSubdivision", model.ToList());

        }

        public ActionResult AddSubdivisionRequisiteView(string identifierSubscriber)
        {
            var SubdList = db.SubscribersSubdivision.Where(u => u.IdentifierSubscriber == identifierSubscriber);
            ViewData["Subdivision"] = SubdList.ToList();
            return PartialView("~/Views/Subdivision/Requisite.cshtml");

        }

        public ActionResult AddSubdivisionAddressView(string identifierSubscriber)
        {

            ViewData["Countries"] = db.Countries.ToList();
            ViewData["Regions"] = db.Regions.ToList();
            return PartialView("~/Views/Address/AddressSubdivision.cshtml");
        }
        
        public ActionResult EditSubdivisionRequisiteView(int id)
        {

            var Subdivision = db.SubscribersSubdivision.Find(id);
            var SubdList = db.SubscribersSubdivision.Where(u => u.IdentifierSubscriber == Subdivision.Subscribers.IdentifierSubscriber && u.idSubdivision != id);
            ViewBag.RequisiteSub = Subdivision;
            ViewData["Subdivision"] = SubdList.ToList();
            return PartialView("~/Views/Subdivision/Requisite.cshtml");
        }

        public ActionResult EditSubdivisionAddressView(int id)
        {
            var Subdivision = db.SubscribersSubdivision.Find(id);
            ViewBag.SubdivisionAddress = Subdivision.Address;
            ViewData["Regions"] = db.Regions.ToList();
            ViewData["Countries"] = db.Countries.ToList();
            return PartialView("~/Views/Address/AddressSubdivision.cshtml");
        }

        //[IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators,AppointChiefSubscriber")]
        [Authorize]
        public ActionResult AddSubdivision(SubscribersSubdivision RequisiteSub, Address AddressSubdivision, string IdentifierSubscriber)
        {
            IsEnergyModel.ResultMode result = new ResultMode();
            RequisiteSub.IdentifierSubscriber = IdentifierSubscriber;
            result = subs.CreateSubdivision(User.Identity.Name, RequisiteSub, AddressSubdivision);
            if (result.Executed)
            {
                var model = db.SubscribersSubdivision.Where(u => u.IdentifierSubscriber == IdentifierSubscriber);
                ViewData["IdentifierSubscriber"] = IdentifierSubscriber;
                return PartialView("~/Views/Subscriber/Subdivision/_TreeListSubdivision.cshtml", model.ToList());
            }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });

        }

        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators,AppointChiefSubscriber")]
        [Authorize]
        public ActionResult EditSubdivision(SubscribersSubdivision RequisiteSub, Address AddressSubdivision, string IdentifierSubscriber)
        {
            IsEnergyModel.ResultMode result = new ResultMode();
            RequisiteSub.IdentifierSubscriber = IdentifierSubscriber;
            result = subs.EditSubdivision(User.Identity.Name, RequisiteSub, AddressSubdivision);//User.Identity.Name, RequisiteSub, AddressSubdivision
            if (result.Executed)
            {
                var model = db.SubscribersSubdivision.Where(u => u.IdentifierSubscriber == IdentifierSubscriber);
                ViewData["IdentifierSubscriber"] = IdentifierSubscriber;
                return PartialView("~/Views/Subscriber/Subdivision/_TreeListSubdivision.cshtml", model.ToList());
            }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });

        }

        //[IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators,AppointChiefSubscriber")]
        [Authorize]
        public ActionResult DeleteSubdivision(string IdentifierSubscriber, int id)
        {
            IsEnergyModel.ResultMode result = new ResultMode();

            result = subs.DeleteSubdivision(User.Identity.Name, id);
            if (result.Executed)
            {
                var model = db.SubscribersSubdivision.Where(u => u.IdentifierSubscriber == IdentifierSubscriber);
                ViewData["IdentifierSubscriber"] = IdentifierSubscriber;
                return PartialView("~/Views/Subscriber/Subdivision/_TreeListSubdivision.cshtml", model.ToList());
            }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });

        }
        [ValidateInput(false)]
        public ActionResult GridViewPartialpcModalModeCreateContract(string IdentifierSubscriber)
        {
            var model = db.SubscribersContracts.Where(u => u.IdentifierSubscriber == IdentifierSubscriber);
            ViewData["IdentifierSubscriber"] = IdentifierSubscriber;
            return PartialView("Access/Consumer/_GridViewPartialpcModalModeCreateContract", model.ToList());
        }

      
       
        
        public ActionResult CreateContractStart(Subscribers subscribers)
        {
            SubscribersContracts subContract = new SubscribersContracts();
            subContract.IdentifierSubscriber = subscribers.IdentifierSubscriber;
            ViewBag.subscribersContracts = subContract;
            return PartialView("Access/Consumer/CreateContractSubscriberPartial", ViewBag.subscribersContracts);
        }

        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        public ActionResult CreateContractSubscriber(SubscribersContracts subscribersContracts)
        {
            ResultMode result = subs.AddContract(User.Identity.Name, subscribersContracts);
            if (result.Executed)
            {
                ViewData["Contracts"] = db.SubscribersContracts.Where(u => u.IdentifierSubscriber == subscribersContracts.IdentifierSubscriber).ToList();
                var model = db.SubscribersContracts.Where(u => u.IdentifierSubscriber == subscribersContracts.IdentifierSubscriber);
                ViewData["IdentifierSubscriber"] = subscribersContracts.IdentifierSubscriber;
                return PartialView("Access/Consumer/_GridViewPartialpcModalModeCreateContract", model.ToList());
                    
            }
            return RedirectToAction("Error", "Error", new { StrError = result.StrError, StrResult = result.StrResult });

        }

        public ActionResult CreateEnumeratorStart(string IdentifierSubscriber)
        {
            Subscribers subscribers = db.Subscribers.Find(IdentifierSubscriber);
            SubscribersEnumerator subEnumer = new SubscribersEnumerator();
            ViewBag.subscribersEnumerator = subEnumer;
            ViewData["Contracts"] = db.SubscribersContracts.Where(u => u.IdentifierSubscriber == subscribers.IdentifierSubscriber).ToList();
            return PartialView("Access/Consumer/CreateEnumeratorSubscriberPartial", ViewBag.subscribersEnumerator);
        }

        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        [HttpPost]
        public ActionResult CreateEnumeratorSubscriber(SubscribersEnumerator subscribersEnumerator)
        {
                IsEnergyModel.ResultMode result = new ResultMode();
                result = subs.AddEnumerator(User.Identity.Name, subscribersEnumerator);
                if (result.Executed)
                {
                    var model = db.SubscribersEnumerator.Where(u => u.SubscribersContracts.IdentifierSubscriber == subscribersEnumerator.SubscribersContracts.IdentifierSubscriber);
                    ViewData["subscribersEnumerator"] = subscribersEnumerator.SubscribersContracts.IdentifierSubscriber;
                    return PartialView("Access/Consumer/_GridViewPartialpcModalModeCreateEnumerator", model.ToList());
                    //return RedirectToAction("GridViewPartialpcModalModeCreateEnumerator", new { IdentifierSubscriber = subscribersEnumerator.SubscribersContracts.IdentifierSubscriber });

                }
                return RedirectToAction("Error", "Error", new { StrError = "Ошибка", StrResult = "Что то пошло не так" });

        }

        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        public ActionResult ContractDelete(SubscribersContracts subscribersContracts)
        {
            SubscribersContracts sub = db.SubscribersContracts.Find(subscribersContracts.IdContractSubscriber);
                IsEnergyModel.ResultMode result = new ResultMode();
                string ident = sub.IdentifierSubscriber;
                result = subs.DelContract(User.Identity.Name,subscribersContracts.IdContractSubscriber);
                if (result.Executed)
                {
                    ViewData["Contracts"] = db.SubscribersContracts.Where(u => u.IdentifierSubscriber == ident).ToList();
                    var model = db.SubscribersContracts.Where(u => u.IdentifierSubscriber == ident);
                    ViewData["IdentifierSubscriber"] = ident;
                    return PartialView("Access/Consumer/_GridViewPartialpcModalModeCreateContract", model.ToList());

                }
                return  RedirectToAction("Error", "Error", new { StrError = "Ошибка", StrResult = "Что то пошло не так" });

        }

        [IsEnergyModel.Filters.AuthorizeMe(Groups = "Administrators,Operators")]
        public ActionResult EnumeratorDelete(SubscribersEnumerator subscribersEnumerator)
        {
            SubscribersEnumerator sub = db.SubscribersEnumerator.Find(subscribersEnumerator.IdEnumerator);
                IsEnergyModel.ResultMode result = new ResultMode();
                string ident = sub.SubscribersContracts.IdentifierSubscriber;
                result = subs.DelEnumerator(User.Identity.Name, subscribersEnumerator.IdEnumerator);
                if (result.Executed)
                {
                    var model = db.SubscribersEnumerator.Where(u => u.SubscribersContracts.IdentifierSubscriber == ident);
                    return PartialView("Access/Consumer/_GridViewPartialpcModalModeCreateEnumerator", model.ToList());

                }
                return  RedirectToAction("Error", "Error", new { StrError = "Ошибка", StrResult = "Что то пошло не так" });

        }

        [ValidateInput(false)]
        public ActionResult TreeListEnergosbytOfSubscriberPartial()
        {
            var model = db.SubscribersSubdivision.Where(u => u.Subscribers.TypeGroupElectricity == 2);
            return PartialView("Access/Consumer/_TreeListEnergosbytOfSubscriberPartial", model.ToList());
        }
        [ValidateInput(false)]
        public ActionResult TreeListGridCompanyOfSubscriberPartial()
        {
            var model = db.SubscribersSubdivision.Where(u => u.Subscribers.TypeGroupElectricity == 3);
            return PartialView("Access/Consumer/_TreeListGridCompanyOfSubscriberPartial", model.ToList());
        }
        [ValidateInput(false)]
        public ActionResult GridViewPartialpcModalModeCreateEnumerator(string IdentifierSubscriber)
        {
            var model = db.SubscribersEnumerator.Where(u => u.SubscribersContracts.IdentifierSubscriber == IdentifierSubscriber);
            ViewData["subscribersEnumerator"] = IdentifierSubscriber;
            return PartialView("Access/Consumer/_GridViewPartialpcModalModeCreateEnumerator", model.ToList());
        }


        [ValidateInput(false)]
        public ActionResult GridViewPartialpcModalModeCreateEnumerator1(string IdentifierSubscriber)
        {
            var model = db.SubscribersEnumerator.Where(u => u.SubscribersContracts.IdentifierSubscriber == IdentifierSubscriber);
            ViewData["subscribersEnumerator"] = IdentifierSubscriber;
            return PartialView("PrivateOfficeSubscriber/_GridViewPartialpcModalModeCreateEnumerator1", model.ToList());
        }

          [ValidateInput(false)]
        public ActionResult GridViewPartialpcModalModeCreateContract1(string IdentifierSubscriber)
        {
            var model = db.SubscribersContracts.Where(u => u.IdentifierSubscriber == IdentifierSubscriber);
            ViewData["IdentifierSubscriber"] = IdentifierSubscriber;
            return PartialView("PrivateOfficeSubscriber/_GridViewPartialpcModalModeCreateContract1", model.ToList());
        }

        [Authorize]
        public ActionResult PrivateOfficeSubscriber()
        {
            try
            {
                ViewData["IFNS"] = db.IFNS.ToList();
                Users user = db.Users.First(s => s.Login == User.Identity.Name);
                Subscribers subscr = db.Subscribers.FirstOrDefault(y => y.IdentifierSubscriber == user.IdentifierSubscriberDefault);
                if (subscr != null)
                    return View("PrivateOfficeSubscriber/Details", subscr);
                else
                    return RedirectToAction("Error", "Error", new { StrError = "Ошибка", StrResult = "Абонент не выбран" });
            }
            catch
            {
                return RedirectToAction("Error", "Error", new { StrError = "Ошибка", StrResult = "Абонент некорректный" });
            }
        }        
    }
}
