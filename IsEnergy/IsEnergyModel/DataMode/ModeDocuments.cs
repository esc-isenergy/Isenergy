using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Security.Cryptography;
using CryptoPro.Sharpei;
using System.Data;


namespace IsEnergyModel.DataMode
{
    public class ModeDocuments
    {
        public ResultMode AddDocumentTemp(string userName, string fileName, byte[] fileData, string IdentifierSubscriberReceive, int idSubdivisionSenderReceiver = 0, bool signatureContractor = false)
        {

            try
            {
                Users user = Filters.AuthorizeMeAll.GetCurrentUser(userName);
                if (user != null)
                {
                    Subscribers subscribers = Filters.AuthorizeMeAll.GetCurrentSubscriber(user);
                    if (subscribers != null)
                    {
                        SubscribersSubdivision subscribersSubdivision = Filters.AuthorizeMeAll.GetCurrentSubscribersSubdivision(userName);

                        Is_EnergyEntities db = new Is_EnergyEntities();
                        DocumentsTemp documents = new DocumentsTemp();
                        documents.IdUserCreate = user.IdUser;
                        documents.IdentifierSubscriberSender = subscribers.IdentifierSubscriber;
                        documents.idSubdivisionSender = (subscribersSubdivision != null) ? subscribersSubdivision.idSubdivision : 0;
                        documents.DataFile = fileData;
                        documents.NameFile = fileName;
                        if (!String.IsNullOrEmpty(IdentifierSubscriberReceive)) { documents.IdentifierSubscriberReceiver = IdentifierSubscriberReceive; }
                        else { documents.IdentifierSubscriberReceiver = subscribers.IdentifierSubscriber; }
                        documents.idSubdivisionReceiver = idSubdivisionSenderReceiver;
                        if (GlobalMetods.XmlSerializationHelper.CheckTypeDeserialize<XSD.ON_SFAKT_1_897_01_05_01_03.Файл>(fileData))
                        {
                            XSD.ON_SFAKT_1_897_01_05_01_03.Файл Фаил = GlobalMetods.XmlSerializationHelper.Deserialize<XSD.ON_SFAKT_1_897_01_05_01_03.Файл>(fileData);
                            documents.SignatureContractor = true;
                            documents.TypeDocument = 1115101;
                            documents.NameDocument = String.Format("{0} от {1}", Фаил.Документ.СвСчФакт.НомерСчФ, Фаил.Документ.СвСчФакт.ДатаСчФ);
                        }
                        else
                        {
                            documents.SignatureContractor = signatureContractor;
                            documents.TypeDocument = 0;
                            documents.NameDocument = fileName;
                        }

                        db.DocumentsTemp.Add(documents);
                        db.SaveChanges();
                        return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Документ добавлен" };
                    }
                }
                return new ResultMode() { Executed = false, StrError = string.Empty, StrResult = "Не удалось добавить документ" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось добавить документ" }; }
        }

        public ResultMode EditDocumentTemp(string userName, int idDocumentTemp,  bool signatureContractor,  string comment)
        {

            try
            {
                Users user = Filters.AuthorizeMeAll.GetCurrentUser(userName);
                if (user != null)
                {
                    Subscribers subscribers = Filters.AuthorizeMeAll.GetCurrentSubscriber(user);
                    if (subscribers != null)
                    {
                        SubscribersSubdivision subscribersSubdivision = Filters.AuthorizeMeAll.GetCurrentSubscribersSubdivision(userName);

                        Is_EnergyEntities db = new Is_EnergyEntities();
                        DocumentsTemp documents = db.DocumentsTemp.Find(idDocumentTemp);
                        db.Entry(documents).State = EntityState.Modified;
                        documents.SignatureContractor = signatureContractor;
                        documents.Comment = comment;
                        db.SaveChanges();
                        return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Документ изменен" };
                    }
                }
                return new ResultMode() { Executed = false, StrError = string.Empty, StrResult = "Не удалось изменен документ" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось изменить документ" }; }
        }

        public ResultMode EditDocumentsTemp(string userName, string IdentifierSubscriberReceive, int idSubdivisionSenderReceiver = 0)
        {

            try
            {
                Users user = Filters.AuthorizeMeAll.GetCurrentUser(userName);
                if (user != null)
                {
                    Subscribers subscribers = Filters.AuthorizeMeAll.GetCurrentSubscriber(user);
                    if (subscribers != null)
                    {
                        SubscribersSubdivision subscribersSubdivision = Filters.AuthorizeMeAll.GetCurrentSubscribersSubdivision(userName);

                        Is_EnergyEntities db = new Is_EnergyEntities();
                        var documentsList = db.DocumentsTemp.Where(u=>u.IdUserCreate == user.IdUser);
                        foreach (DocumentsTemp document in documentsList)
                        {
                            db.Entry(document).State = EntityState.Modified;
                            document.IdUserCreate = user.IdUser;
                            document.IdentifierSubscriberSender = subscribers.IdentifierSubscriber;
                            document.idSubdivisionSender = (subscribersSubdivision != null) ? subscribersSubdivision.idSubdivision : 0;
                            if (!String.IsNullOrEmpty(IdentifierSubscriberReceive)) { document.IdentifierSubscriberReceiver = IdentifierSubscriberReceive; }
                            else { document.IdentifierSubscriberReceiver = subscribers.IdentifierSubscriber; }
                            document.idSubdivisionReceiver = idSubdivisionSenderReceiver;
                            db.SaveChanges();
                        }
                        return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Документ изменен" };
                    }
                }
                return new ResultMode() { Executed = false, StrError = string.Empty, StrResult = "Не удалось изменен документ" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось изменить документ" }; }
        }

        public ResultMode ClearDocumentsTempUser(string userName)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                Users user = Filters.AuthorizeMeAll.GetCurrentUser(userName);
                if (user != null)
                {

                    var documentsList = db.DocumentsTemp.Where(u => u.IdUserCreate == user.IdUser);
                    foreach (DocumentsTemp documentTempItem in documentsList)
                    {
                        db.DocumentsTemp.Remove(documentTempItem);
                    }

                    db.SaveChanges();
                }
                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Список очищен" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось очистить список" }; }

        }

        public ResultMode DelDocumentTemp(DocumentsTemp documentTemp)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                db.DocumentsTemp.Remove(documentTemp);
                db.SaveChanges();
                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Документ удален" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось удалить документ" }; }
        }

