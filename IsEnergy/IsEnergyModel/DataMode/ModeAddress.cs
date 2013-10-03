using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsEnergyModel.DataMode
{
    public class ModeAddress
    {
        public ResultMode Create(Address address)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                address = ComputeData(address);
                db.Address.Add(address);
                db.SaveChanges();
                // Добавим сертификат
                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Адрес успешно добавлен", ObjectResult = address };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось добавить адрес" }; }
        }

        public ResultMode Edit(Address address)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                if (address.IdAddress == 0) return Create(address);
                    db.Entry(address).State = EntityState.Modified;
                    address = ComputeData(address);
                    db.SaveChanges();
                
                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Адрес успешно изменен", ObjectResult = address };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось изменить адрес" }; }
        }

        public ResultMode Delete(int id)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                Address address = db.Address.Find(id);
                db.Address.Remove(address);
                db.SaveChanges();
                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Адрес удален" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось удалить адрес" }; }
        }

        private Address ComputeData(Address address)
        {
            Is_EnergyEntities db = new Is_EnergyEntities();
            if (address.TypeAddress == 0)
            {
                //чистим данные
                address.OtherCountry_Address = null;
                address.OtherCountry_IdCountry = null;

                string City = "", Street = "", Home = "", OfficeOrApartment = "", Housing = "";
                if (address.RF_City != null)
                {
                    City = "г.";
                }
                if (address.RF_Street != null)
                {
                    Street = "ул.";
                }
                if (address.RF_Home != null)
                {
                    Home = "д.";
                }
                if (address.RF_OfficeOrApartment != null)
                {
                    OfficeOrApartment = "кв./оффис";
                } if (address.RF_Housing != null)
                {
                    Housing = "корпус";
                }
                if (address.RF_IdRegion.HasValue)
                {
                    Regions regions = db.Regions.Find(address.RF_IdRegion.Value);
                    address.AddressShort = string.Format("{0} {7} {1} {2} {8} {3} {9} {4} {11} {5}  {10} {6} ", regions.Name,
                    address.RF_City, address.RF_localityCity,
                    address.RF_Street, address.RF_Home, address.RF_Housing,
                    address.RF_OfficeOrApartment, City, Street, Home, OfficeOrApartment, Housing);
                }
                else
                {
                    address.AddressShort = string.Format("{6} {0} {1} {7} {2} {8} {3} {10} {4}  {9} {5} ",
                       address.RF_City, address.RF_localityCity,
                       address.RF_Street, address.RF_Home, address.RF_Housing,
                       address.RF_OfficeOrApartment, City, Street, Home, OfficeOrApartment, Housing);
                }
            }
            else
            {
                //чистим данные
                address.RF_City = null;
                address.RF_Street = null;
                address.RF_Home = null;
                address.RF_OfficeOrApartment = null;
                address.RF_Housing = null;
                address.RF_IdRegion = null;
                address.RF_Area = null;
                address.RF_Index = null;
                address.RF_localityCity = null;

                if (address.OtherCountry_IdCountry.HasValue)
                {
                    Countries countries = db.Countries.Find(address.OtherCountry_IdCountry.Value);
                    address.AddressShort = string.Format("{0} {1}", countries.Name,
                            address.OtherCountry_Address);
                }
                else
                {
                    address.AddressShort = address.OtherCountry_Address;
                }
            }

            return address;
        }

    }
}
