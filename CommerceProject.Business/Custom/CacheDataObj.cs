using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace CommerceProject.Business.Custom
{
    //buraya eklenen her bir cache objesinin veritabanında da karşılığı olmalı. Program.Cache tablonun adı
    public enum CacheDataObj
    {
        [Description("İçerik Ayarları")]
        IcerikAyarlari = 1,

        [Description("Aktif Kategoriler")]
        AktifKategoriler = 2,

        [Description("Aktif Ürünler")]
        AktifUrunler = 3,

        [Description("Aktif Paketler")]
        AktifPaketler = 4,

        [Description("Aktif Taksitler")]
        AktifTaksitler = 5,

        [Description("Aktif Hesap Numaraları")]
        AktifHesapNumaralari = 6,

        [Description("Aktif Sık Sorulan Sorular")]
        AktifSSS = 7,

        [Description("Aktif Markalar")]
        AktifMarkalar = 8,

        [Description("Aktif Gönderimler")]
        AktifGonderimler = 9,

        [Description("Aktif Sipariş Ödeme Tipleri")]
        AktifSiparisOdemeTipleri = 10,

        [Description("Aktif Adres İller")]
        AktifAdresIller = 11,

        [Description("Aktif Adres İlçeler")]
        AktifAdresIlceler = 12,
    }
}