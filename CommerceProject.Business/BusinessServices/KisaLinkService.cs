using CommerceProject.Business.BusinessContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;
using CommerceProject.Business.Helper.Cryptography;
using CommerceProject.Business.Custom;

namespace CommerceProject.Business.BusinessServices
{
    public class KisaLinkService : GenericRepository<KisaLink>, IKisaLinkService
    {
        private readonly IIcerikAyarService icerikAyarService = new IcerikAyarService();

        public string GenerateShortLink()
        {
            string siteUrl = icerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari).Url;
            string shortUrl = CryptoHelper.GetUniqueKey(8);
            string siteShortUrl = string.Format("{0}/?{1}", siteUrl, shortUrl);

            //eğer daha önce üretilmiş bir link çakışırsa yenisini üret
            if (this.FindBy(x => x.AktifMi == true && x.KisaltilmisLink == siteShortUrl).Any())
                return this.GenerateShortLink();
            else
                return siteShortUrl;
        }

        public string GetLongLink(string shortUrl)
        {
            string siteUrl = icerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari).Url;
            var kisaLink = this.GetSingle(x => x.AktifMi && x.KisaltilmisLink == shortUrl);

            if (kisaLink == null)
            {
                return siteUrl;
            }
            else
            {
                return kisaLink.UzunLink;
            }
        }

        public bool AddVisitor(string shortUrl)
        {
            var kisaLink = this.GetSingle(x => x.AktifMi && x.KisaltilmisLink == shortUrl);

            if (kisaLink == null)
            {
                return false;
            }
            else
            {
                kisaLink.ZiyaretSayisi = kisaLink.ZiyaretSayisi + 1;

                this.Edit(kisaLink);
                return this.Save();
            }
        }
    }
}
