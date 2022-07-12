using _PosnetDotNetModule;
using CommerceProject.Business.Entities;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Security.Cryptography;

namespace CommerceProject.Business.Helper.Bank
{
    public class BankHelper
    {
        public static VirtualPosResultForm GarantiBankasi(VirtualPosForm pf, SanalPos sp)
        {
            VirtualPosResultForm resultForm = new VirtualPosResultForm();

            try
            {
                ePayment.cc5payment mycc5pay = new ePayment.cc5payment();
                mycc5pay.host = sp.Host;
                mycc5pay.name = sp.Name;
                mycc5pay.password = sp.Password;
                mycc5pay.clientid = sp.ClientId;
                mycc5pay.orderresult = 0;
                mycc5pay.oid = BankTool.RandomNumber();
                mycc5pay.currency = sp.Currency;
                mycc5pay.chargetype = sp.ChargeType;

                //gelenler
                mycc5pay.cardnumber = pf.kartNumarasi.ToString();
                mycc5pay.expmonth = string.Format("{0:00}", pf.ay);
                mycc5pay.expyear = pf.yil.ToString().Substring(2, 2);
                mycc5pay.cv2 = string.Format("{0:000}", pf.guvenlikKodu);
                mycc5pay.subtotal = pf.tutar.ToString();

                if (pf.taksit == -1)
                {
                    mycc5pay.taksit = "1";
                }
                else
                {
                    mycc5pay.taksit = pf.taksit.ToString();
                }

                //yedek bilgiler
                mycc5pay.bname = pf.kartSahibi;
                mycc5pay.phone = BankTool.GetIp();
                string x = mycc5pay.processorder();
                if (x == "1" & mycc5pay.appr == "Approved")
                {
                    //bankadan geri dönen
                    resultForm.sonuc = true;
                    resultForm.groupId = mycc5pay.groupid;
                    resultForm.referansNo = mycc5pay.refno;
                    resultForm.transId = mycc5pay.transid;
                    resultForm.code = mycc5pay.code;
                }
                else
                {
                    resultForm.sonuc = false;
                    resultForm.hataKodu = mycc5pay.err;
                    resultForm.hataMesaji = mycc5pay.errmsg;
                }
            }
            catch (Exception)
            {
                resultForm.sonuc = false;
                resultForm.hataMesaji = resultForm.sistemHatasi;
            }

            return resultForm;
        }

        public static VirtualPosResultForm YapiKrediBankasi(VirtualPosForm pf)
        {
            VirtualPosResultForm resultForm = new VirtualPosResultForm();

            // Banka bilgileri.
            string mid = "xxx";
            string tid = "xxx";

            try
            {
                Random rnd = new Random();
                string ccno = pf.kartNumarasi.ToString(), expdate = pf.yil.ToString().Replace("20", string.Empty) + pf.ay, cvc = string.Format("{0:000}", pf.guvenlikKodu), orderid = "1234567890123456789" + rnd.Next(11111, 99999), amount = pf.tutar.ToString(), currencycode = "YT", instnumber = pf.taksit.ToString();

                C_Posnet posnetObj = new C_Posnet();
                bool result = false;
                posnetObj.SetURL("https://www.posnet.ykb.com/PosnetWebService/XML");
                posnetObj.SetMid(mid);
                posnetObj.SetTid(tid);
                result = posnetObj.DoSaleTran(ccno, expdate, cvc, orderid, amount, currencycode, instnumber, "", "");

                if (pf.taksit > 0) { posnetObj.SetKOICode(pf.taksit.ToString()); }

                if (posnetObj.GetApprovedCode() == "1")
                {
                    resultForm.sonuc = true;
                    resultForm.code = posnetObj.GetAuthcode();
                    resultForm.referansNo = posnetObj.GetHostlogkey();
                }
                else
                {
                    resultForm.sonuc = false;
                    resultForm.hataMesaji = posnetObj.GetResponseText();
                }
            }
            catch (Exception)
            {
                resultForm.sonuc = false;
                resultForm.hataMesaji = resultForm.sistemHatasi;
            }

            return resultForm;
        }

