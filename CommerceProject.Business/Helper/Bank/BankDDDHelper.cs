using ArbakCCLib.ENTITY;
using ArbakCCLib.MANAGER;
using CommerceProject.Business.BusinessContracts;
using CommerceProject.Business.BusinessServices;
using CommerceProject.Business.Custom;
using CommerceProject.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceProject.Business.Helper.Bank
{
    public class BankDDDHelper
    {
        private static BankDDDHelper controller;
        public static BankDDDHelper Controller
        {
            get { if (controller == null) controller = new BankDDDHelper(); return controller; }
        }

        private CCManager ccManager;
        public CCManager CCManager
        {
            get { return ccManager; }
        }

        private BankDDDHelper()
        {
            IIcerikAyarService icerikAyarService = new IcerikAyarService();
            IcerikAyar icerikAyar = icerikAyarService.GetFirstFromCache(CacheDataObj.IcerikAyarlari);
            ISanalPosService sanalPosService = new SanalPosService();

            List<PosInfo> posInfoList = new List<PosInfo>();

            foreach (var sanalPos in sanalPosService.FindBy(x => x.AktifMi && x.DDDAktifMi && x.Banka.AktifMi))
            {
                switch (sanalPos.SanalPosId)
                {
                    case 1:
                        var posInfoGaranti = new GarantiPosInfo()
                        {
                            //Bank = ArbakCCLib.ENTITY.ENUMS.Banks.GARANTI,
                            //TerminalID = "10068200",
                            //MerchantID = "8687330",
                            //TerminalUserID = "xxxxxx",
                            //BankName = "garanti",
                            //ClientID = "xxx",
                            //Name = "GARANTI",
                            //Password = "416161313233313233313233313233313233313233313233",
                            //ProvisionPassword = "Aaa123123*",
                            //PosType = ArbakCCLib.ENTITY.ENUMS.PosType.GARANTI,
                            //PosURL = "https://sanalposprov.garanti.com.tr/servlet/gt3dengine"

                            Bank = ArbakCCLib.ENTITY.ENUMS.Banks.GARANTI,
                            TerminalID = sanalPos.Xcip,
                            MerchantID = sanalPos.PosNumber,
                            TerminalUserID = sanalPos.Name,
                            BankName = "garanti",
                            ClientID = sanalPos.ClientId,
                            Name = "GARANTI",
                            Password = sanalPos.DDDStoreKey,
                            ProvisionPassword = sanalPos.Password,
                            PosType = ArbakCCLib.ENTITY.ENUMS.PosType.GARANTI,
                            PosURL = sanalPos.Host
                        };
                        posInfoList.Add(posInfoGaranti);
                        break;
                    case 2:
                        var posInfoYapiKredi = new YKBPosInfo()
                        {
                            Bank = ArbakCCLib.ENTITY.ENUMS.Banks.YAPIKREDI,
                            TerminalID = "",
                            MerchantID = "",
                            ClientID = "",
                            BankName = "yapikredi",
                            Name = "",
                            Key = "10,10,10,10,10,10,10,10",
                            PosType = ArbakCCLib.ENTITY.ENUMS.PosType.YAPIKREDI,
                            PosURL = "http://setmpos.ykb.com/3DSWebService/YKBPaymentService"
                        };
                        posInfoList.Add(posInfoYapiKredi);
                        break;
                    case 3:
                        // vakıfbank yok
                        break;
                    case 4:
                        var posInfoAkbank = new PosInfo()
                        {
                            Bank = ArbakCCLib.ENTITY.ENUMS.Banks.AKBANK,
                            BankName = "akbank",
                            ClientID = "",
                            Name = "AKBANK",
                            Password = "",
                            PosType = ArbakCCLib.ENTITY.ENUMS.PosType.EST,
                            PosURL = "https://entegrasyon.asseco-see.com.tr/fim/est3Dgate"
                        };
                        posInfoList.Add(posInfoAkbank);
                        break;
                    case 5:
                        var posInfoIsBankasi = new PosInfo()
                        {
                            Bank = ArbakCCLib.ENTITY.ENUMS.Banks.ISBANKASI,
                            BankName = "akbank",
                            ClientID = "",
                            Name = "AKBANK",
                            Password = "",
                            PosType = ArbakCCLib.ENTITY.ENUMS.PosType.EST,
                            PosURL = "https://entegrasyon.asseco-see.com.tr/fim/est3Dgate"
                        };
                        posInfoList.Add(posInfoIsBankasi);
                        break;
                    case 6:
                        var posInfoFinansbank = new PosInfo()
                        {
                            Bank = ArbakCCLib.ENTITY.ENUMS.Banks.FINANSBANK,
                            BankName = "akbank",
                            ClientID = "",
                            Name = "AKBANK",
                            Password = "",
                            PosType = ArbakCCLib.ENTITY.ENUMS.PosType.EST,
                            PosURL = "https://entegrasyon.asseco-see.com.tr/fim/est3Dgate"
                        };
                        posInfoList.Add(posInfoFinansbank);
                        break;
                    case 7:
                        // denizbank yok
                        break;
                    case 8:
                        var posInfoHalkbank = new PosInfo()
                        {
                            Bank = ArbakCCLib.ENTITY.ENUMS.Banks.HALKBANK,
                            BankName = "halkbank",
                            ClientID = "",
                            Name = "AKBANK",
                            Password = "",
                            PosType = ArbakCCLib.ENTITY.ENUMS.PosType.EST,
                            PosURL = "https://entegrasyon.asseco-see.com.tr/fim/est3Dgate"
                        };
                        posInfoList.Add(posInfoHalkbank);
                        break;
                }
            }

            ccManager = CCManager.CreateInstance(icerikAyar.DDDSuccessURL, icerikAyar.DDDErrorURL, icerikAyar.DDDConfirmURL, posInfoList);
        }
    }
}
