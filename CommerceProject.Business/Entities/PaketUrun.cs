//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CommerceProject.Business.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class PaketUrun
    {
        public int PaketUrunId { get; set; }
        public int PaketId { get; set; }
        public int UrunId { get; set; }
        public int Adet { get; set; }
        public System.DateTime OlusturmaTarihi { get; set; }
        public System.DateTime GuncellemeTarihi { get; set; }
        public bool AktifMi { get; set; }
    
        public virtual Paket Paket { get; set; }
        public virtual Urun Urun { get; set; }
    }
}