using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceProject.Admin.Models
{
    public class RaporUrunStokHareketAnalizDataObj
    {
        public string Ay { get; set; }
        public decimal StokGirisToplamMiktar { get; set; }
        public decimal StokCikisToplamMiktar { get; set; }
    }
}