using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsEnergyModel.DataMode
{
    public class ModePhysicalPersons
    {
        public ResultMode Create(string userEditLogin,PhysicalPersons physicalPersons, string login, string pass, Address address, byte[] byteCert = null)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                string IdentifierSubscriber = String.Format("{0}-{1}_", ConfigModel.idOperEDO, physicalPersons.INN).PadRight(46, '0');

                //добовляем адресс
                ModeAddress ma = new ModeAddress();
                ResultMode resultMA = ma.Create(address);
                if (!resultMA.Executed) return resultMA;
                physicalPersons.IdAddress = (resultMA.ObjectResult as Address).IdAddress;


                // создание абонента
                ModeSubscribers subscriber = new ModeSubscribers();
                ResultMode resultSubscriber = subscriber.Create(userEditLogin,IdentifierSubscriber, physicalPersons.ShortName, physicalPersons.Requisites, 3, address,
                    physicalPersons.FirstName, physicalPersons.MidleName, physicalPersons.LastName,physicalPersons.Email,physicalPersons.Phone,login, pass, byteCert);
                if (resultSubscriber.Executed)
                    physicalPersons.IdentifierSubscriber = (resultSubscriber.ObjectResult as Subscribers).IdentifierSubscriber;
                else return resultSubscriber;

                // создание Физ. лицо
                db.PhysicalPersons.Add(physicalPersons);
                db.SaveChanges();

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Физическое лицо успешно добавлено", ObjectResult = physicalPersons };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось добавить физическое лицо" }; }
        }

        public ResultMode Edit(string userEditLogin,PhysicalPersons physicalPersons,Address address)
        {
            try
            {
                physicalPersons.Address = null;
                Is_EnergyEntities db = new Is_EnergyEntities();
                db.Entry(physicalPersons).State = EntityState.Modified;

                //Изменяем адресс
                ModeAddress ma = new ModeAddress();
                ResultMode resultMA = ma.Edit(address);
                if (!resultMA.Executed) return resultMA;
                physicalPersons.IdAddress = (resultMA.ObjectResult as Address).IdAddress;

                // меняем абонента
                ModeSubscribers ms = new ModeSubscribers();
                ResultMode ResultMs = ms.Edit(userEditLogin,physicalPersons.IdentifierSubscriber, physicalPersons.ShortName, physicalPersons.Requisites, 3);
                if (!ResultMs.Executed) return ResultMs;

                db.SaveChanges();
                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Физическое лицо успешно изменено", ObjectResult = physicalPersons };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось изменить физическое лицо" }; }
        }

        public ResultMode Delete(int id)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();

                PhysicalPersons physicalPersons = db.PhysicalPersons.Find(id);

                //удаляем адресс
                ModeAddress ma = new ModeAddress();
                ResultMode resultMA = ma.Delete(physicalPersons.IdAddress.Value);
                if (!resultMA.Executed) return resultMA;

                // удаляем абонента
                ModeSubscribers ms = new ModeSubscribers();
                ResultMode ResultMs = ms.Delete(physicalPersons.IdentifierSubscriber);
                if (!ResultMs.Executed) return ResultMs;

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Физическое лицо удалено" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось удалить физическое лицо" }; }
        }


    }
}
