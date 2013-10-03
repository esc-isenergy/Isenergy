using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsEnergyModel.DataMode
{
    public class ModeAdministrators
    {
        public ResultMode Create(Administrators administrators)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                // создание Юр. лица
                db.Administrators.Add(administrators);
                db.SaveChanges();

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Администратор успешно добавлен", ObjectResult = administrators };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось добавить администратора" }; }
        }

        public ResultMode Edit(Administrators administrators)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                db.Entry(administrators).State = EntityState.Modified;
                db.SaveChanges();
                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Администратор успешно изменен", ObjectResult = administrators };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось изменить администратора" }; }
        }

        public ResultMode Delete(int id)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();

                Administrators administrators = db.Administrators.Find(id);
                db.Administrators.Remove(administrators);
                db.SaveChanges();

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Администратор удален" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось удалить администратора" }; }
        }


    }
}
