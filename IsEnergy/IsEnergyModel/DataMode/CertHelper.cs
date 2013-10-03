using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace IsEnergyModel.DataMode
{
    public class CertHelper
    {
        private X509Certificate2 x509;
        private byte[] data;
        private string[] subject;
        private string[] publisher;

        public string PublisherName { get; private set; }
        public string INN { get; private set; }
        public string NameUser { get; private set; }
        public string CN { get; private set; }
        public string Post { get; private set; }
        public string[] FIO { get; private set; }
        public string Subject { get { return x509.Subject; } }
        public string SerialNumber { get { return x509.SerialNumber; } }
        public string Thumbprint { get { return x509.Thumbprint; } }
        public DateTime ValidDate { get { return x509.NotBefore; } }
        public DateTime ExpireDate { get { return x509.NotAfter; } }
        public string EcpBase64 { get; private set; }

        public CertHelper(byte[] data)
        {
            this.data = data;
            try
            {
                x509 = new X509Certificate2(data);
            }
            catch (Exception)
            {
            }
            EcpBase64 = Convert.ToBase64String(data);
            try
            {
                publisher = x509.IssuerName.Name.Split(new char[] { ',' });

                subject = x509.Subject.Split(new char[] { ',' });

                FIO = new string[3];
                Post = "0";

            }
            catch (Exception)
            {
            }
            try
            {
                foreach (string s in publisher)
                {

                    string[] a = s.Split(new char[] { '=' });
                    switch (a[0])
                    {
                        case "O":
                            PublisherName = a[1];
                            break;
                        case " O":
                            PublisherName = a[1];
                            break;
                    }
                }

                foreach (string s in subject)
                {

                    string[] a = s.Split(new char[] { '=' });
                    switch (a[0])
                    {
                        case "ИНН":
                            INN = a[1];
                            break;
                        case " ИНН":
                            INN = a[1];
                            break;
                        case "SN":
                            for (int i = 0; i < 3; i++)
                                FIO[i] = a[1].Split(new char[] { ' ' })[i];
                            break;
                        case " SN":
                            for (int i = 0; i < 3; i++)
                                FIO[i] = a[1].Split(new char[] { ' ' })[i];
                            break;

                        case "G":
                            for (int i = 0; i < 2; i++)
                                FIO[i + 1] = a[1].Split(new char[] { ' ' })[i];
                            break;
                        case " G":
                            for (int i = 0; i < 2; i++)
                                FIO[i + 1] = a[1].Split(new char[] { ' ' })[i];
                            break;
                        case "T":
                            Post = a[1];
                            break;
                        case " T":
                            Post = a[1];
                            break;
                        case "O":
                            NameUser = a[1];
                            break;
                        case " O":
                            NameUser = a[1];
                            break;
                        case "CN":
                            CN = a[1];
                            break;
                        case " CN":
                            CN = a[1];
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

    }


}
