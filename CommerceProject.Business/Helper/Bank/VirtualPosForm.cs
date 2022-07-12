using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceProject.Business.Helper.Bank
{
    public class VirtualPosForm
    {
        public string siparisId { get; set; }
        public string kartSahibi { get; set; }
        public long kartNumarasi { get; set; }
        public int guvenlikKodu { get; set; }
        public int ay { get; set; }
        public int yil { get; set; }
        public double tutar { get; set; }
        public int taksit { get; set; }
    }
}
