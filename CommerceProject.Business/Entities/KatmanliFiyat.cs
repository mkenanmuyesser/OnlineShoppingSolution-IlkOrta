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
    
    public partial class KatmanliFiyat
    {
        public int KatmanliFiyatId { get; set; }
        public int UrunId { get; set; }
        public Nullable<int> SirketId { get; set; }
        public Nullable<int> RolId { get; set; }
        public int Adet { get; set; }
        public decimal Fiyat { get; set; }
        public bool AktifMi { get; set; }
    }
}