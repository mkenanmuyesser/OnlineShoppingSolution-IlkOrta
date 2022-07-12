using CommerceProject.Business.Custom;
using CommerceProject.Business.Helper.Program;

namespace CommerceProject.Business.Entities
{
    public partial class Urun : FiyatDataObj
    {
        public UrunResim AnaResim { get; set; }

        public UrunResim ThumbResim { get; set; }

        public int YorumSayisi { get; set; }

        public decimal YorumOrtalama { get; set; }

        public int IndirimMiktari { get; set; }

        public int ToplamSatisSayisi { get; set; }

        public decimal UrunBirimKdvDahilTutar { get; set; }

        public decimal UrunBirimKdvHaricTutar { get; set; }

        public decimal UrunBirimKdvTutar { get; set; }

        public string Slug { get { return ProgramHelper.GenerateSlug(UrunId, Adi); } }
    }
}