using DevExpress.Web.ASPxUploadControl;
using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IsEnergy.AllMode
{
    public class ControllersMode
    {

        /// <summary>
        /// передаем имя получаем байты
        /// </summary>
        /// <param name="UploadName"> Имя </param>
        /// <returns></returns>
        public static byte[] UploadGetByte(string UploadName)
        {
            try
            {
                UploadedFile[] files = UploadControlExtension.GetUploadedFiles(UploadName);
                if (files != null)
                {
                    if (files[0].ContentLength != 0)
                    {
                        byte[] DataArrey = new byte[files[0].ContentLength];
                        DataArrey = files[0].FileBytes;
                        return DataArrey;
                    }
                }
                return (null);
            }
            catch { return (null); }

        }

    }
}