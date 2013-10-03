using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IsEnergyModel
{
    public partial class Users {

       public string ShortName { get { return String.Format("{0} {1} {2}", FirstName , MidleName , LastName); }}

    }
}
