using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsEnergyModel.DataMode
{
    public class ModeContractors
    {
        public ResultMode AddContractors(string userName, string IdentifierSubscriberContractor, string invitationText)
        {
            try
            {
                Subscribers subThis = Filters.AuthorizeMeAll.GetCurrentSubscriber(userName);
                if (subThis != null)
                {
                    Is_EnergyEntities db = new Is_EnergyEntities();
                    Contractors contractors = new Contractors();
                    contractors.IdentifierSubscriber = subThis.IdentifierSubscriber;
                    contractors.IdentifierSubscriberContractor = IdentifierSubscriberContractor;
                    contractors.InvitationText = invitationText;
                    contractors.State = 0;
                    db.Contractors.Add(contractors);
                    db.SaveChanges();
                    return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Контрагент добавлен" };
                }
                return new ResultMode() { Executed = false, StrError = string.Empty, StrResult = "Не найден текущий абонент" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось добавить контрагента" }; }
        }

        public ResultMode ApplyContractors(int idContractor)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                Contractors contractors = db.Contractors.Find(idContractor);
                db.Entry(contractors).State = EntityState.Modified;
                contractors.State =1;
                db.SaveChanges();

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Контрагент принят" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось принять контрагента" }; }

        }

        public ResultMode CancelContractors(int idContractor)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                Contractors contractors = db.Contractors.Find(idContractor);
                db.Contractors.Remove(contractors);
                db.SaveChanges();

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Контрагент удален" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось удалить контрагента" }; }
        }

        public ResultMode FailContractors(int idContractor)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                Contractors contractors = db.Contractors.Find(idContractor);
                db.Contractors.Remove(contractors);
                db.SaveChanges();

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Контрагент удален" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось удалить контрагента" }; }
        }

        public ResultMode DelContractors(int idContractor)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                Contractors contractors = db.Contractors.Find(idContractor);
                db.Contractors.Remove(contractors);
                db.SaveChanges();

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Контрагент удален" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось удалить контрагента" }; }
        }

    }
}
