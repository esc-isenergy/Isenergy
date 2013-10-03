using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsEnergyModel.DataMode
{
    public class ModeLegalEntities
    {

        public ResultMode Create(string userEditLogin,LegalEntities legalEntities, string login,  string pass, Address address, byte[] byteCert = null)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                string IdentifierSubscriber = String.Format("{0}-{1}_{2}_",ConfigModel.idOperEDO, legalEntities.INN, legalEntities.OGRN).PadRight(46, '0');

                //добовляем адресс
                ModeAddress ma = new ModeAddress();
                ResultMode resultMA = ma.Create(address);
                if (!resultMA.Executed) return resultMA;
                legalEntities.IdAddress = (resultMA.ObjectResult as Address).IdAddress;

                // создание абонента
                ModeSubscribers subscriber = new ModeSubscribers();
                ResultMode resultSubscriber = subscriber.Create(userEditLogin,IdentifierSubscriber, legalEntities.NameOrganization, legalEntities.Requisites, 1, address,
                    legalEntities.ContactFaceFirstName, legalEntities.ContactFaceMidleName, legalEntities.ContactFaceLastName, legalEntities.Email,legalEntities.Phone, login, pass, byteCert);
                if (resultSubscriber.Executed)
                    legalEntities.IdentifierSubscriber = (resultSubscriber.ObjectResult as Subscribers).IdentifierSubscriber;
                else return resultSubscriber;


                // создание Юр. лица
                db.LegalEntities.Add(legalEntities);
                db.SaveChanges();

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Юридическое лицо успешно добавлено", ObjectResult = legalEntities };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось добавить юридическое лицо" }; }
        }

        public ResultMode Edit(string userEditLogin,LegalEntities legalEntities, Address address)
        {
            try
            {
                legalEntities.Address = null;
                Is_EnergyEntities db = new Is_EnergyEntities();
                db.Entry(legalEntities).State = EntityState.Modified;

                //Изменяем адресс
                ModeAddress ma = new ModeAddress();
                ResultMode resultMA = ma.Edit(address);
                if (!resultMA.Executed) return resultMA;
                legalEntities.IdAddress = (resultMA.ObjectResult as Address).IdAddress;


                // меняем абонента
                ModeSubscribers ms = new ModeSubscribers();
                ResultMode ResultMs = ms.Edit(userEditLogin,legalEntities.IdentifierSubscriber, legalEntities.NameOrganization, legalEntities.Requisites, 1);
                if (!ResultMs.Executed) return ResultMs;

                db.SaveChanges();
                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Юридическое лицо успешно изменено", ObjectResult = legalEntities };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось изменить юридическое лицо" }; }
        }

        public ResultMode Delete(int id)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();

                LegalEntities legalEntities = db.LegalEntities.Find(id);

                //удаляем адресс
                ModeAddress ma = new ModeAddress();
                ResultMode resultMA = ma.Delete(legalEntities.IdAddress.Value);
                if (!resultMA.Executed) return resultMA;

                // удаляем абонента
                ModeSubscribers ms = new ModeSubscribers();
                ResultMode ResultMs = ms.Delete(legalEntities.IdentifierSubscriber);
                if (!ResultMs.Executed) return ResultMs;

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Юридическое лицо удалено" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось удалить юридическое лицо" }; }
        }

    }
}