        public ResultMode DelDocumentTemp(int idDocumentTemp)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                DocumentsTemp documentTemp = db.DocumentsTemp.Find(idDocumentTemp);
                if (documentTemp != null)
                {
                    db.DocumentsTemp.Remove(documentTemp);
                    db.SaveChanges();
               
                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Документ удален" }; 
                }
                return new ResultMode() { Executed = false, StrError = string.Empty, StrResult = "Документ не найден" }; 
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось удалить документ" }; }
        }

        public byte[] GetHashDocumentTemp(int idDocumentTemp)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                DocumentsTemp documentsTemp = db.DocumentsTemp.Find(idDocumentTemp);
                return GetHashDocumentTemp(documentsTemp);
            }
            catch { return null; }
        }

        public byte[] GetHashDocumentTemp(DocumentsTemp documentsTemp)
        {
            try
            {
                Gost3411CryptoServiceProvider CSPHash = new Gost3411CryptoServiceProvider();
                return CSPHash.ComputeHash(documentsTemp.DataFile);
            }
            catch { return null; }
        }

        #region Stemps

        public ResultMode Steps_0_to_3(string userName, int idDocumentTemp, byte[] fileDataSign)
        {
            try
            {
                //Сохраняем в черновики
                ResultMode resultTempSave = SaveDocumentTempInReal(idDocumentTemp);
                if (!resultTempSave.Executed) return resultTempSave;
                Documents doc = resultTempSave.ObjectResult as Documents;

                //отправляем фаил
                ResultMode resultSend = SendDocument(userName, doc.IdDocument, fileDataSign);
                if (!resultSend.Executed) return resultSend;
                Documents docSend = resultSend.ObjectResult as Documents;

                if (docSend.TypeDocument == 1115101)
                {
                    //Потверждение оператора даты принятия  фаил
                    ResultMode resultSendP = Stemp_3_ConfirmationDateReceiptInOperatorInvoices(doc.IdDocument);
                    if (!resultSendP.Executed) return resultSend;
                    docSend = resultSendP.ObjectResult as Documents;
                }

                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Документ сохранен", ObjectResult = docSend };

            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось сохранить" }; }
        }

        public ResultMode SaveDocumentTempInReal(int idDocumentTemp)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                //находим документ 
                DocumentsTemp documentsTemp = db.DocumentsTemp.Find(idDocumentTemp);
                if (documentsTemp != null)
                {
                    //сохраняем в черновики 
                    Documents document = new Documents();
                    document.Comment = documentsTemp.Comment;
                    document.DataFile = documentsTemp.DataFile;
                    document.NameFile = documentsTemp.NameFile;
                    document.DateSend = GlobalMetods.CommonMethods.GetDataTimeNow;
                    document.IdentifierSubscriberReceiver = documentsTemp.IdentifierSubscriberReceiver;
                    document.IdentifierSubscriberSender = documentsTemp.IdentifierSubscriberSender;
                    document.idSubdivisionReceiver = documentsTemp.idSubdivisionReceiver;
                    document.idSubdivisionSender = documentsTemp.idSubdivisionSender;
                    document.NameDocument = documentsTemp.NameDocument;
                    document.SignatureContractor = documentsTemp.SignatureContractor;
                    document.State = 1;
                    document.TypeDocument = documentsTemp.TypeDocument;
                    db.Documents.Add(document);
                    db.SaveChanges();
                    
                    //удаляем Temp фаил
                    ResultMode resultTemp = DelDocumentTemp(documentsTemp.IdDocumentTemp);
                    if (!resultTemp.Executed) return resultTemp;
                    
                    return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Документ сохранен", ObjectResult = document};
                }
                return new ResultMode() { Executed = false, StrError = string.Empty, StrResult = "Документ не найден" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось сохранить" }; }
        }

        public ResultMode SendDocument(string userName, int idDocument, byte[] fileDataSign)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                //находим документ 
                Documents doc = db.Documents.Find(idDocument);
                if (doc != null)
                {
                    Gost3410CryptoServiceProvider CSP = new Gost3410CryptoServiceProvider(ConfigModel.CSP_cp);
                    Gost3411CryptoServiceProvider CSPHash = new Gost3411CryptoServiceProvider();
                    //Проверяем правильность подписи и выводим результат.
                    bool CSPVerify = CSP.VerifyHash(GetHashDocument(doc), fileDataSign);
                    if (true)
                    {
                        //Сохраняем в БД(Документооборот) документ и его подись 
                        DocumentFlow documentFlow = new DocumentFlow();
                        documentFlow.DataFile = doc.DataFile;
                        documentFlow.NameFile = doc.NameFile;
                        documentFlow.DataSign = fileDataSign;
                        documentFlow.DateCreate = GlobalMetods.CommonMethods.GetDataTimeNow;
                        documentFlow.IdDocumentMain = doc.IdDocument;
                        documentFlow.NameDocument = doc.NameDocument;
                        documentFlow.NameSign = doc.NameDocument + ".sig"; ;
                        documentFlow.State = 2;
                        db.DocumentFlow.Add(documentFlow);
                        db.SaveChanges();
                        //Сохраняем в БД изменение статуса 
                        db.Entry(doc).State = EntityState.Modified;
                        doc.State = 2;
                        db.SaveChanges();
                        return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Документ сохранен",ObjectResult= doc };
                    }
                    return new ResultMode() { Executed = false, StrError = string.Empty, StrResult = "Документ не прошёл проверку ЭЦП" };
                }
                return new ResultMode() { Executed = false, StrError = string.Empty, StrResult = "Документ не найден" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось сохранить" }; }
        }

        public ResultMode Stemp_3_ConfirmationDateReceiptInOperatorInvoices(int idDocument)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                //находим документ 
                Documents doc = db.Documents.Find(idDocument);
                if (doc != null)
                {

                    Subscribers subscribersSender = db.Subscribers.Find(doc.IdentifierSubscriberSender);
                    Subscribers subscribersReceiver = db.Subscribers.Find(doc.IdentifierSubscriberReceiver);
                    DateTime dateNow = GlobalMetods.CommonMethods.GetDataTimeNow;
                    string nameFileNoExpansion = doc.NameFile.Substring(0, doc.NameFile.LastIndexOf('.'));
                    DocumentFlow parentDocumentFlow = doc.DocumentFlow.FirstOrDefault(u => u.State == 2);

                    //Создаем потверждение оператора
                    string ID_File = string.Format("DP_PDPOL_{0}_{1}_{2}_{3}", doc.IdentifierSubscriberSender, ConfigModel.idOperEDO, dateNow.ToString("yyyyMMdd"), Guid.NewGuid());

                    XSD.DP_PDPOL_1_984_00_01_01_01.Файл DP_PDPOL = new XSD.DP_PDPOL_1_984_00_01_01_01.Файл();
                    DP_PDPOL.ВерсПрог = ConfigModel.AssemblyVersionProg;
                    DP_PDPOL.ИдФайл = ID_File;
                    XSD.DP_PDPOL_1_984_00_01_01_01.ФайлДокумент файлДокумент = new XSD.DP_PDPOL_1_984_00_01_01_01.ФайлДокумент();

                    XSD.DP_PDPOL_1_984_00_01_01_01.ФайлДокументОперЭДО оперЭДО = new XSD.DP_PDPOL_1_984_00_01_01_01.ФайлДокументОперЭДО();
                    оперЭДО.ИдОперЭДО = ConfigModel.idOperEDO;
                    оперЭДО.ИННЮЛ = ConfigModel.INNOperEDO;
                    оперЭДО.НаимОрг = ConfigModel.NameOperEDO;
                    файлДокумент.ОперЭДО = оперЭДО;

                    XSD.DP_PDPOL_1_984_00_01_01_01.ФайлДокументСведПодтв сведПодтв = new XSD.DP_PDPOL_1_984_00_01_01_01.ФайлДокументСведПодтв();
                    сведПодтв.ДатаОтпр = dateNow.ToString("dd.MM.yyyy");
                    сведПодтв.ВремяОтпр = dateNow.ToString("HH:mm:ss");

                    XSD.DP_PDPOL_1_984_00_01_01_01.ФайлДокументСведПодтвСведОтпрФайл сведОтпрФайл = new XSD.DP_PDPOL_1_984_00_01_01_01.ФайлДокументСведПодтвСведОтпрФайл();
                    сведОтпрФайл.ИмяПостФайла = nameFileNoExpansion;
                    if (parentDocumentFlow == null) return new ResultMode() { Executed = false, StrError = string.Empty, StrResult = "Документ не подписан отправителем" };
                    сведОтпрФайл.ЭЦППолФайл = Convert.ToBase64String(parentDocumentFlow.DataSign);
                    сведПодтв.СведОтпрФайл = сведОтпрФайл;

                    файлДокумент.СведПодтв = сведПодтв;

                    XSD.DP_PDPOL_1_984_00_01_01_01.УчастЭДО отпрДок = new XSD.DP_PDPOL_1_984_00_01_01_01.УчастЭДО();
                    отпрДок.ИдУчастЭДО = subscribersSender.IdentifierSubscriber;

                    if(subscribersSender.TypeElementAbonent == 1)
                    {
                        LegalEntities legalEntitiesSender = subscribersSender.LegalEntities.FirstOrDefault();
                        XSD.DP_PDPOL_1_984_00_01_01_01.УчастЭДОЮЛ участЭДОSender = new XSD.DP_PDPOL_1_984_00_01_01_01.УчастЭДОЮЛ();
                        участЭДОSender.ИННЮЛ = legalEntitiesSender.INN;
                        участЭДОSender.КПП = legalEntitiesSender.KPP;
                        участЭДОSender.НаимОрг = legalEntitiesSender.NameOrganization;
                        отпрДок.Item = участЭДОSender;
                    }
                    else if (subscribersSender.TypeElementAbonent == 2)
                    {
                        SoleTraders soleTradersSender = subscribersSender.SoleTraders.FirstOrDefault();
                        XSD.DP_PDPOL_1_984_00_01_01_01.УчастЭДОИП участЭДОSender = new XSD.DP_PDPOL_1_984_00_01_01_01.УчастЭДОИП();
                        участЭДОSender.ИННФЛ = soleTradersSender.INN;
                        участЭДОSender.ФИО = new XSD.DP_PDPOL_1_984_00_01_01_01.ФИОТип() { Имя = soleTradersSender.MidleName, Фамилия = soleTradersSender.FirstName, Отчество = soleTradersSender.LastName };
                        отпрДок.Item = участЭДОSender;
                    }
                    else { return new ResultMode() { Executed = false, StrError = string.Empty, StrResult = "Не определен тип отправителя" }; }

                    файлДокумент.ОтпрДок = отпрДок;

                    XSD.DP_PDPOL_1_984_00_01_01_01.УчастЭДО полДок = new XSD.DP_PDPOL_1_984_00_01_01_01.УчастЭДО();
                    полДок.ИдУчастЭДО = subscribersReceiver.IdentifierSubscriber;

                    if (subscribersSender.TypeElementAbonent == 1)
                    {
                        LegalEntities legalEntitiesSender = subscribersReceiver.LegalEntities.FirstOrDefault();
                        XSD.DP_PDPOL_1_984_00_01_01_01.УчастЭДОЮЛ участЭДОSender = new XSD.DP_PDPOL_1_984_00_01_01_01.УчастЭДОЮЛ();
                        участЭДОSender.ИННЮЛ = legalEntitiesSender.INN;
                        участЭДОSender.КПП = legalEntitiesSender.KPP;
                        участЭДОSender.НаимОрг = legalEntitiesSender.NameOrganization;
                        полДок.Item = участЭДОSender;
                    }
                    else if (subscribersSender.TypeElementAbonent == 2)
                    {
                        SoleTraders soleTradersSender = subscribersReceiver.SoleTraders.FirstOrDefault();
                        XSD.DP_PDPOL_1_984_00_01_01_01.УчастЭДОИП участЭДОSender = new XSD.DP_PDPOL_1_984_00_01_01_01.УчастЭДОИП();
                        участЭДОSender.ИННФЛ = soleTradersSender.INN;
                        участЭДОSender.ФИО = new XSD.DP_PDPOL_1_984_00_01_01_01.ФИОТип() { Имя = soleTradersSender.MidleName, Фамилия = soleTradersSender.FirstName, Отчество = soleTradersSender.LastName };
                        полДок.Item = участЭДОSender;
                    }
                    else { return new ResultMode() { Executed = false, StrError = string.Empty, StrResult = "Не определен тип получателя" }; }

                    файлДокумент.ПолДок = полДок;

                    файлДокумент.Подписант = new XSD.DP_PDPOL_1_984_00_01_01_01.ФайлДокументПодписант()
                    {
                        Должность = ConfigModel.postSingerOperEDO,
                        ФИО = new XSD.DP_PDPOL_1_984_00_01_01_01.ФИОТип
                        {
                            Имя = ConfigModel.MidelNameSingerOperEDO,
                            Отчество = ConfigModel.LastNameSingerOperEDO,
                            Фамилия = ConfigModel.FirstNameSingerOperEDO
                        }
                    };

                    DP_PDPOL.Документ = файлДокумент;



                    byte[] Byte_DP_PDPOL = GlobalMetods.XmlSerializationHelper.Serialize(DP_PDPOL);

                    Gost3410CryptoServiceProvider CSP = new Gost3410CryptoServiceProvider(ConfigModel.CSP_cp);
                    Gost3411CryptoServiceProvider CSPHash = new Gost3411CryptoServiceProvider();

                    byte[] Byte_DP_PDPOLSing = CSP.SignData(Byte_DP_PDPOL, CSPHash);

                    if (CSP.VerifyData(Byte_DP_PDPOL,CSPHash,Byte_DP_PDPOLSing))
                    {
                        //Сохраняем в БД(Документооборот) документ и его подись 
                        DocumentFlow documentFlow = new DocumentFlow();
                        documentFlow.DataFile = Byte_DP_PDPOL;
                        documentFlow.NameFile = nameFileNoExpansion + ".xml";
                        documentFlow.DataSign = Byte_DP_PDPOLSing;
                        documentFlow.DateCreate = GlobalMetods.CommonMethods.GetDataTimeNow;
                        documentFlow.IdDocumentMain = doc.IdDocument;
                        documentFlow.NameDocument = "Подтверждение оператора даты получения";
                        documentFlow.NameSign = nameFileNoExpansion + ".xml.sig";
                        documentFlow.State = 3;
                        db.DocumentFlow.Add(documentFlow);
                        db.SaveChanges();
                        //Сохраняем в БД изменение статуса 
                        db.Entry(doc).State = EntityState.Modified;
                        doc.State = 3;
                        db.SaveChanges();
                        return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Документ сохранен" };
                    }
                    return new ResultMode() { Executed = false, StrError = string.Empty, StrResult = "Документ не прошёл проверку ЭЦП" };
                }
                return new ResultMode() { Executed = false, StrError = string.Empty, StrResult = "Документ не найден" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось сохранить" }; }
        }

        public ResultMode Stemp_4_ConfirmationReceiptOperatorInSubscriberInvoices(string userName,int idDocument)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                Users user = Filters.AuthorizeMeAll.GetCurrentUser(userName);
                //находим документ 
                Documents doc = db.Documents.Find(idDocument);
                if (doc != null)
                {

                    Subscribers subscribersSender = db.Subscribers.Find(doc.IdentifierSubscriberSender);
                    Subscribers subscribersReceiver = db.Subscribers.Find(doc.IdentifierSubscriberReceiver);
                    DateTime dateNow = GlobalMetods.CommonMethods.GetDataTimeNow;
                    DocumentFlow parentDocumentFlow = doc.DocumentFlow.FirstOrDefault(u => u.State == 3);
                    string nameFileNoExpansion = parentDocumentFlow.NameFile.Substring(0, parentDocumentFlow.NameFile.LastIndexOf('.'));

                    DateTime TimeNow = GlobalMetods.CommonMethods.GetDataTimeNow;
                    //Создаем потверждение оператора
                    string ID_File = string.Format("DP_IZVPOL_{0}_{1}_{2}_{3}",ConfigModel.idOperEDO, doc.IdentifierSubscriberSender,  dateNow.ToString("yyyyMMdd"), Guid.NewGuid());

                    XSD.DP_IZVPOL_1_982_00_01_01_01.Файл DP_IZVPOL = new XSD.DP_IZVPOL_1_982_00_01_01_01.Файл();
                    DP_IZVPOL.ВерсПрог = ConfigModel.AssemblyVersionProg;
                    DP_IZVPOL.ИдФайл = ID_File;
                    XSD.DP_IZVPOL_1_982_00_01_01_01.ФайлДокумент файлДокумент = new XSD.DP_IZVPOL_1_982_00_01_01_01.ФайлДокумент();

                    XSD.DP_IZVPOL_1_982_00_01_01_01.ФайлДокументУчастЭДО УчастЭДО = new XSD.DP_IZVPOL_1_982_00_01_01_01.ФайлДокументУчастЭДО();
                    УчастЭДО.ИдУчастЭДО = subscribersSender.IdentifierSubscriber;

                    
                    if (subscribersSender.TypeElementAbonent == 1)
                    {
                        LegalEntities legalEntitiesSender = subscribersSender.LegalEntities.FirstOrDefault();
                        XSD.DP_IZVPOL_1_982_00_01_01_01.ЮЛТип участЭДОSender = new XSD.DP_IZVPOL_1_982_00_01_01_01.ЮЛТип();
                        участЭДОSender.ИННЮЛ = legalEntitiesSender.INN;
                        участЭДОSender.КПП = legalEntitiesSender.KPP;
                        участЭДОSender.НаимОрг = legalEntitiesSender.NameOrganization;
                        УчастЭДО.Item = участЭДОSender;
                    }
                    else if (subscribersSender.TypeElementAbonent == 2)
                    {
                        SoleTraders soleTradersSender = subscribersSender.SoleTraders.FirstOrDefault();
                        XSD.DP_IZVPOL_1_982_00_01_01_01.ФЛТип участЭДОSender = new XSD.DP_IZVPOL_1_982_00_01_01_01.ФЛТип();
                        участЭДОSender.ИННФЛ = soleTradersSender.INN;
                        участЭДОSender.ФИО = new XSD.DP_IZVPOL_1_982_00_01_01_01.ФИОТип() { Имя = soleTradersSender.MidleName, Фамилия = soleTradersSender.FirstName, Отчество = soleTradersSender.LastName };
                        УчастЭДО.Item = участЭДОSender;
                    }
                    else { return new ResultMode() { Executed = false, StrError = string.Empty, StrResult = "Не определен тип отправителя" }; }
                    
                    файлДокумент.УчастЭДО = УчастЭДО;

                    XSD.DP_IZVPOL_1_982_00_01_01_01.ФайлДокументСвИзвПолуч СвИзвПолуч = new XSD.DP_IZVPOL_1_982_00_01_01_01.ФайлДокументСвИзвПолуч();
                    СвИзвПолуч.ДатаПол = TimeNow.ToString("dd.MM.yyyy");
                    СвИзвПолуч.ВремяПол = TimeNow.ToString("HH:mm:ss");
                    XSD.DP_IZVPOL_1_982_00_01_01_01.ФайлДокументСвИзвПолучСведПолФайл СведПолФайл = new XSD.DP_IZVPOL_1_982_00_01_01_01.ФайлДокументСвИзвПолучСведПолФайл();
                    СведПолФайл.ИмяПостФайла = nameFileNoExpansion;
                    СведПолФайл.ЭЦППолФайл = Convert.ToBase64String(parentDocumentFlow.DataSign);
                    СвИзвПолуч.СведПолФайл = СведПолФайл;

                    файлДокумент.СвИзвПолуч = СвИзвПолуч;

                    XSD.DP_IZVPOL_1_982_00_01_01_01.ФайлДокументОтпрДок ОтпрДок = new XSD.DP_IZVPOL_1_982_00_01_01_01.ФайлДокументОтпрДок();
                    ОтпрДок.ИдУчастЭДО = ConfigModel.idOperEDO;

                    XSD.DP_IZVPOL_1_982_00_01_01_01.ФайлДокументОтпрДокОперЭДО ОтпрДокSender = new XSD.DP_IZVPOL_1_982_00_01_01_01.ФайлДокументОтпрДокОперЭДО();
                    ОтпрДокSender.ИННЮЛ = ConfigModel.INNOperEDO;
                    ОтпрДокSender.ИдОперЭДО = ConfigModel.idOperEDO;
                    ОтпрДокSender.НаимОрг = ConfigModel.NameOperEDO;
                    ОтпрДок.Item = ОтпрДокSender;

                    файлДокумент.ОтпрДок = ОтпрДок;

                    XSD.DP_IZVPOL_1_982_00_01_01_01.ФайлДокументПодписант Подписант = new XSD.DP_IZVPOL_1_982_00_01_01_01.ФайлДокументПодписант();
                    Подписант.Должность = string.Empty;
                    Подписант.ФИО = new XSD.DP_IZVPOL_1_982_00_01_01_01.ФИОТип() { Имя = user.MidleName, Фамилия = user.FirstName, Отчество = user.LastName };
                    файлДокумент.Подписант = Подписант;
                    DP_IZVPOL.Документ = файлДокумент;


                    byte[] Byte_DP_IZVPOL = GlobalMetods.XmlSerializationHelper.Serialize(DP_IZVPOL);
                    if (Byte_DP_IZVPOL!=null)
                    {
                        //Сохраняем в БД(Документооборот) документ и его подись 
                        DocumentFlow documentFlow = new DocumentFlow();
                        documentFlow.DataFile = Byte_DP_IZVPOL;
                        documentFlow.NameFile = nameFileNoExpansion + ".xml";
                        documentFlow.IdDocumentMain = doc.IdDocument;
                        documentFlow.NameDocument = "Подтверждение оператора даты получения";
                        documentFlow.State = 4;
                        db.DocumentFlow.Add(documentFlow);
                        db.SaveChanges();
                        //Сохраняем в БД изменение статуса 
                        db.Entry(doc).State = EntityState.Modified;
                        doc.State = 4;
                        db.SaveChanges();
                        return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Документ сохранен", ObjectResult = documentFlow };
                    }
                    return new ResultMode() { Executed = false, StrError = string.Empty, StrResult = "Документ не создан" };
                }
                return new ResultMode() { Executed = false, StrError = string.Empty, StrResult = "Документ не найден" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось сохранить" }; }
        }

        public ResultMode Stemp_5_ConfirmationReceiptOperatorInSubscriberInvoicesVerifyAndSave(string userName, int idDocument, byte[] fileDataSign)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                //находим документ 
                DocumentFlow parentDocumentFlow = db.DocumentFlow.Find(idDocument);
                db.Entry(parentDocumentFlow).State = EntityState.Modified;

                Documents doc = db.Documents.Find(parentDocumentFlow.IdDocumentMain);
                if (doc != null)
                {
                    Gost3410CryptoServiceProvider CSP = new Gost3410CryptoServiceProvider(ConfigModel.CSP_cp);
                    Gost3411CryptoServiceProvider CSPHash = new Gost3411CryptoServiceProvider();
                    //Проверяем правильность подписи и выводим результат.
                    bool CSPVerify = CSP.VerifyHash(GetHashDocument(doc), fileDataSign);
                    if (true)
                    {
                        //Сохраняем в БД(Документооборот) документ и его подись 
                        DocumentFlow documentFlow = new DocumentFlow();
                        documentFlow.DataSign = fileDataSign;
                        documentFlow.DateCreate = GlobalMetods.CommonMethods.GetDataTimeNow;
                        documentFlow.NameSign = documentFlow.NameFile + ".xml.sig";
                        documentFlow.State = 5;
                        db.SaveChanges();
                        //Сохраняем в БД изменение статуса 
                        db.Entry(doc).State = EntityState.Modified;
                        doc.State = 2;
                        db.SaveChanges();
                        return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Документ сохранен", ObjectResult = doc };
                    }
                    return new ResultMode() { Executed = false, StrError = string.Empty, StrResult = "Документ не прошёл проверку ЭЦП" };
                }
                return new ResultMode() { Executed = false, StrError = string.Empty, StrResult = "Документ не найден" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось сохранить" }; }
        }

        #endregion

        //Documents
        public byte[] GetHashDocument(int idDocument)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                Documents document = db.Documents.Find(idDocument);
                return GetHashDocument(document);
            }
            catch { return null; }
        }

        public byte[] GetHashDocument(Documents document)
        {
            try
            {
                Gost3411CryptoServiceProvider CSPHash = new Gost3411CryptoServiceProvider();
                return CSPHash.ComputeHash(document.DataFile);
            }
            catch { return null; }
        }

        //Генерация потверждения получения оператором
    }
}
