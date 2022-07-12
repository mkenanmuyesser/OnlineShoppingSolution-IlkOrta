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
    
    public partial class Urun
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Urun()
        {
            this.IskontoUrun = new HashSet<IskontoUrun>();
            this.IstekListesi = new HashSet<IstekListesi>();
            this.Sepet = new HashSet<Sepet>();
            this.SiparisDetay = new HashSet<SiparisDetay>();
            this.IlgiliUrun = new HashSet<IlgiliUrun>();
            this.IlgiliUrun1 = new HashSet<IlgiliUrun>();
            this.PaketUrun = new HashSet<PaketUrun>();
            this.StokHareket = new HashSet<StokHareket>();
            this.UrunResim = new HashSet<UrunResim>();
            this.UrunKategori = new HashSet<UrunKategori>();
            this.UrunNitelik = new HashSet<UrunNitelik>();
            this.UrunOzellik = new HashSet<UrunOzellik>();
            this.UrunYorum = new HashSet<UrunYorum>();
        }
    
        public int UrunId { get; set; }
        public Nullable<int> MarkaId { get; set; }
        public int VergiId { get; set; }
        public int NitelikGrupId { get; set; }
        public string Barkod { get; set; }
        public string UrunKod { get; set; }
        public string Adi { get; set; }
        public string KisaAciklama { get; set; }
        public string UzunAciklama { get; set; }
        public string Tags { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string OrnekDosyaYolu { get; set; }
        public bool YorumYapilabilsinMi { get; set; }
        public int StokMiktar { get; set; }
        public bool StoktaVarMi { get; set; }
        public bool StokMiktariGosterilsinMi { get; set; }
        public int StokAlarmSeviyesi { get; set; }
        public int MinimumSiparisMiktari { get; set; }
        public int MaksimumSiparisMiktari { get; set; }
        public bool SatinAlAktifMi { get; set; }
        public bool OnSiparisAktifMi { get; set; }
        public bool OzelFiyatAktifMi { get; set; }
        public Nullable<System.DateTime> OzelFiyatBaslangicTarihi { get; set; }
        public Nullable<System.DateTime> OzelFiyatBitisTarihi { get; set; }
        public decimal Maliyet { get; set; }
        public decimal Fiyat { get; set; }
        public decimal EskiFiyat { get; set; }
        public Nullable<decimal> OzelFiyat { get; set; }
        public Nullable<System.DateTime> BaslangicTarihi { get; set; }
        public Nullable<System.DateTime> BitisTarihi { get; set; }
        public System.DateTime OlusturmaTarihi { get; set; }
        public System.DateTime GuncellemeTarihi { get; set; }
        public int Sira { get; set; }
        public bool AktifMi { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IskontoUrun> IskontoUrun { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IstekListesi> IstekListesi { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sepet> Sepet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SiparisDetay> SiparisDetay { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IlgiliUrun> IlgiliUrun { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IlgiliUrun> IlgiliUrun1 { get; set; }
        public virtual Marka Marka { get; set; }
        public virtual NitelikGrup NitelikGrup { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PaketUrun> PaketUrun { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StokHareket> StokHareket { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UrunResim> UrunResim { get; set; }
        public virtual Vergi Vergi { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UrunKategori> UrunKategori { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UrunNitelik> UrunNitelik { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UrunOzellik> UrunOzellik { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UrunYorum> UrunYorum { get; set; }
    }
}
