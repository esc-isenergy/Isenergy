using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsEnergyModel
{
    public partial class Contractors
    {
        Is_EnergyEntities db = new Is_EnergyEntities();

        public string SubscriberContractorName
        {
            get
            { if (!String.IsNullOrEmpty(IdentifierSubscriberContractor)) return db.Subscribers.Find(IdentifierSubscriberContractor).Name; else return null; }
        }
        public string SubscriberContractorRequisites
        {
            get
            { if (!String.IsNullOrEmpty(IdentifierSubscriberContractor)) return db.Subscribers.Find(IdentifierSubscriberContractor).Requisites; else return null; }
        }

    }

}
