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
    
    public partial class AdresIlce
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AdresIlce()
        {
            this.Adres = new HashSet<Adres>();
        }
    
        public int AdresIlceId { get; set; }
        public int AdresIlId { get; set; }
        public string IlceAdi { get; set; }
        public bool AktifMi { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Adres> Adres { get; set; }
        public virtual AdresIl AdresIl { get; set; }
    }
}