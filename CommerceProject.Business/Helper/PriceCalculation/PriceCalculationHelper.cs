using CommerceProject.Business.BusinessContracts;
using CommerceProject.Business.BusinessServices;
using CommerceProject.Business.Custom;
using CommerceProject.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceProject.Business.Helper.PriceCalculation
{
    public class PriceCalculationHelper
    {
        private static IIcerikAyarService IcerikAyarService = new IcerikAyarService();
        private static IGonderimService GonderimService = new GonderimService();
        private static ISiparisOdemeTipService SiparisOdemeTipService = new SiparisOdemeTipService();
        private static ITaksitService TaksitService = new TaksitService();

        public static FiyatDataObj PriceDetail(decimal price, decimal tax)
        {
            var icerikAyar = IcerikAyarService.GetFirst();

            FiyatDataObj fiyatDataObj;

            if (icerikAyar.KdvDahilMi)
            {
                fiyatDataObj = new FiyatDataObj
                {
                    KdvDahilTutar = price,
                    KdvHaricTutar = price / (1 + tax),
                    KdvTutar = price - (price / (1 + tax))
                };
            }
            else
            {
                fiyatDataObj = new FiyatDataObj
                {
                    KdvHaricTutar = price,
                    KdvTutar = price * tax,
                    KdvDahilTutar = price + (price * tax)
                };
            }

            return fiyatDataObj;
        }

        public static SiparisSonucDataObj PriceSummary(SiparisOzetDataObj siparisOzetDataObj)
        {
            SiparisSonucDataObj siparisSonucDataObj = new SiparisSonucDataObj();

            if (siparisOzetDataObj.SepettekiUrunler.Any() && siparisOzetDataObj.GonderimTipId > 0 && siparisOzetDataObj.OdemeTipId > 0)
            {
                Guid kullaniciId = siparisOzetDataObj.KullaniciId;
                decimal toplamIskonto = 0;
                decimal toplamKomisyon = 0;

                decimal paraPuanMiktar = 0;
                decimal kuponMiktar = 0;

                decimal urunToplamKdvDahilTutar = 0;
                decimal urunToplamKdvHaricTutar = 0;
                decimal urunToplamKdvTutar = 0;

                decimal odemeTipUcreti = 0;
                decimal gonderimUcreti = 0;

                var siparisOdemeTip = SiparisOdemeTipService.GetById(siparisOzetDataObj.OdemeTipId);

                // Aşama 1 - Ürünler bazında fiyat oluşturulacak
                Sepet[] sepettekiUrunler = new Sepet[siparisOzetDataObj.SepettekiUrunler.Count];
                siparisOzetDataObj.SepettekiUrunler.CopyTo(sepettekiUrunler);
                foreach (var sepettekiUrun in sepettekiUrunler)
                {
                    var sepet = siparisOzetDataObj.SepettekiUrunler.Single(x => x.SepetId  == sepettekiUrun.SepetId && x.UrunId==sepettekiUrun.UrunId);

                    var urun = sepet.Urun;
                    decimal urunBirimFiyat = 0;
                    // Ürün özel fiyat aralığında mı?
                    if (urun.OzelFiyatAktifMi &&
                        (urun.OzelFiyatBaslangicTarihi.HasValue && urun.OzelFiyatBaslangicTarihi.Value <= DateTime.Now) &&
                        (urun.OzelFiyatBitisTarihi.HasValue && urun.OzelFiyatBitisTarihi >= DateTime.Now))
                    {
                        urunBirimFiyat = urun.OzelFiyat.Value;
                    }
                    else
                    {
                        urunBirimFiyat = urun.Fiyat;
                    }

                    var urunBirimFiyatDataObj = PriceDetail(urunBirimFiyat, urun.Vergi.VergiOrani);

                    sepet.Urun.UrunBirimKdvDahilTutar = urunBirimFiyatDataObj.KdvDahilTutar;
                    sepet.Urun.UrunBirimKdvHaricTutar = urunBirimFiyatDataObj.KdvHaricTutar;
                    sepet.Urun.UrunBirimKdvTutar = urunBirimFiyatDataObj.KdvTutar;

                    // TODO: burası bitirilmedi. kullanıcı ile ilgili indirimler ve oluşan fiyat burada yansıtılacak... 
                    // Ayrıca kupon ve parapuan gibi şeyler de burada yer alacak. parapuan sistemi entegre edilince koşul "toplam miktar kişinin parapuanından daha fazla olacak" şeklinde ayarlanacaktır.          

                    // Aşama 2 - Ürüne ait özel iskonto var mı?
                    // Burada iskonto urun, iskonto sirket, iskonto marka, iskonto kategori tablolarına bakılacak.
                    // TODO: bu tabloların kullanımına karar verilecek.
                    // toplamIskonto +=

                    // Aşama 3 - Kupon var nı? Kullanıldı mı?
                    // TODO: kullanıma karar verilecek
                    // kuponMiktar +=

                    // Aşama 4 - Para puan var mı? Kullanıldı mı?
                    // TODO: kullanıma karar verilecek
                    // paraPuanMiktar +=


                    // Aşama 5 - Ödeme tipine göre ürün fiyatları belirlenecek
                    // Seçilen ödeme yöntemine göre iskonto veya komisyon var mı?
                    if (siparisOdemeTip != null)
                    {
                        // Kredi kartı seçildiyse
                        if (siparisOdemeTip.SiparisOdemeTipId == 1)
                        {
                            // Taksit seçildi mi seçilmedi mi? 
                            var taksit = TaksitService.GetById(siparisOzetDataObj.TaksitId.Value);

                            if (taksit != null && taksit.TaksitSayisi > 0)
                            {
                                // Taksit seçildiyse taksite göre fiyat hesaplanacak, faiz oranı eklenecek
                                decimal aylikTaksit = urunBirimFiyat / (decimal)taksit.TaksitSayisi;
                                taksit.AylikOdeme = aylikTaksit + (aylikTaksit * taksit.FaizOrani);
                                taksit.ToplamTutar = taksit.AylikOdeme * taksit.TaksitSayisi;

                                // Toplam komisyona ekle
                                toplamKomisyon += urunBirimFiyat - taksit.ToplamTutar;

                                urunBirimFiyat = taksit.ToplamTutar;
                            }
                            else
                            {
                                // Taksit seçilmediyse ödeme tipine göre komisyon veya iskonto var ise eklenecek
                                if (siparisOdemeTip.Miktar > 0)
                                {
                                    // Komisyon 1-2
                                    // İskonto 3-4 
                                    switch (siparisOdemeTip.KomisyonIskontoTipId)
                                    {
                                        case 1:
                                            toplamIskonto += urunBirimFiyat * (siparisOdemeTip.Miktar / 100m);
                                            urunBirimFiyat = urunBirimFiyat - toplamIskonto;
                                            break;
                                        case 3:
                                            toplamKomisyon += urunBirimFiyat * (siparisOdemeTip.Miktar / 100m);
                                            urunBirimFiyat = urunBirimFiyat + toplamKomisyon;
                                            break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Ürün bazlı oran eklenecek. Ancak 2 ve 4 tipleri toplam ürün fiyatına ekleneceği için onları ürün bazlı değil toplam bazlı hesaplayacağız.
                            // Komisyon 1-2
                            // İskonto 3-4 
                            switch (siparisOdemeTip.KomisyonIskontoTipId)
                            {
                                case 1:
                                    urunBirimFiyat = siparisOdemeTip.Miktar == 0 ? urunBirimFiyat : urunBirimFiyat - (urunBirimFiyat * (siparisOdemeTip.Miktar / 100m));
                                    break;
                                case 3:
                                    urunBirimFiyat = siparisOdemeTip.Miktar == 0 ? urunBirimFiyat : urunBirimFiyat + (urunBirimFiyat * (siparisOdemeTip.Miktar / 100m));
                                    break;
                            }
                        }
                    }

                    // Ürün toplam fiyatına ekle
                    var urunAraToplam = urunBirimFiyat * sepet.Adet;

                    var fiyatData = PriceDetail(urunAraToplam, urun.Vergi.VergiOrani);
                    urunToplamKdvDahilTutar += fiyatData.KdvDahilTutar;
                    urunToplamKdvHaricTutar += fiyatData.KdvHaricTutar;
                    urunToplamKdvTutar += fiyatData.KdvTutar;

                    sepet.Urun.KdvDahilTutar = fiyatData.KdvDahilTutar;
                    sepet.Urun.KdvHaricTutar = fiyatData.KdvHaricTutar;
                    sepet.Urun.KdvTutar = fiyatData.KdvTutar;
                }

                // Aşama 6 - Ödeme tipine göre ücret farkı
                if (siparisOdemeTip != null && siparisOdemeTip.Miktar > 0)
                {
                    // Kredi kartı ödeme tipi seçilip taksit secilmediyse veya ödeme tipi diğerleriyse toplam farka yansıtılacak
                    if ((siparisOdemeTip.SiparisOdemeTipId == 1 && siparisOzetDataObj.TaksitId != null && siparisOzetDataObj.TaksitId == 0) ||
                        siparisOdemeTip.SiparisOdemeTipId != 1)
                    {
                        // Komisyon 1-2
                        // İskonto 3-4 
                        switch (siparisOdemeTip.KomisyonIskontoTipId)
                        {
                            case 2:
                            case 4:
                                odemeTipUcreti += siparisOdemeTip.Miktar;
                                break;
                        }
                    }
                }

                // Aşama - 7 Gönderim ücreti varsa ona ekle
                var gonderim = GonderimService.GetById(siparisOzetDataObj.GonderimTipId);
                if (gonderim != null)
                {
                    if (gonderim.AltLimit > 0)
                    {
                        //toplam fiyat alt limiti geçmiş mi?
                        if (urunToplamKdvDahilTutar < gonderim.AltLimit)
                        {
                            gonderimUcreti = gonderim.Fiyat;
                        }
                    }
                    else
                    {
                        gonderimUcreti = gonderim.Fiyat;
                    }
                }

                var priceDetail = new FiyatDataObj
                {
                    KdvDahilTutar = urunToplamKdvDahilTutar,
                    KdvHaricTutar = urunToplamKdvHaricTutar,
                    KdvTutar = urunToplamKdvTutar,
                };

                // Buradaki iskonto ve komisyon bilgileri hesaplamaya sokulmuyor, bilgi amacıyla alınıyor.
                siparisSonucDataObj.ToplamIskonto = toplamIskonto;
                siparisSonucDataObj.ToplamKomisyon = toplamKomisyon;

                siparisSonucDataObj.KullaniciId = siparisOzetDataObj.KullaniciId;

                siparisSonucDataObj.ParaPuanMiktar = paraPuanMiktar;
                siparisSonucDataObj.KuponMiktar = kuponMiktar;

                siparisSonucDataObj.GonderimUcreti = gonderimUcreti;
                siparisSonucDataObj.OdemeTipUcreti = odemeTipUcreti;
                siparisSonucDataObj.UrunToplam = priceDetail;
                siparisSonucDataObj.OdenecekTutar = urunToplamKdvDahilTutar + odemeTipUcreti + gonderimUcreti - kuponMiktar - paraPuanMiktar;
                siparisSonucDataObj.SepettekiUrunler = siparisOzetDataObj.SepettekiUrunler;
            }
            else
            {
                siparisSonucDataObj = null;
            }

            return siparisSonucDataObj;
        }
    }
}
