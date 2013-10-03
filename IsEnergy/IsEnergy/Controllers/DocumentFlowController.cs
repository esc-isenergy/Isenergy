using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IsEnergy;
using IsEnergyModel;
using DevExpress.Web.ASPxUploadControl;

namespace IsEnergy.Controllers
{
    public class DocumentFlowController : Controller
    {
        private Is_EnergyEntities db = new Is_EnergyEntities();
        IsEnergyModel.DataMode.ModeDocuments doc = new IsEnergyModel.DataMode.ModeDocuments();
        IsEnergyModel.DataMode.ModeUser docuser = new IsEnergyModel.DataMode.ModeUser();

        
        public ActionResult Index()
        {
            string identifierSubscriber = IsEnergyModel.Filters.AuthorizeMeAll.GetCurrentSubscriber(User.Identity.Name).IdentifierSubscriber;
            var model = db.Subscribers.Where(u => u.IdentifierSubscriber != identifierSubscriber).ToList();
            ViewData["Subscribers"]=model;
            return View();
        }
        public ActionResult GridViewPartial()
        {
           var model = db.Documents;
           return PartialView("_GridViewPartial", model.ToList());
        }

        public ActionResult DetailsDocuments(int id)
        {  
            Documents document =new Documents();
            Subscribers subscriber=new Subscribers();
            SubscribersSubdivision subscriberssubdivision = new SubscribersSubdivision();
            document = db.Documents.Find(id);
            subscriber=db.Subscribers.Find(document.IdentifierSubscriberSender);
            ViewBag.Sender=subscriber.Name;
            if(document.idSubdivisionSender!=0) 
            {
               subscriberssubdivision=subscriber.SubscribersSubdivision.First(s=>s.idSubdivision==document.idSubdivisionSender);
               ViewBag.SubdivisionSenderName=subscriberssubdivision.SubdivisionName;
               ViewBag.SubdivisionSenderCode=subscriberssubdivision.SubdivisionCode;
            }
            subscriber=db.Subscribers.Find(document.IdentifierSubscriberReceiver);
            ViewBag.Receiver=subscriber.Name;
            if(document.idSubdivisionReceiver!=0) 
            {
              subscriberssubdivision=subscriber.SubscribersSubdivision.First(s=>s.idSubdivision==document.idSubdivisionReceiver);
              ViewBag.SubdivisionReceiverName = subscriberssubdivision.SubdivisionName;
              ViewBag.SubdivisionReceiverCode = subscriberssubdivision.SubdivisionCode;
            }

            return View(document);
        }

        public ActionResult OutgoingKontragent(string identifierSubscriber)
        {
            var model = db.Subscribers.Where(u => u.IdentifierSubscriber != identifierSubscriber);
            return PartialView("_GridViewInternalKontragents", model.ToList());
        }

        public ActionResult TreeListSubdivision(string identifierSubscriber )
        {
            if (identifierSubscriber == null) identifierSubscriber = IsEnergyModel.Filters.AuthorizeMeAll.GetCurrentSubscriber(User.Identity.Name).IdentifierSubscriber;
            var model = db.SubscribersSubdivision.Where(u => u.IdentifierSubscriber == identifierSubscriber);
            ViewData["IdentifierSubscriber"] = identifierSubscriber;
            return PartialView("_TreeListSubdivision", model.ToList());

        }

        public ActionResult GridViewDocuments()
        {
            Users user = IsEnergyModel.Filters.AuthorizeMeAll.GetCurrentUser(User.Identity.Name);
            var model = db.DocumentsTemp.Where(u => u.IdUserCreate == user.IdUser);
            return PartialView("~/Views/DocumentFlow/DocumentPanel.cshtml", model.ToList());
        }
        public ActionResult ClearDocumentsTemp()
        {
            doc.ClearDocumentsTempUser(User.Identity.Name);
            Users user = IsEnergyModel.Filters.AuthorizeMeAll.GetCurrentUser(User.Identity.Name);
            var model = db.DocumentsTemp.Where(u => u.IdUserCreate == user.IdUser);
            return PartialView("~/Views/DocumentFlow/DocumentPanel.cshtml", model.ToList());
 
        }
        public ActionResult SaveDocumentTempInReal(int IdDocumentTemp)
        {
            Users user = IsEnergyModel.Filters.AuthorizeMeAll.GetCurrentUser(User.Identity.Name);
            doc.SaveDocumentTempInReal(IdDocumentTemp);
            var model = db.DocumentsTemp.Where(u => u.IdUserCreate == user.IdUser);
            return PartialView("~/Views/DocumentFlow/DocumentPanel.cshtml", model.ToList());

        }
        public ActionResult ClearDocumentFromTempList(int IdDocumentTemp)
        {
            Users user = IsEnergyModel.Filters.AuthorizeMeAll.GetCurrentUser(User.Identity.Name);
            doc.DelDocumentTemp(IdDocumentTemp);
            var model = db.DocumentsTemp.Where(u => u.IdUserCreate == user.IdUser);
            return PartialView("~/Views/DocumentFlow/DocumentPanel.cshtml", model.ToList());

        }

        public ActionResult UploadDocument(UploadedFile[] UploadDocuments, string GetIdentifierSubscriber=null, int GetIdSubdivision=0)
        {
            byte[] DataArrey = null;
           
            if (UploadDocuments != null)
            {
                foreach (UploadedFile file in UploadDocuments)
                {
                    DataArrey = file.FileBytes;
                    doc.AddDocumentTemp(User.Identity.Name, file.FileName, DataArrey, GetIdentifierSubscriber, GetIdSubdivision);
                }       
                
            }

            return View();
        }

        public byte[] HashSignDocumentTemp (int IdDocumentTemp)
        {
            return doc.GetHashDocumentTemp(IdDocumentTemp);
        }

        public string CertSubjectNameSignDocumentTemp (int IdDocumentTemp)
        {
           return  docuser.GetCertSubjectName(User.Identity.Name);
        }

        public bool SaveDocumentTempInRealAndSend (int IdDocumentTemp,byte[] dataSing)
        {
           ResultMode result = doc.Steps_0_to_3(User.Identity.Name, IdDocumentTemp, dataSing);
           return result.Executed;
        }


    }
}
