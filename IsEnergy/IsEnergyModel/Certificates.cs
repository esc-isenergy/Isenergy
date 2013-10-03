//------------------------------------------------------------------------------
// <auto-generated>
//    Этот код был создан из шаблона.
//
//    Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//    Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IsEnergyModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Certificates
    {
        public Certificates()
        {
            this.LegalEntities = new HashSet<LegalEntities>();
            this.Users = new HashSet<Users>();
            this.SoleTraders = new HashSet<SoleTraders>();
        }
    
        public int IdCertificate { get; set; }
        public string CertificateSerialNumber { get; set; }
        public System.DateTime CertificateValidFrom { get; set; }
        public System.DateTime CertificateValidUntil { get; set; }
        public string CertificateSubjectFIO { get; set; }
        public byte[] CertificateFile { get; set; }
        public string PublisherCertificate { get; set; }
        public Nullable<int> IdUser { get; set; }
        public string CertificateSubject { get; set; }
        public string CertificateCN { get; set; }
    
        public virtual ICollection<LegalEntities> LegalEntities { get; set; }
        public virtual ICollection<Users> Users { get; set; }
        public virtual ICollection<SoleTraders> SoleTraders { get; set; }
    }
}
