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
    
    public partial class Countries
    {
        public Countries()
        {
            this.Address = new HashSet<Address>();
        }
    
        public int IdCountry { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Comment { get; set; }
    
        public virtual ICollection<Address> Address { get; set; }
    }
}
