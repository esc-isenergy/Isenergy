using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsEnergyModel
{
    public partial class SoleTraders
    {
        public string Requisites { get { return String.Format("ИНН: {0} ОГРН: {1} Адресс: {2}", INN, OGRN, (Address != null) ? Address.AddressShort : string.Empty); } }
        public string ContactFaceShort { get { return String.Format("{0} {1} {2}", ContactFaceFirstName, ContactFaceMidleName, ContactFaceLastName); } }
        public string ShortName { get { return String.Format("{0} {1} {2}", FirstName,MidleName, LastName); } }

    }
}