        public static VirtualPosResultForm VakifBank(VirtualPosForm pf)
        {
            VirtualPosResultForm resultForm = new VirtualPosResultForm();

            // Banka bilgileri.
            string kullanici = "0001";
            string sifre = "xxx";
            string uyeno = "xxx";
            string posno = "xxx";
            string xcip = "xxx";

            try
            {
                byte[] b = new byte[5000];
                string sonuc;
                Encoding encoding = Encoding.GetEncoding("ISO-8859-9");

                String tutarcevir = pf.tutar.ToString();
                tutarcevir = tutarcevir.Replace(".", "");
                tutarcevir = tutarcevir.Replace(",", "");
                tutarcevir = String.Format("{0:0000000000.00}", Convert.ToInt32(tutarcevir)).Replace(",", "");

                string taksitcevir = "";

                if (pf.taksit == -1)
                {
                    taksitcevir = "00";
                }
                else
                {
                    taksitcevir = String.Format("{0:00}", pf.taksit);
                }

                String yilcevir = pf.yil.ToString();
                yilcevir = yilcevir.Substring(2, 2);

                string aycevir = string.Format("{0:00}", pf.ay);

                string provizyonMesaji = "kullanici=" + kullanici + "&sifre=" + sifre + "&islem=PRO&uyeno=" + uyeno + "&posno=" + posno + "&kkno=" + pf.kartNumarasi + "&gectar=" + yilcevir + aycevir + "&cvc=" + string.Format("{0:000}", pf.guvenlikKodu) + "&tutar=" + tutarcevir + "&provno=000000&taksits=" + taksitcevir + "&islemyeri=I&uyeref=" + BankTool.RandomNumber() + "&vbref=0&khip=" + BankTool.GetIp() + "&xcip=" + xcip;

                b.Initialize();
                b = Encoding.ASCII.GetBytes(provizyonMesaji);

                WebRequest h1 = (WebRequest)HttpWebRequest.Create("https://subesiz.vakifbank.com.tr/vpos724v3/?" + provizyonMesaji);
                h1.Method = "GET";

                WebResponse wr = h1.GetResponse();
                Stream s2 = wr.GetResponseStream();

                byte[] buffer = new byte[10000];
                int len = 0, r = 1;
                while (r > 0)
                {
                    r = s2.Read(buffer, len, 10000 - len);
                    len += r;
                }
                s2.Close();
                sonuc = encoding.GetString(buffer, 0, len).Replace("\r", "").Replace("\n", "");

                String gelenonaykodu, gelenrefkodu;
                XmlNode node = null;
                XmlDocument _msgTemplate = new XmlDocument();
                _msgTemplate.LoadXml(sonuc);
                node = _msgTemplate.SelectSingleNode("//Cevap/Msg/Kod");
                gelenonaykodu = node.InnerText.ToString();

                if (gelenonaykodu == "00")
                {
                    node = _msgTemplate.SelectSingleNode("//Cevap/Msg/ProvNo");
                    gelenrefkodu = node.InnerText.ToString();
                    resultForm.sonuc = true;
                    resultForm.referansNo = gelenrefkodu;
                }
                else
                {
                    resultForm.sonuc = false;
                    resultForm.hataMesaji = "";
                    resultForm.hataKodu = gelenonaykodu;
                }
            }
            catch (Exception e)
            {
                resultForm.sonuc = false;
                resultForm.hataMesaji = resultForm.sistemHatasi;
            }

            return resultForm;
        }

