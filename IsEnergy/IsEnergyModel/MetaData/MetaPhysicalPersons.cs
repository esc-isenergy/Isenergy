using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsEnergyModel
{
    public partial class PhysicalPersons
    {
        public string Requisites { get { return String.Format("ИНН: {0}  Адресс: {1}", INN, (Address != null) ? Address.AddressShort : string.Empty); } }
        public string ShortName { get { return String.Format("{0} {1} {2}", FirstName, MidleName, LastName); } }
    }
}
