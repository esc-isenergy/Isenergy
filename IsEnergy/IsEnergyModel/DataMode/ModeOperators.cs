using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsEnergyModel.DataMode
{
    public class ModeOperators
    {
        public ResultMode Create(Operators operators)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                // создание Юр. лица
                db.Operators.Add(operators);
                db.SaveChanges();

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Оператор успешно добавлен", ObjectResult = operators };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось добавить оператора" }; }
        }

        public ResultMode Edit(Operators operators)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                db.Entry(operators).State = EntityState.Modified;
                db.SaveChanges();
                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Оператор успешно изменен", ObjectResult = operators };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось изменить оператора" }; }
        }

        public ResultMode Delete(int id)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();

                Operators operators = db.Operators.Find(id);
                db.Operators.Remove(operators);
                db.SaveChanges();

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Оператор удален" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось удалить оператора" }; }
        }

    }
}
