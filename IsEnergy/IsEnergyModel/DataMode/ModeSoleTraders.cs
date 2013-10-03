using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsEnergyModel.DataMode
{
    public class ModeSoleTraders
    {
        public ResultMode Create(string userEditLogin, SoleTraders soleTraders, string login, string pass, Address address, byte[] byteCert = null)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                string IdentifierSubscriber = String.Format("{0}-{1}_{2}_",ConfigModel.idOperEDO, soleTraders.INN, soleTraders.OGRN).PadRight(46, '0');

                //добовляем адресс
                ModeAddress ma = new ModeAddress();
                ResultMode resultMA = ma.Create(address);
                if (!resultMA.Executed) return resultMA;
                soleTraders.IdAddress = (resultMA.ObjectResult as Address).IdAddress;

                // создание абонента
                ModeSubscribers subscriber = new ModeSubscribers();
                ResultMode resultSubscriber = subscriber.Create(userEditLogin, IdentifierSubscriber, soleTraders.ShortName, soleTraders.Requisites, 2, address,
                    soleTraders.ContactFaceFirstName, soleTraders.ContactFaceMidleName, soleTraders.ContactFaceLastName, soleTraders.Email, soleTraders.Phone,login, pass, byteCert);
                if (resultSubscriber.Executed)
                    soleTraders.IdentifierSubscriber = (resultSubscriber.ObjectResult as Subscribers).IdentifierSubscriber;
                else return resultSubscriber;

                // создание Юр. лица
                db.SoleTraders.Add(soleTraders);
                db.SaveChanges();

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Индивидуальный предприниматель успешно добавлен", ObjectResult = soleTraders };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось добавить индивидуального предпринимателя" }; }
        }

        public ResultMode Edit(string userEditLogin,SoleTraders soleTraders,Address address)
        {
            try
            {
                soleTraders.Address = null;

                Is_EnergyEntities db = new Is_EnergyEntities();
                db.Entry(soleTraders).State = EntityState.Modified;

                //Изменяем адресс
                ModeAddress ma = new ModeAddress();
                ResultMode resultMA = ma.Edit(address);
                if (!resultMA.Executed) return resultMA;
                soleTraders.IdAddress = (resultMA.ObjectResult as Address).IdAddress;
                // меняем абонента
                ModeSubscribers ms = new ModeSubscribers();
                ResultMode ResultMs = ms.Edit(userEditLogin,soleTraders.IdentifierSubscriber, soleTraders.ShortName, soleTraders.Requisites, 2);
                if (!ResultMs.Executed) return ResultMs;

                db.SaveChanges();
                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Индивидуальный предприниматель успешно изменено", ObjectResult = soleTraders };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось изменить индивидуального предпринимателя" }; }
        }

        public ResultMode Delete(int id)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();

                SoleTraders soleTraders = db.SoleTraders.Find(id);

                //удаляем адресс
                ModeAddress ma = new ModeAddress();
                ResultMode resultMA = ma.Delete(soleTraders.IdAddress.Value);
                if (!resultMA.Executed) return resultMA;

                // удаляем абонента
                ModeSubscribers ms = new ModeSubscribers();
                ResultMode ResultMs = ms.Delete(soleTraders.IdentifierSubscriber);
                if (!ResultMs.Executed) return ResultMs;

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Индивидуальный предприниматель удалено" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось удалить индивидуального предпринимателя" }; }
        }


    }
}
