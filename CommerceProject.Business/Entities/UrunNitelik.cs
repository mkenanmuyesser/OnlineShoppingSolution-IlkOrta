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
    
    public partial class UrunNitelik
    {
        public int UrunNitelikId { get; set; }
        public int UrunId { get; set; }
        public int NitelikId { get; set; }
        public string NitelikDegeri { get; set; }
        public int Sira { get; set; }
        public bool AktifMi { get; set; }
    
        public virtual Nitelik Nitelik { get; set; }
        public virtual Urun Urun { get; set; }
    }
}
