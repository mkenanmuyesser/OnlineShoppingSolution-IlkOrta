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
    
    public partial class IndirimKategori
    {
        public string IndirimKategoriId { get; set; }
        public int IndirimId { get; set; }
        public int KategoriId { get; set; }
        public bool AltKategorilerdeGecerliMi { get; set; }
        public bool AktifMi { get; set; }
    
        public virtual Indirim Indirim { get; set; }
        public virtual Kategori Kategori { get; set; }
    }
}
