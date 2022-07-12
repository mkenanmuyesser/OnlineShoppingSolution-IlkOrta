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
    
    public partial class Adres
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Adres()
        {
            this.KullaniciAdres = new HashSet<KullaniciAdres>();
            this.Siparis = new HashSet<Siparis>();
            this.Siparis1 = new HashSet<Siparis>();
        }
    
        public int AdresId { get; set; }
        public int FaturaTipId { get; set; }
        public string AdresAdi { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string TcNo { get; set; }
        public string FirmaUnvan { get; set; }
        public string VergiNo { get; set; }
        public string VergiDairesi { get; set; }
        public string AdresBilgi { get; set; }
        public string Aciklama { get; set; }
        public string PostaKodu { get; set; }
        public int AdresIlceId { get; set; }
        public int AdresIlId { get; set; }
        public string Telefon1 { get; set; }
        public string Telefon2 { get; set; }
        public System.DateTime Tarih { get; set; }
        public bool AktifMi { get; set; }
    
        public virtual AdresIl AdresIl { get; set; }
        public virtual AdresIlce AdresIlce { get; set; }
        public virtual FaturaTip FaturaTip { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KullaniciAdres> KullaniciAdres { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Siparis> Siparis { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Siparis> Siparis1 { get; set; }
    }
}
