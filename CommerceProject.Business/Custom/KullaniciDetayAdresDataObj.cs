using CommerceProject.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceProject.Business.Custom
{
    public class KullaniciDetayAdresDataObj
    {
        public Guid KullaniciId { get; set; }
        public Adres FaturaAdresi { get; set; }
        public Adres GonderimAdresi { get; set; }
    }
}
