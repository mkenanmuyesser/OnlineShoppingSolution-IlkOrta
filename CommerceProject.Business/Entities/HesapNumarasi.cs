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
    
    public partial class HesapNumarasi
    {
        public int HesapNumarasiId { get; set; }
        public int BankaId { get; set; }
        public string Iban { get; set; }
        public string Sehir { get; set; }
        public string SubeKodu { get; set; }
        public string SubeAdi { get; set; }
        public string HesapNo { get; set; }
        public string HesapSahibi { get; set; }
        public int Sira { get; set; }
        public bool AktifMi { get; set; }
    
        public virtual Banka Banka { get; set; }
    }
}
