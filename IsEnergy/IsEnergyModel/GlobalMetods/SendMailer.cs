using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;


namespace IsEnergyModel.GlobalMetods
{
    public class Messages
    {
        private static string smtp;
        private static int port;
        private static string login;
        private static string pass;

        public static bool Send_mail(string Заголовок, string Сообщение, string to_mail)
        {
            MailMessage Message = new MailMessage();
            //Формирование письма
            Message.From = new MailAddress(login);
            Message.To.Add(new MailAddress(to_mail));
            Message.Subject = Заголовок;
            Message.Body = Сообщение;

            //Авторизация на SMTP сервере
            SmtpClient Smtp = new SmtpClient(smtp, port);
            Smtp.Credentials = new NetworkCredential(login, pass);
            Smtp.Send(Message);//отправка
            return true;
        }

        public static bool Send_mail(string Заголовок, string Сообщение, string[] to_mail)
        {
            MailMessage Message = new MailMessage();
            //Формирование письма
            Message.From = new MailAddress(login);
            foreach (string ToMail in to_mail)
            {
                Message.To.Add(new MailAddress(ToMail));
            }
            Message.Subject = Заголовок;
            Message.Body = Сообщение;
            //Авторизация на SMTP сервере
            SmtpClient Smtp = new SmtpClient(smtp, port);
            Smtp.Credentials = new NetworkCredential(login, pass);
            Smtp.Send(Message);//отправка
            return true;
        }

        public static bool Send_mailFor80020(string отправитель,string Заголовок, string Сообщение, string to_mail, byte[] filedata, string fileName, byte[] filedata2, string fileName2)
        {
            try
            {
                MailMessage Message = new MailMessage();
                ////var message = new MailMessage("somemail@gmail.com", "somemail@mail.com", "test", "message");
                //Формирование письма
                Message.From = new MailAddress(отправитель);
                Message.To.Add(new MailAddress(to_mail));
                Message.Subject = Заголовок;
                Message.Body = Сообщение;
                FileStream fs = File.Create("C:\\rar1\\" + fileName);
                FileStream fs2 = File.Create("C:\\rar1\\" + fileName + ".sig");
                fs.Write(filedata, 0, filedata.Length);
                fs2.Write(filedata2, 0, filedata2.Length);
                fs.Close();
                fs2.Close();

                //Прикрепляем файл
                Attachment attach = new Attachment("C:\\rar1\\" + fileName, MediaTypeNames.Application.Octet);

                Message.Attachments.Add(attach);
                attach = new Attachment("C:\\rar1\\" + fileName + ".sig", MediaTypeNames.Application.Octet);
                Message.Attachments.Add(attach);
                //Авторизация на SMTP сервере
                SmtpClient Smtp = new SmtpClient(smtp, port);
                Smtp.Credentials = new NetworkCredential(login, pass);
                Smtp.Send(Message);//отправка
                Message.Dispose();

                File.Delete("C:\\rar1\\" + fileName);
                File.Delete("C:\\rar1\\" + fileName + ".sig");
                return true;
            }
            catch { return false; }
            
        }

        public static bool Send_mail(string Заголовок, string Сообщение, string[] to_mail, string fileName)
        {
            MailMessage Message = new MailMessage();
            //Формирование письма
            Message.From = new MailAddress(login);
            foreach (string ToMail in to_mail)
            {
                Message.To.Add(new MailAddress(ToMail));
            }
            Message.Subject = Заголовок;
            Message.Body = Сообщение;

            //Прикрепляем файл
            Attachment attach = new Attachment(fileName, MediaTypeNames.Application.Octet);
            Message.Attachments.Add(attach);
            //Авторизация на SMTP сервере
            SmtpClient Smtp = new SmtpClient(smtp, port);
            Smtp.Credentials = new NetworkCredential(login, pass);
            Smtp.Send(Message);//отправка
            return true;
        }

        public static bool Send_mail(string Заголовок, string Сообщение, string to_mail, string[] fileName)
        {
            MailMessage Message = new MailMessage();
            //Формирование письма
            Message.From = new MailAddress(login);
           Message.To.Add(new MailAddress(to_mail));
            Message.Subject = Заголовок;
            Message.Body = Сообщение;

            //Прикрепляем файл
            foreach (string fileName_ in fileName)
            {
                Attachment attach = new Attachment(fileName_, MediaTypeNames.Application.Octet);
                Message.Attachments.Add(attach);
            }
            //Авторизация на SMTP сервере
            SmtpClient Smtp = new SmtpClient(smtp, port);
            Smtp.Credentials = new NetworkCredential(login, pass);
            Smtp.Send(Message);//отправка
            return true;
        }

        public static bool Send_mail(string Заголовок, string Сообщение, string[] to_mail, string[] fileName)
        {
            MailMessage Message = new MailMessage();
            //Формирование письма
            Message.From = new MailAddress(login);
            foreach (string ToMail in to_mail)
            {
                Message.To.Add(new MailAddress(ToMail));
            }
            Message.Subject = Заголовок;
            Message.Body = Сообщение;

            //Прикрепляем файл
            foreach (string fileName_ in fileName)
            {
                Attachment attach = new Attachment(fileName_, MediaTypeNames.Application.Octet);
                Message.Attachments.Add(attach);
            }
            //Авторизация на SMTP сервере
            SmtpClient Smtp = new SmtpClient(smtp, port);
            Smtp.Credentials = new NetworkCredential(login, pass);
            Smtp.Send(Message);//отправка
            return true;
        }

        public static  void STMP_Registration(string stmp_, int port_, string login_, string pass_)
        {
            smtp = stmp_;
            port = port_;
            login = login_;
            pass = pass_;
        }

    }
}
