using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsEnergyModel
{
    public partial class LegalEntities
    {
        public string Requisites { get { return String.Format("ИНН: {0} КПП:{1}  ОГРН: {2} Адресс: {3}", INN, KPP, OGRN,(Address!=null)?Address.AddressShort:string.Empty ); } }
        public string ContactFaceShort { get { return String.Format("{0} {1} {2}", ContactFaceFirstName, ContactFaceMidleName, ContactFaceLastName); } }
    }
}
