using CommerceProject.Business.Helper.Program;
using System.Collections.Generic;

namespace CommerceProject.Business.Entities
{
    public partial class Kategori
    {
        public int MenuSira { get; set; }

        public bool SeciliKategori { get; set; }

        public IEnumerable<Kategori> AltKategoriler { get; set; }

        public string Slug { get { return ProgramHelper.GenerateSlug(KategoriId, Adi); } }
    }
}