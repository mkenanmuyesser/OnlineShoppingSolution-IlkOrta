using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace CommerceProject.Business.Custom
{
    public enum CacheObject
    {
        [Description("İçerik Ayarları")]
        IcerikAyarlari = 1,

        [Description("Aktif Kategoriler")]
        AktifKategoriler = 2,

        [Description("Aktif Ürünler")]
        AktifUrunler = 3,       

        [Description("Aktif Taksitler")]
        AktifTaksitler = 4,

        [Description("Aktif Hesap Numaraları")]
        AktifHesapNumaralari = 5,

        [Description("Aktif Sık Sorulan Sorular")]
        AktifSSS = 6,
    }
}