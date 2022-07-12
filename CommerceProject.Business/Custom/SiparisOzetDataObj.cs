using CommerceProject.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceProject.Business.Custom
{
    public class SiparisOzetDataObj
    {
        public Guid KullaniciId { get; set; }
        public int GonderimTipId { get; set; }
        public int OdemeTipId { get; set; }
        public KrediKartiDataObj KrediKartiBilgi { get; set; }       
        public Adres AdresData { get; set; }
        public int? GonderimAdresId { get; set; }
        public int? FaturaAdresId { get; set; }
        public int? TaksitId { get; set; }
        public int? BankaId { get; set; }
        public List<Sepet> SepettekiUrunler { get; set; }
    }
}
