using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsEnergyModel
{
    public partial class Subscribers
    {
        public string NameTypeElementAbonent
        {
            get
            {
                switch (TypeElementAbonent)
                {
                    case 1:
                        return "Юр. лицо";
                    case 2:
                        return "ИП";
                    case 3:
                        return "Физ. лицо";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
