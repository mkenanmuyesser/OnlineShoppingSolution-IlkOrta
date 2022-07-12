using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommerceProject.Business.Entities;

namespace CommerceProject.Business.Custom
{
    public class IskontoUrunBilgiDataObj
    {
        public int IskontoId { get; set; }
        public List<IskontoUrun> IskonUrunListesi { get; set; }
        public List<IskontoKategori> IskontoKategoriListesi { get; set; }
        public List<IskontoMarka> IskontoMarkaListesi { get; set; }
    }
}
