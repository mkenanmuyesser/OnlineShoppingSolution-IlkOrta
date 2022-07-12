using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceProject.Business.Custom
{
    public class IcerikAyarEpostaDataObj
    {
        public string GonderilecekEposta { get; set; }
        public string GonderilecekEpostaTanim { get; set; }
        public string GonderilecekEpostaKullaniciAdi { get; set; }
        public string GonderilecekEpostaSifre { get; set; }
        public string GonderilecekEpostaHost { get; set; }
        public int GonderilecekEpostaPort { get; set; }
        public bool GonderilecekEpostaAktifMi { get; set; }
    }
}