        public static VirtualPosResultForm AkBank(VirtualPosForm pf)
        {
            VirtualPosResultForm resultForm = new VirtualPosResultForm();

            try
            {
                ePayment.cc5payment mycc5pay = new ePayment.cc5payment();
                mycc5pay.host = "https://www.sanalakpos.com/servlet/cc5ApiServer";
                mycc5pay.name = "xxx";
                mycc5pay.password = "xxx";
                mycc5pay.clientid = "xxx";
                mycc5pay.orderresult = 0;
                mycc5pay.oid = BankTool.RandomNumber();
                mycc5pay.currency = "949";
                mycc5pay.chargetype = "Auth";

                //gelenler
                mycc5pay.cardnumber = pf.kartNumarasi.ToString();
                mycc5pay.expmonth = pf.ay.ToString();
                mycc5pay.expyear = pf.yil.ToString().Substring(2, 2);
                mycc5pay.cv2 = pf.guvenlikKodu.ToString();
                mycc5pay.subtotal = string.Format("{0:0.00}", pf.tutar);
                if (pf.taksit == -1)
                    mycc5pay.taksit = "1";
                else
                    mycc5pay.taksit = pf.taksit.ToString();

                //fatura bilgileri
                mycc5pay.bname = pf.kartSahibi;
                mycc5pay.bcity = BankTool.GetIp();

                string x = mycc5pay.processorder();

                if (x == "1" & mycc5pay.appr == "Approved")
                {
                    resultForm.sonuc = true;
                    resultForm.groupId = mycc5pay.groupid;
                    resultForm.code = mycc5pay.code;
                    resultForm.transId = mycc5pay.transid;
                    resultForm.referansNo = mycc5pay.refno;
                }
                else
                {
                    resultForm.hataMesaji = "";
                    resultForm.hataKodu = mycc5pay.errmsg;
                    resultForm.sonuc = false;
                }
            }
            catch (Exception)
            {
                resultForm.hataMesaji = resultForm.sistemHatasi;
                resultForm.sonuc = false;
            }

            return resultForm;
        }

        public static VirtualPosResultForm IsBankasi(VirtualPosForm pf)
        {
            VirtualPosResultForm resultForm = new VirtualPosResultForm();

            try
            {
                ePayment.cc5payment mycc5pay = new ePayment.cc5payment();
                mycc5pay.host = "https://spos.isbank.com.tr/servlet/cc5ApiServer";
                mycc5pay.name = "xxx";
                mycc5pay.password = "xxx";
                mycc5pay.clientid = "xxx";
                mycc5pay.orderresult = 0;
                mycc5pay.oid = BankTool.RandomNumber();
                mycc5pay.currency = "949";
                mycc5pay.chargetype = "Auth";

                //gelenler
                mycc5pay.cardnumber = pf.kartNumarasi.ToString();
                mycc5pay.expmonth = pf.ay.ToString();
                mycc5pay.expyear = pf.yil.ToString().Replace("20", string.Empty);
                mycc5pay.cv2 = pf.guvenlikKodu.ToString();
                mycc5pay.subtotal = pf.tutar.ToString();
                mycc5pay.taksit = pf.taksit.ToString();

                //fatura bilgileri
                mycc5pay.bname = pf.kartSahibi;
                mycc5pay.bcity = BankTool.GetIp();

                string x = mycc5pay.processorder();

                if (x == "1" & mycc5pay.appr == "Approved")
                {
                    //bankadan geri dönen
                    resultForm.sonuc = false;
                    resultForm.groupId = mycc5pay.groupid;
                    resultForm.transId = mycc5pay.transid;
                    resultForm.code = mycc5pay.code;
                    resultForm.referansNo = mycc5pay.refno;

                }
                else
                {
                    resultForm.sonuc = false;
                    resultForm.hataMesaji = mycc5pay.errmsg;
                    resultForm.hataKodu = mycc5pay.errmsg;

                }
            }
            catch (Exception)
            {
                resultForm.sonuc = false;
                resultForm.hataMesaji = resultForm.sistemHatasi;
            }

            return resultForm;
        }

