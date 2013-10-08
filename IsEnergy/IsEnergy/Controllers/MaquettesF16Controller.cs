using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IsEnergy;
using IsEnergyModel;
using DevExpress.Web.ASPxUploadControl;
using DevExpress.Web.Mvc;

namespace IsEnergy.Controllers
{
    public class MaquettesF16Controller : Controller
    {
        //
        // GET: /MaquettesF16/
        
        IsEnergyModel.Is_EnergyEntities db = new IsEnergyModel.Is_EnergyEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DetailsMaquettesF16(int IdMaquette16)
        {
            var model = db.Maquettes16.Find(IdMaquette16);
            return View("Details", model);
        }
        
        [ValidateInput(false)]
        public ActionResult GridViewPartial()
        {
            var model = db.Maquettes16;
            return PartialView("_GridViewPartial", model.ToList());
        }
              
    }
}
