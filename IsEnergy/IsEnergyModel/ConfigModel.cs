using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IsEnergyModel
{
   public static class ConfigModel
    {
        #region Переменные AD

       public const string sDomain = "senergy.local";
       public const string sDefaultOU = "OU=IsEnergy,DC=senergy,DC=local";
       public const string sDefaultRootOU = "DC=senergy,DC=local";
       public const string sServiceUser = "Deploy";
       public const string sServicePassword = "Zaq12wsx";

        public class cADUserGroup
        {
            //группы админов
            public const string sAdmin = "IsEnergyAdministrators";
            public const string sRootAdmin = "IsEnergyMainAdministrators";
            public const string sModifyingAdministrators = "IsEnergyModifyingAdministrators";
            public const string sModifyingOperators = "IsEnergyModifyingOperators";
            public const string sModifyingNews = "IsEnergyModifyingNews";
            public const string sModifyingMailing = "IsEnergyModifyingMailing";
            //группы операторов
            public const string sOperator = "IsEnergyOperators";
        }


        #endregion

        #region CSP

        public static CspParameters CSP_cp = new CspParameters() { KeyContainerName = "testcrp", ProviderType = 75 };

        #endregion

        public static string AssemblyVersionProg = "1.2.0.1";
        public static string idOperEDO = "2DL";
        public static string INNOperEDO = "1657055576";
        public static string KPPOperEDO = "165701001";
        public static string OGRNOperEDO = "1051629047843";
        public static string NameOperEDO = "ООО «Центр Электронных Услуг»";

        public static string postSingerOperEDO = "Генеральный директор";
        public static string FirstNameSingerOperEDO = "Давлетьяров";
        public static string MidelNameSingerOperEDO = "Ахмет";
        public static string LastNameSingerOperEDO = "Маратович";
    }
}
