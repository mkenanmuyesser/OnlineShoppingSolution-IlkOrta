using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceProject.Business.Custom
{
    public class IcerikAyarUygulamaDataObj
    {
        public string UygulamaAd { get; set; }
        public string UygulamaAciklama { get; set; }
        public string Url { get; set; }
        public string SecureUrl { get; set; }
        public decimal ParaPuanLimit { get; set; }
        public decimal ParaPuanKazanc { get; set; }
        public int KeepAliveTime { get; set; }
        public int ClearCacheTime { get; set; }
        public bool SslAktifMi { get; set; }
        public bool UygulamaAktifMi { get; set; }
        public string FacebookHesapUrl { get; set; }
        public string TwitterHesapUrl { get; set; }
        public int CookieTime { get; set; }
        public bool ParaPuanAktifMi { get; set; }
        public bool SayfaKuponAktifMi { get; set; }
        public bool CacheAktifMi { get; set; }
    }
}