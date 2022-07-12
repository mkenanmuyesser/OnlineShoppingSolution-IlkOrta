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
    
    public partial class Kategori
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Kategori()
        {
            this.IskontoKategori = new HashSet<IskontoKategori>();
            this.Kategori1 = new HashSet<Kategori>();
            this.KategoriOzellik = new HashSet<KategoriOzellik>();
            this.PaketKategori = new HashSet<PaketKategori>();
            this.UrunKategori = new HashSet<UrunKategori>();
        }
    
        public int KategoriId { get; set; }
        public Nullable<int> UstKategoriId { get; set; }
        public Nullable<int> VergiId { get; set; }
        public string Adi { get; set; }
        public string Aciklama { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string Resim { get; set; }
        public System.DateTime OlusturmaTarihi { get; set; }
        public System.DateTime GuncellemeTarihi { get; set; }
        public Nullable<System.DateTime> Tarih { get; set; }
        public int Sira { get; set; }
        public bool AktifMi { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IskontoKategori> IskontoKategori { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Kategori> Kategori1 { get; set; }
        public virtual Kategori Kategori2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KategoriOzellik> KategoriOzellik { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PaketKategori> PaketKategori { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UrunKategori> UrunKategori { get; set; }
    }
}