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
    
    public partial class HediyeKartKullanim
    {
        public int HediyeKartKullanimId { get; set; }
        public int HediyeKartId { get; set; }
        public int SiparisId { get; set; }
        public decimal KullanilanMiktar { get; set; }
        public System.DateTime Tarih { get; set; }
    
        public virtual HediyeKart HediyeKart { get; set; }
        public virtual Siparis Siparis { get; set; }
    }
}
