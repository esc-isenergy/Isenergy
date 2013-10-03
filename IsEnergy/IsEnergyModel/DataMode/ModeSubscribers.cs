using IsEnergyModel.Filters;
using IsEnergyModel.GlobalMetods;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace IsEnergyModel.DataMode
{
   public class ModeSubscribers
    {

        public ResultMode Create(string userEditLogin,string identifierSubscriber, string name, string requisites, int typeElementAbonent,Address address,
            String userFirstName, String userMidleName, String userLastName, String userEmail, String userPhone, String userLogin, string pass, byte[] byteCert = null)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                // создание абонента
                Subscribers subscriber = new Subscribers();
                subscriber.IdentifierSubscriber = identifierSubscriber;
                subscriber.Name = name;
                subscriber.Requisites = requisites;
                subscriber.TypeElementAbonent = typeElementAbonent;
                db.Subscribers.Add(subscriber);
                db.SaveChanges();

                // создание подразделение
                SubscribersSubdivision subdivision = new SubscribersSubdivision();
                subdivision.SubdivisionName = "Главное подразделение";
                subdivision.SubdivisionCode = "0001";
                subdivision.IdentifierSubscriber = identifierSubscriber;

                //создаем пользователя
                Users user = new Users();
                user.Email = userEmail;
                user.Phone = userPhone;
                user.FirstName = userFirstName;
                user.MidleName = userMidleName;
                user.LastName = userLastName;
                user.Login = userLogin;

                ResultMode resultMss = CreateSubdivision(userEditLogin,subdivision, address);
                if (!resultMss.Executed) return resultMss;

                //добовляем в список сотрудников
                ResultMode resultMsul = CreateNewUserList(userEditLogin,user, pass, (resultMss.ObjectResult as SubscribersSubdivision).idSubdivision, byteCert);
                if (!resultMsul.Executed) return resultMsul;


                ResultMode resultSH = SaveHistory(identifierSubscriber,userEditLogin,"Создание абонента");
                if (!resultSH.Executed) return resultSH;

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Абонент успешно добавлен", ObjectResult = subscriber };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось добавить абонента" }; }
        }

        public ResultMode Edit(string userEditLogin,string identifierSubscriber, string name, string requisites, int typeElementAbonent)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                Subscribers subscribers = db.Subscribers.Find(identifierSubscriber);
                db.Entry(subscribers).State = EntityState.Modified;
                subscribers.Name = name;
                subscribers.Requisites = requisites;
                subscribers.TypeElementAbonent = typeElementAbonent;
                db.SaveChanges();

                ResultMode resultSH = SaveHistory(identifierSubscriber, userEditLogin, "Редактирование данных абонента");
                if (!resultSH.Executed) return resultSH;

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Абонент успешно изменен", ObjectResult = subscribers };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось изменить абонента" }; }
        }

        public ResultMode Delete(string identifierSubscriber)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                Subscribers subscriber = db.Subscribers.Find(identifierSubscriber);
                db.Subscribers.Remove(subscriber);
                db.SaveChanges();

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Абонент удален" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось удалить абонента" }; }
        }

        //Subdivision
        public ResultMode CreateSubdivision(string userEditLogin, SubscribersSubdivision subscribersSubdivision, Address address)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();

                //добовляем адресс
                ModeAddress ma = new ModeAddress();
                ResultMode resultMA = ma.Create(address);
                if (!resultMA.Executed) return resultMA;
                subscribersSubdivision.IdAddress = (resultMA.ObjectResult as Address).IdAddress;

                db.SubscribersSubdivision.Add(subscribersSubdivision);
                db.SaveChanges();

                ResultMode resultSH = SaveHistory(subscribersSubdivision.IdentifierSubscriber, userEditLogin, "Создание подразделение " + subscribersSubdivision.SubdivisionName);
                if (!resultSH.Executed) return resultSH;

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Подразделение успешно добавлено", ObjectResult = subscribersSubdivision };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось добавить подразделение" }; }
        }

        public ResultMode EditSubdivision(string userEditLogin, IsEnergyModel.SubscribersSubdivision subscribersSubdivision, Address address)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                db.Entry(subscribersSubdivision).State = EntityState.Modified;
                
                //Изменим  адресс
                ModeAddress ma = new ModeAddress();
                ResultMode resultMsul = ma.Edit(address);
                if (!resultMsul.Executed) return resultMsul;
                subscribersSubdivision.IdAddress = (resultMsul.ObjectResult as Address).IdAddress;

                db.SaveChanges();

                ResultMode resultSH = SaveHistory(subscribersSubdivision.IdentifierSubscriber, userEditLogin, "Редактирование подразделение " + subscribersSubdivision.SubdivisionName);
                if (!resultSH.Executed) return resultSH;

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Подразделение успешно изменено", ObjectResult = subscribersSubdivision };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось изменить подразделение" }; }
        }

        public ResultMode DeleteSubdivision(string userEditLogin, int id)
        {
            try
            {
                if (id != 0)
                {
                    Is_EnergyEntities db = new Is_EnergyEntities();
                    SubscribersSubdivision subscribersSubdivision = db.SubscribersSubdivision.Find(id);

                    //удаляем дочки и его
                    ResultMode resultChildAndThis = DeleteSubdivisionChildAndThis(userEditLogin, subscribersSubdivision.idSubdivision, subscribersSubdivision.IdentifierSubscriber);
                    return resultChildAndThis;
                }
                return new ResultMode() { Executed = false, StrError = "-", StrResult = "Не удалось удалить подразделение" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось удалить подразделение" };}
        }

        public ResultMode DeleteSubdivisionChildAndThis(string userEditLogin, int id, string identifierSubscriber)
        {
            try
            {
                if (id != 0)
                {
                    Is_EnergyEntities db = new Is_EnergyEntities();
                    SubscribersSubdivision subscribersSubdivision = db.SubscribersSubdivision.Find(id);
                    if (subscribersSubdivision.IdentifierSubscriber == identifierSubscriber)
                    {
                        //удаляем дочки

                        IQueryable<SubscribersSubdivision> subscribersSubdivisionChildList = db.SubscribersSubdivision.Where(u => u.IdParent == id);
                        foreach (SubscribersSubdivision subscribersSubdivisionChild in subscribersSubdivisionChildList)
                        {
                            ResultMode resultChild = DeleteSubdivisionChildAndThis(userEditLogin, subscribersSubdivisionChild.idSubdivision, identifierSubscriber);
                            if (!resultChild.Executed) return resultChild;
                        }


                        string subdivisionName = subscribersSubdivision.SubdivisionName;
                        //Изменим  адресс
                        ModeAddress ma = new ModeAddress();
                        ResultMode resultMA = ma.Delete(subscribersSubdivision.IdAddress.Value);
                        if (!resultMA.Executed) return resultMA;

                        db.SubscribersSubdivision.Remove(subscribersSubdivision);
                        db.SaveChanges();

                        ResultMode resultSH = SaveHistory(identifierSubscriber, userEditLogin, "Удаление подразделения " + subdivisionName);
                        if (!resultSH.Executed) return resultSH;

                        return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Подразделение удалено" };
                    }
                    return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Не удалось удалить подразделение, не совпал идентификатор абонента" };
                }
                return new ResultMode() { Executed = false, StrError = "-", StrResult = "Не удалось удалить подразделение" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось удалить подразделение" }; }
        }

        //UserList
        public ResultMode CreateUserList(string userEditLogin, SubscribersUserList subscriberUserList)
        {
            try
            {
                
                Is_EnergyEntities db = new Is_EnergyEntities();
                
                SubscribersUserList subscribersUserList = new SubscribersUserList();
                // если он один
                if (db.SubscribersUserList.FirstOrDefault(s => s.IdSubdivision == subscriberUserList.IdSubdivision) == null) subscribersUserList.Active = true;
                subscribersUserList.IdSubdivision = subscriberUserList.IdSubdivision;
                subscribersUserList.IdUser = subscriberUserList.IdUser;
                db.SubscribersUserList.Add(subscribersUserList);
                db.SaveChanges();

                SubscribersSubdivision subscribersSubdivision = db.SubscribersSubdivision.Find(subscriberUserList.IdSubdivision);
                Users user = db.Users.Find(subscriberUserList.IdUser);

                ResultMode resultSH = SaveHistory(subscribersSubdivision.IdentifierSubscriber, userEditLogin, "Добавление Сотрудника " + user.ShortName);
                if (!resultSH.Executed) return resultSH;

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Сотрудник успешно добавлен", ObjectResult = subscribersUserList };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось добавить сотрудника" }; }
        }

        public ResultMode CreateNewUserList(string userEditLogin,Users user, string pass, int idSubscribersSubdivision, byte[] byteCert = null)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                SubscribersSubdivision subscribersSubdivision = db.SubscribersSubdivision.Find(idSubscribersSubdivision);

                //добовляем пользователя
                ModeUser mu = new ModeUser();
                ResultMode resultMu = mu.Create(user, pass, byteCert);
                if (!resultMu.Executed) return resultMu;

                SubscribersUserList subscribersUserList = new SubscribersUserList();
                // если он один
                if (db.SubscribersUserList.FirstOrDefault(s => s.IdSubdivision == idSubscribersSubdivision) == null) subscribersUserList.Active = true;
                subscribersUserList.IdSubdivision = idSubscribersSubdivision;
                subscribersUserList.IdUser = (resultMu.ObjectResult as Users).IdUser;
                db.SubscribersUserList.Add(subscribersUserList);
                db.SaveChanges();

                ResultMode resultSH = SaveHistory(subscribersSubdivision.IdentifierSubscriber, userEditLogin, "Добавление Сотрудника " + user.ShortName);
                if (!resultSH.Executed) return resultSH;

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Сотрудник успешно добавлен", ObjectResult = subscribersUserList };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось добавить сотрудника" }; }
        }

        public ResultMode EditUserList(string userEditLogin, IsEnergyModel.SubscribersUserList subscribersUserList)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();

                SubscribersSubdivision subscribersSubdivision = db.SubscribersSubdivision.Find(subscribersUserList.IdSubdivision);
                Users user = db.Users.Find(subscribersUserList.IdUser);

                db.Entry(subscribersUserList).State = EntityState.Modified;
                db.SaveChanges();

                ResultMode resultSH = SaveHistory(subscribersSubdivision.IdentifierSubscriber, userEditLogin, "Редактирование Сотрудника " + user.ShortName);
                if (!resultSH.Executed) return resultSH;

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Сотрудник успешно изменен", ObjectResult = subscribersUserList };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось изменить сотрудника" }; }
        }

        public ResultMode DeleteUserList(string userEditLogin, int id)
        {
            try
            {

                Is_EnergyEntities db = new Is_EnergyEntities();
                SubscribersUserList subscribersUserList = db.SubscribersUserList.Find(id);

                SubscribersSubdivision subscribersSubdivision = db.SubscribersSubdivision.Find(subscribersUserList.IdSubdivision);
                Users user = db.Users.Find(subscribersUserList.IdUser);
                string identifierSubscriber = subscribersSubdivision.IdentifierSubscriber;

                db.SubscribersUserList.Remove(subscribersUserList);
                db.SaveChanges();

                ResultMode resultSH = SaveHistory(identifierSubscriber, userEditLogin, "Удаление Сотрудника " + user.ShortName);
                if (!resultSH.Executed) return resultSH;

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Сотрудник удален" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось удалить сотрудника" }; }
        }

        public ResultMode AppointChiefUser(string userEditLogin, int idsubscribersUserList)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();

                SubscribersUserList subscribersUserList = db.SubscribersUserList.Find(idsubscribersUserList);
                SubscribersSubdivision subscribersSubdivision = db.SubscribersSubdivision.Find(subscribersUserList.IdSubdivision);
                Users user = db.Users.Find(subscribersUserList.IdUser);

                db.Entry(subscribersUserList).State = EntityState.Modified;
                List<SubscribersUserList> userListAll = db.SubscribersUserList.Where(s => s.IdSubdivision == subscribersUserList.IdSubdivision).ToList();
                foreach (SubscribersUserList userListOne in userListAll)
                {
                    userListOne.Active = false;
                }
                subscribersUserList.Active = true;
                db.SaveChanges();

                ResultMode resultSH = SaveHistory(subscribersSubdivision.IdentifierSubscriber, userEditLogin, "Назначени сотрудника главным " + user.ShortName);
                if (!resultSH.Executed) return resultSH;

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Сотрудник успешно назначен главным", ObjectResult = subscribersUserList };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось назначить главным сотрудника" }; }
        }

       //Service
        public ResultMode SaveService(string userEditLogin, Subscribers subscribersNew)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                Subscribers subscribers = db.Subscribers.Find(subscribersNew.IdentifierSubscriber);
                db.Entry(subscribers).State = EntityState.Modified;

                subscribers.AccessFormalizedDocuments = subscribersNew.AccessFormalizedDocuments;
                subscribers.AccessFormalizedDocumentsAmount = subscribersNew.AccessFormalizedDocumentsAmount;
                subscribers.AccessFormalizedDocumentsDateEnd = subscribersNew.AccessFormalizedDocumentsDateEnd;
                subscribers.AccessFormalizedDocumentsDateStart = subscribersNew.AccessFormalizedDocumentsDateStart;
                subscribers.AccessUnformalizedDocuments = subscribersNew.AccessUnformalizedDocuments;
                subscribers.AccessUnformalizedDocumentsAmount = subscribersNew.AccessUnformalizedDocumentsAmount;
                subscribers.AccessUnformalizedDocumentsDateEnd = subscribersNew.AccessUnformalizedDocumentsDateEnd;
                subscribers.AccessUnformalizedDocumentsDateStart = subscribersNew.AccessUnformalizedDocumentsDateStart;

                subscribers.TypeGroupElectricity = subscribersNew.TypeGroupElectricity;
                if (subscribers.TypeGroupElectricity == 1)
                {
                    subscribers.AccessElectricityForm16 = subscribersNew.AccessElectricityForm16;
                    subscribers.AccessElectricityForm16Amount = subscribersNew.AccessElectricityForm16Amount;
                    subscribers.AccessElectricityForm16DateEnd = subscribersNew.AccessElectricityForm16DateEnd;
                    subscribers.AccessElectricityForm16DateStart = subscribersNew.AccessElectricityForm16DateStart;
                    subscribers.AccessElectricityForm80020 = subscribersNew.AccessElectricityForm80020;
                    subscribers.AccessElectricityForm80020Amount = subscribersNew.AccessElectricityForm80020Amount;
                    subscribers.AccessElectricityForm80020DateEnd = subscribersNew.AccessElectricityForm80020DateEnd;
                    subscribers.AccessElectricityForm80020DateStart = subscribersNew.AccessElectricityForm80020DateStart;
                    subscribers.AccessElectricityPrimaryDocuments = subscribersNew.AccessElectricityPrimaryDocuments;
                    subscribers.AccessElectricityPrimaryDocumentsDateEnd = subscribersNew.AccessElectricityPrimaryDocumentsDateEnd;
                    subscribers.AccessElectricityPrimaryDocumentsDateStart = subscribersNew.AccessElectricityPrimaryDocumentsDateStart;
                }
                else
                {
                    subscribers.AccessElectricityForm16 = null;
                    subscribers.AccessElectricityForm16Amount = null;
                    subscribers.AccessElectricityForm16DateEnd = null;
                    subscribers.AccessElectricityForm16DateStart = null;
                    subscribers.AccessElectricityForm80020 = null;
                    subscribers.AccessElectricityForm80020Amount = null;
                    subscribers.AccessElectricityForm80020DateEnd = null;
                    subscribers.AccessElectricityForm80020DateStart = null;
                    subscribers.AccessElectricityPrimaryDocuments = null;
                    subscribers.AccessElectricityPrimaryDocumentsDateEnd = null;
                    subscribers.AccessElectricityPrimaryDocumentsDateStart = null;
                }
                


                db.SaveChanges();

                ResultMode resultSH = SaveHistory(subscribers.IdentifierSubscriber, userEditLogin, "Изменения сервисов");
                if (!resultSH.Executed) return resultSH;

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Сервисы успешно изменен", ObjectResult = subscribers };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось изменить сервисы" }; }
        }

        public ResultMode AddContract(string userEditLogin, SubscribersContracts subscribersContracts)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                db.SubscribersContracts.Add(subscribersContracts);
                db.SaveChanges();

                ResultMode resultSH = SaveHistory(subscribersContracts.IdentifierSubscriber, userEditLogin, "Добавление договора: " + subscribersContracts.NumberContract);
                if (!resultSH.Executed) return resultSH;

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Договор успешно добавлен", ObjectResult = subscribersContracts };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось добавить договор" }; }

        }

        public ResultMode DelContract(string userEditLogin, int idSubscribersContracts)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                SubscribersContracts subscribersContracts = db.SubscribersContracts.Find(idSubscribersContracts);
                string identifierSubscriber = subscribersContracts.IdentifierSubscriber;
                string numberContract = subscribersContracts.NumberContract;

                db.SubscribersContracts.Remove(subscribersContracts);
                db.SaveChanges();

                ResultMode resultSH = SaveHistory(identifierSubscriber, userEditLogin, "Удаление договора: " + numberContract);
                if (!resultSH.Executed) return resultSH;

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Договор успешно удален" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось удалить договор" }; }

        }

        public ResultMode AddEnumerator(string userEditLogin, SubscribersEnumerator subscribersEnumerator)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                db.SubscribersEnumerator.Add(subscribersEnumerator);
                db.SaveChanges();

                SubscribersContracts subscribersContracts = db.SubscribersContracts.Find(subscribersEnumerator.IdContractSubscriber);
                string identifierSubscriber = subscribersContracts.IdentifierSubscriber;

                ResultMode resultSH = SaveHistory(identifierSubscriber, userEditLogin, "Добавление счётчика: " + subscribersEnumerator.Code);
                if (!resultSH.Executed) return resultSH;

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Счётчик успешно добавлен", ObjectResult = subscribersEnumerator };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось добавить счётчик" }; }

        }

        public ResultMode DelEnumerator(string userEditLogin, int idEnumerator)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                SubscribersEnumerator subscribersEnumerator = db.SubscribersEnumerator.Find(idEnumerator);
                string identifierSubscriber = subscribersEnumerator.SubscribersContracts.IdentifierSubscriber;
                string code = subscribersEnumerator.Code;
                db.SubscribersEnumerator.Remove(subscribersEnumerator);
                db.SaveChanges();

                ResultMode resultSH = SaveHistory(identifierSubscriber, userEditLogin, "Удаление счётчика: " + code);
                if (!resultSH.Executed) return resultSH;

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Счётчик успешно удален" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось удалить счётчик" }; }

        }

       //History
        public ResultMode SaveHistory(string identifierSubscriber,string userEditLogin,string textEdit)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();

                Users userEdit =AuthorizeMeAll.GetCurrentUser(userEditLogin);
                Subscribers subscriberEdit =AuthorizeMeAll.GetCurrentSubscriber(userEdit);

                SubscribesLog subscribeLog = new SubscribesLog();
                subscribeLog.IdentifierSubscriber = identifierSubscriber;
                subscribeLog.NameSubscriberEditor = subscriberEdit.Name;
                subscribeLog.IdUserEditor = userEdit.IdUser;
                subscribeLog.NameUserEditor = userEdit.ShortName;
                subscribeLog.IdentifierSubscriberEditor = subscriberEdit.IdentifierSubscriber;
                subscribeLog.DateEdit = CommonMethods.GetDataTimeNow;
                subscribeLog.TextEdit = textEdit;
                db.SubscribesLog.Add(subscribeLog);
                db.SaveChanges();
                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "История успешно добавлена", ObjectResult = subscribeLog };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось добавить историю" }; }
        }

    }
}
