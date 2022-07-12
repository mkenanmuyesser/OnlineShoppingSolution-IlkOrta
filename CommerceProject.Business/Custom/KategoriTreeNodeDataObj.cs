using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceProject.Business.Custom
{
    public class KategoriTreeNodeDataObj
    {
        public int KategoriId { get; set; }
        public string KategoriAdi { get; set; }

        public KategoriTreeNodeDataObj ParentKategori { get; set; }
        public List<KategoriTreeNodeDataObj> Children { get; set; }
    }
}
