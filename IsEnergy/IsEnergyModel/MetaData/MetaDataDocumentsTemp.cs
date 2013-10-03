using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsEnergyModel
{

    public partial class DocumentsTemp
    {
        Is_EnergyEntities db = new Is_EnergyEntities();
        public string SubscriberReceiverName
        {
            get
            { if (!String.IsNullOrEmpty(IdentifierSubscriberReceiver)) return db.Subscribers.Find(IdentifierSubscriberReceiver).Name; else return null; }
        }
        public string SubdivisionReceiverName
        {
            get
            { if (idSubdivisionReceiver != null && idSubdivisionReceiver != 0)return db.SubscribersSubdivision.Find(idSubdivisionReceiver).SubdivisionName; else return null; }
        }

        public string TypeDocumentName
        {
            get
            {
                switch (TypeDocument)
                {
                    case 1115101:
                        return "Счёт-фактура";
                    default:
                        return "Неформализованный";
                }

            }
        }
    }
}
