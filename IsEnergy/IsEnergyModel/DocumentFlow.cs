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
    
    public partial class DocumentFlow
    {
        public int IdDocumentFlow { get; set; }
        public int IdDocumentMain { get; set; }
        public string NameDocument { get; set; }
        public string NameSign { get; set; }
        public byte[] DataSign { get; set; }
        public System.DateTime DateCreate { get; set; }
        public string Comment { get; set; }
        public Nullable<int> IndexDocumentFlow { get; set; }
        public byte[] DataFile { get; set; }
        public string NameFile { get; set; }
        public int State { get; set; }
    
        public virtual Documents Documents { get; set; }
    }
}
