using CommerceProject.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceProject.Business.Custom
{
    [Serializable]
    public class SiparisSonucDataObj
    {
        public bool SiparisBasariliMi { get; set; }
        public int SiparisId { get; set; }
        public int? BankaId { get; set; }
        public string TransferId { get; set; }
        public Guid KullaniciId { get; set; }
        public Kullanici Kullanici { get; set; }
        public Adres GonderimAdres { get; set; }
        public Adres FaturaAdres { get; set; }
        public DateTime SiparisTarihi { get; set; }
        public string OdemeTipi { get; set; }    
        public decimal ToplamIskonto { get; set; }
        public decimal ToplamKomisyon { get; set; }
        public decimal ParaPuanMiktar { get; set; }
        public decimal KuponMiktar { get; set; }
        public decimal OdemeTipUcreti { get; set; }
        public decimal GonderimUcreti { get; set; }
        public decimal OdenecekTutar { get; set; }
        public FiyatDataObj UrunToplam { get; set; }
        public List<Sepet> SepettekiUrunler { get; set; }
        public KrediKartiDataObj KrediKartiBilgi { get; set; }
    }
}
