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
    
    public partial class IstekListesi
    {
        public int IstekListesiId { get; set; }
        public int UrunId { get; set; }
        public System.Guid KullaniciId { get; set; }
        public System.DateTime Tarih { get; set; }
        public bool AktifMi { get; set; }
    
        public virtual Kullanici Kullanici { get; set; }
        public virtual Urun Urun { get; set; }
    }
}
