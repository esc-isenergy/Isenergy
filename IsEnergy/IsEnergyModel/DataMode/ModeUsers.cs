using CryptoPro.Sharpei;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace IsEnergyModel.DataMode
{

    public class ModeUser
    {
        public ResultMode Create(Users user, string pass, byte[] byteCert = null)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                cAD.CreateNewUser(null, user.Login, pass, user.MidleName, user.FirstName, user.Email);
                db.Users.Add(user);
                db.SaveChanges();
                // Добавим сертификат
                if (byteCert!=null) AddCertificate(user, byteCert);
                return new ResultMode() {Executed =true,StrError=string.Empty,StrResult="Пользователь успешно добавлен" , ObjectResult = user};
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось добавить пользователя" }; }
        }

        public ResultMode Edit(Users user)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                cAD.EditUser(user.Login, user.MidleName, user.FirstName, user.Email);
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Пользователь успешно изменен", ObjectResult = user };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось изменить пользователя" }; }
        }

        public ResultMode EditPass(Users user, string pass)
        {
            try
            {
                string oMessege = string.Empty;
                cAD.SetUserPassword(user.Login, pass);
                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Пароль изменен" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось изменить пароль" }; }
        }

        public ResultMode Delete(int id)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();
                Users user= db.Users.Find(id);
                cAD.DeleteUser(user.Login);
                db.Users.Remove(user);
                db.SaveChanges();
                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Пользователь удален" };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось удалить пользователя" }; }
        }

        public ResultMode AddCertificate(Users user, byte[] byteCert)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();

                Certificates newSert = new Certificates();
                CertHelper sert = new CertHelper(byteCert);
                newSert.CertificateSerialNumber = sert.SerialNumber;
                newSert.CertificateSubject = sert.Subject;
                newSert.CertificateCN = sert.CN;
                newSert.CertificateSubjectFIO = String.Format("{0} {1} {2}", sert.FIO[0], sert.FIO[1], sert.FIO[2]);
                newSert.CertificateValidFrom = sert.ValidDate;
                newSert.CertificateValidUntil = sert.ExpireDate;
                newSert.CertificateFile = byteCert;
                newSert.PublisherCertificate = sert.PublisherName;
                newSert.IdUser = user.IdUser;
                db.Certificates.Add(newSert);
                db.Entry(user).State = EntityState.Modified;
                user.Certificates = newSert;
                db.SaveChanges();
                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Сертификат добавлен", ObjectResult = newSert };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось добавить сертификат" }; }
        }

        public ResultMode AddCertificate(int idUser, byte[] byteCert)
        {
            try
            {

                Is_EnergyEntities db = new Is_EnergyEntities();

                Certificates newSert = new Certificates();
                CertHelper sert = new CertHelper(byteCert);
                newSert.CertificateSerialNumber = sert.SerialNumber;
                newSert.CertificateSubject = sert.Subject;
                newSert.CertificateCN = sert.CN;
                newSert.CertificateSubjectFIO = String.Format("{0} {1} {2}", sert.FIO[0], sert.FIO[1], sert.FIO[2]);
                newSert.CertificateValidFrom = sert.ValidDate;
                newSert.CertificateValidUntil = sert.ExpireDate;
                newSert.CertificateFile = byteCert;
                newSert.PublisherCertificate = sert.PublisherName;
                newSert.IdUser = idUser;
                db.Certificates.Add(newSert);
                Users user = db.Users.Find(idUser);
                db.Entry(user).State = EntityState.Modified;
                user.Certificates = newSert;
                db.SaveChanges();
                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Сертификат добавлен" , ObjectResult = newSert};
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось добавить сертификат" }; }
        }

        public ResultMode EditNotices(Users userNew)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();

                Users user = db.Users.Find(userNew.IdUser);
                db.Entry(user).State = EntityState.Modified;
                user.NoticesEnergyConsumptionMaquettes16 = userNew.NoticesEnergyConsumptionMaquettes16;
                user.NoticesEnergyConsumptionMaquettes80020 = userNew.NoticesEnergyConsumptionMaquettes80020;
                user.NoticesEnergyConsumptionPrimaryDocuments = userNew.NoticesEnergyConsumptionPrimaryDocuments;
                user.NoticesIncomingContractors = userNew.NoticesIncomingContractors;
                user.NoticesInDoc = userNew.NoticesInDoc;
                user.NoticesInMessages = userNew.NoticesInMessages;
                user.NoticesNews = userNew.NoticesNews;
                user.NoticesOutContractors = userNew.NoticesOutContractors;
                user.NoticesOutDoc = userNew.NoticesOutDoc;
                user.NoticesOutMessages = userNew.NoticesOutMessages;

                db.SaveChanges();
                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Уведомления успешно изменены", ObjectResult = user };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось изменить уведомления" }; }
        }

        public ResultMode ChangeOrganization(Users user)
        {
            try
            {
                Is_EnergyEntities db = new Is_EnergyEntities();

                Users useChangeOrganizationr = db.Users.Find(user.IdUser);
                db.Entry(useChangeOrganizationr).State = EntityState.Modified;
                useChangeOrganizationr.IdentifierSubscriberDefault = user.IdentifierSubscriberDefault;

                db.SaveChanges();
                return new ResultMode() { Executed = true, StrError = string.Empty, StrResult = "Организация по умолчанию успешно изменена", ObjectResult = user };
            }
            catch (Exception ex) { return new ResultMode() { Executed = false, StrError = ex.Message, StrResult = "Не удалось изменить организацию по умолчанию" }; }
        }

        public string GetCertSubjectName(Users user) {
            try
            {
                return user.Certificates.CertificateCN;
            }
            catch { return string.Empty; }
        }

        public string GetCertSubjectName(string userName)
        {
            try
            {
                Users user = Filters.AuthorizeMeAll.GetCurrentUser(userName);
                return GetCertSubjectName(user);
            }
            catch { return string.Empty; }
        }

        public byte[] GetCertHash(Users user)
        {
            try
            {
                Gost3411CryptoServiceProvider CSPHash = new Gost3411CryptoServiceProvider();
                return CSPHash.ComputeHash(user.Certificates.CertificateFile);
            }
            catch { return null; }
        }

    }

}
