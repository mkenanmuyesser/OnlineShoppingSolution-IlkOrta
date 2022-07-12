using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceProject.Admin.Models
{
    public class UrunDusukStokDataObj
    {
        public int UrunId { get; set; }
        public int StokAlarmSeviyesi { get; set; }
        public decimal ToplamGirisAdedi { get; set; }
        public decimal ToplamCikisAdedi { get; set; }
    }
}