using CommerceProject.Business.Custom;
using CommerceProject.Business.Helper.Program;

namespace CommerceProject.Business.Entities
{
    public partial class Paket : FiyatDataObj
    {
        public PaketResim AnaResim { get; set; }

        public PaketResim ThumbResim { get; set; }

        public decimal EskiFiyat { get; set; }

        public decimal Fiyat { get; set; }

        public int IndirimMiktari { get; set; }

        public string Slug { get { return ProgramHelper.GenerateSlug(PaketId, Adi); } }
    }
}