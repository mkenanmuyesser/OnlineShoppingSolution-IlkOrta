using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommerceProject.Business.Entities;

namespace CommerceProject.Business.Custom
{
    public class IskontoSepetBilgiDataObj
    {
        public int IskontoId { get; set; }
        public List<IskontoRol> IskontoRolListesi { get; set; }
        public List<IskontoSirket> IskontoSirketListesi { get; set; }
    }
}
