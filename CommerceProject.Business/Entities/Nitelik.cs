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
    
    public partial class Nitelik
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Nitelik()
        {
            this.PaketNitelik = new HashSet<PaketNitelik>();
            this.UrunNitelik = new HashSet<UrunNitelik>();
        }
    
        public int NitelikId { get; set; }
        public int NitelikGrupId { get; set; }
        public string Adi { get; set; }
        public string Aciklama { get; set; }
        public System.DateTime OlusturmaTarihi { get; set; }
        public System.DateTime GuncellemeTarihi { get; set; }
        public int Sira { get; set; }
        public bool AktifMi { get; set; }
    
        public virtual NitelikGrup NitelikGrup { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PaketNitelik> PaketNitelik { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UrunNitelik> UrunNitelik { get; set; }
    }
}