        public static VirtualPosResultForm FinansBank(VirtualPosForm pf)
        {
            VirtualPosResultForm resultForm = new VirtualPosResultForm();

            try
            {
                ePayment.cc5payment mycc5pay = new ePayment.cc5payment();
                mycc5pay.host = "https://finanstest.fbwebpos.com/servlet/cc5ApiServer";
                mycc5pay.name = "xxx";
                mycc5pay.password = "xxx";
                mycc5pay.clientid = "xxx";
                mycc5pay.orderresult = 0;
                mycc5pay.oid = BankTool.RandomNumber();
                mycc5pay.currency = "949";
                mycc5pay.chargetype = "Auth";

                //gelenler
                mycc5pay.cardnumber = pf.kartNumarasi.ToString();
                mycc5pay.expmonth = string.Format("{0:00}", pf.ay);
                mycc5pay.expyear = pf.yil.ToString().Substring(2, 2);
                mycc5pay.cv2 = string.Format("{0:000}", pf.guvenlikKodu);
                mycc5pay.subtotal = pf.tutar.ToString();
                if (pf.taksit == -1)
                    mycc5pay.taksit = "1";
                else
                    mycc5pay.taksit = pf.taksit.ToString();

                //yedek bilgiler
                mycc5pay.bname = pf.kartSahibi;
                mycc5pay.phone = BankTool.GetIp();
                string x = mycc5pay.processorder();
                if (x == "1" & mycc5pay.appr == "Approved")
                {
                    //bankadan geri dönen
                    resultForm.sonuc = true;
                    resultForm.groupId = mycc5pay.groupid;
                    resultForm.referansNo = mycc5pay.refno;
                    resultForm.transId = mycc5pay.transid;
                    resultForm.code = mycc5pay.code;
                }
                else
                {
                    resultForm.sonuc = false;
                    resultForm.hataKodu = mycc5pay.err;
                    resultForm.hataMesaji = mycc5pay.errmsg;
                }
            }
            catch (Exception)
            {
                resultForm.sonuc = false;
                resultForm.hataMesaji = resultForm.sistemHatasi;
            }

            return resultForm;
        }

        public static VirtualPosResultForm DenizBank(VirtualPosForm pf)
        {
            VirtualPosResultForm resultForm = new VirtualPosResultForm();

            try
            {
                ePayment.cc5payment mycc5pay = new ePayment.cc5payment();
                mycc5pay.host = "https://sanalpos.denizbank.com.tr/servlet/cc5ApiServer";
                mycc5pay.name = "xxxx";
                mycc5pay.password = "xxxx";
                mycc5pay.clientid = "xxxx";
                mycc5pay.orderresult = 0;
                mycc5pay.oid = BankTool.RandomNumber();
                mycc5pay.currency = "949";
                mycc5pay.chargetype = "Auth";

                //gelenler
                mycc5pay.cardnumber = pf.kartNumarasi.ToString();
                mycc5pay.expmonth = pf.ay.ToString();
                mycc5pay.expyear = pf.yil.ToString().Substring(2, 2);
                mycc5pay.cv2 = pf.guvenlikKodu.ToString();
                mycc5pay.subtotal = string.Format("{0:0.00}", pf.tutar);
                mycc5pay.taksit = pf.taksit.ToString();

                //fatura bilgileri
                mycc5pay.bname = pf.kartSahibi;
                mycc5pay.bcity = BankTool.GetIp();

                string x = mycc5pay.processorder();

                if (x == "1" & mycc5pay.appr == "Approved")
                {
                    resultForm.sonuc = true;
                    resultForm.groupId = mycc5pay.groupid;
                    resultForm.code = mycc5pay.code;
                    resultForm.transId = mycc5pay.transid;
                    resultForm.referansNo = mycc5pay.refno;
                }
                else
                {
                    resultForm.hataMesaji = "";
                    resultForm.hataKodu = mycc5pay.errmsg;
                    resultForm.sonuc = false;
                }
            }
            catch (Exception)
            {
                resultForm.hataMesaji = resultForm.sistemHatasi;
                resultForm.sonuc = false;
            }

            return resultForm;
        }
      
    }
}
