using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceProject.Business.Custom
{
    public class IskontoBilgiDataObj
    {
        public int IskontoId { get; set; }
        public int IskontoTipId { get; set; }
        public string Adi { get; set; }
        public string Aciklama { get; set; }
        public decimal IskontoMiktari { get; set; }
        public decimal? MinimumFiyat { get; set; }
        public decimal? MaksimumFiyat { get; set; }
        public bool HediyeKartAktifMi { get; set; }
        public string HediyeKartKuponKod { get; set; }
        public DateTime? BaslangicTarih { get; set; }
        public DateTime? BitisTarih { get; set; }
        public int Sira { get; set; }
        public bool AktifMi { get; set; }
    }
}