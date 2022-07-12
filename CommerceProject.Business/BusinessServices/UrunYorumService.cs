using CommerceProject.Business.BusinessContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;

namespace CommerceProject.Business.BusinessServices
{
    public class UrunYorumService : GenericRepository<UrunYorum>, IUrunYorumService
    {
        public List<UrunYorum> GetUserComments(Guid kullaniciId)
        {
            var yorumlar = this.FindBy(x => x.AktifMi == true &&
                                            x.KullaniciId == kullaniciId,
                                            false).ToList();

            return yorumlar;
        }

        public bool AddUserComment(Guid kullaniciId, int urunId, int puan, string yorum)
        {
            var urunYorum = new UrunYorum()
            {
                KullaniciId = kullaniciId,
                UrunId = urunId,
                Yorum = yorum,
                Puan = puan,
                Tarih = DateTime.Now,
                GosterilsinMi = false,
                AktifMi = true
            };

            this.Add(urunYorum);
            return this.Save();
        }
    }
}
