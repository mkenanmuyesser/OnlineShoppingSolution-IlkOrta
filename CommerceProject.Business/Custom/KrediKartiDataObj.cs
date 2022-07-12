using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceProject.Business.Custom
{
    public class KrediKartiDataObj
    {
        public int KrediKartiTipId { get; set; }
        public string KrediKartiMaskeliNumara { get; set; }
        public int Ay  { get; set; }
        public int Yil { get; set; }
        public int GuvenlikKodu { get; set; }
        public string KartNumarasi { get; set; }
        public string KartSahibi { get; set; }
        public int Taksit { get; set; }
        public int TaksitSayisi { get; set; }
    }
}
