using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommerceProject.Business.Helper.Caching;
using CommerceProject.Business.Helper.Bank;
using System.Text;

namespace CommerceProject.Tests
{
    [TestClass]
    public class BankTests
    {
        [TestMethod]
        public void BankTest1()
        {
            StringBuilder sb = new StringBuilder();

            // kullanıcıdan kart bilgilerini alıyoruz.
            VirtualPosForm pf = new VirtualPosForm
            {
                ay = 1,
                yil = 2011,
                guvenlikKodu = 123,
                kartNumarasi = 1234567891234568,
                kartSahibi = "kart sahibi",
                taksit = 3,
                tutar = 1.00
            };

            // Poslarımıza yukarıdaki bilgileri gönderiyoruz.

            // Örnek gönderim;
            var resultForm = BankHelper.AkBank(pf);
            //p.GarantiBankasi(pf);
            //p.VakifBank(pf);
            //p.YapiKredi(pf);
            //p.IsBankasi(pf);

            // Poslardan geriye dönen bilgileri alıyoruz.

            if (resultForm.sonuc)
            {
                // Çekim işlemi başarılı ise, geri dönen bilgileri alıyoruz.
                // Genellikle bu bilgiler veritabanında saklanır.
                // Bankadan bankaya değişiklik göstereceği için, alanlardan bazıları boş gelebilir.
                sb.Append(resultForm.referansNo);
                sb.Append(resultForm.groupId);
                sb.Append(resultForm.transId);
                sb.Append(resultForm.code);
            }
            else
            {
                // Çekim işlemi herhangi bir sebepden dolayı olumsuz sonuçlanmışsa, bankadan dönen hatayı alıyoruz.
                // Hata kodlarının açıklamaları ilgili banka dökümantasyonunda belirtilmiştir.
                sb.Append(resultForm.sonuc);
                sb.Append(resultForm.hataMesaji);
                sb.Append(resultForm.hataKodu);
            }

            Assert.IsNotNull(sb.ToString());
        }
    }
}
