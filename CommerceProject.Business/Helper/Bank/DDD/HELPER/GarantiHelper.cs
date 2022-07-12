using ArbakCCLib.ENTITY;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace ArbakCCLib.HELPER
{
    public class GarantiHelper : CCHelperBase
    {
        public override void PaymentRequest(HttpContext context, PosForm posForm, PosInfo pos, string okUrl, string failUrl)
        {
            GarantiPosInfo gPos = (GarantiPosInfo)pos;
            byte[] Dizi = new byte[7];
            RandomNumberGenerator Sayi;
            Sayi = RandomNumberGenerator.Create();
            Sayi.GetBytes(Dizi);

            Random rnd = new Random();

            string strMode = "PROD";
            string strApiVersion = "v0.01";
            string strTerminalProvUserID = "PROVAUT";
            string strType = "sales";

            string strAmount = posForm.Amount.ToString();
            if (strAmount.IndexOf(".") == -1 && strAmount.IndexOf(",") == -1)
                strAmount += "00";
            else
                strAmount = strAmount.Replace(".", "").Replace(",", ""); ;//İşlem Tutarı 1.00 TL için 100 gönderilmeli
            string strCurrencyCode = "949"; //para birimi kodu
            string strInstallmentCount = posForm.Installment == 0 ? "" : posForm.Installment.ToString(); //Taksit Sayısı. Boş gönderilirse taksit yapılmaz
            string strTerminalUserID = gPos.TerminalUserID;
            string strOrderID = rnd.Next(10000, 99999) + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 9);
            string strTerminalID = gPos.TerminalID; //8 Haneli TerminalID yazılmalı.
            string _strTerminalID = "0" + strTerminalID;
            string strTerminalMerchantID = gPos.MerchantID; //Üye İşyeri Numarası
            string strStoreKey = gPos.Password; //3D Secure şifresi
            string strProvisionPassword = gPos.ProvisionPassword; //TerminalProvUserID şifresi
            string strSuccessURL = okUrl;
            string strErrorURL = failUrl;
            string SecurityData = GetSHA1(strProvisionPassword + _strTerminalID).ToUpper();
            string HashData = GetSHA1(strTerminalID + strOrderID + strAmount + strSuccessURL + strErrorURL + strType + strInstallmentCount + strStoreKey + SecurityData).ToUpper();
           string Url = pos.PosURL;
            string formId = "myForm1";

            StringBuilder htmlForm = new StringBuilder();
            htmlForm.AppendLine("<html>");
            htmlForm.AppendLine(String.Format("<body onload='document.forms[\"{0}\"].submit()'>", formId));
            htmlForm.AppendLine(String.Format("<form id=\"{0}\" method=\"POST\" action=\"{1}\">", formId, Url));
            htmlForm.AppendLine("<input type=\"hidden\" name=\"mode\" value=\"" + strMode + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"secure3dsecuritylevel\" value=\"3D_PAY\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"apiversion\" value=\"" + strApiVersion + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"terminalprovuserid\" value=\"" + strTerminalProvUserID + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"orderid\" value=\""+strOrderID+"\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"terminaluserid\" value=\"" + strTerminalUserID + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"terminalmerchantid\" value=\"" + strTerminalMerchantID + "\"/>");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"txntype\" value=\"" + strType + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"txnamount\" value=\"" + strAmount + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"txncurrencycode\" value=\"" + strCurrencyCode + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"txninstallmentcount\" value=\"" + strInstallmentCount + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"terminalid\" value=\"" + strTerminalID + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"successurl\" value=\"" + strSuccessURL + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"errorurl\" value=\"" + strErrorURL + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"motoind\" value=\"N\"/>");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"refreshtime\" value=\"10\"/>");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"txntimestamp\" value=\""+DateTime.Now.ToString()+"\"/>");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"customeremailaddress\" value=\"" + posForm.Email + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"customeripaddress\" value=\"" + posForm.IPAdress + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"secure3dhash\" value=\"" + HashData + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"cardnumber\" value=\"" + posForm.CcNumber + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"cardexpiredatemonth\" value=\"" + posForm.ExpireMonth + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"cardexpiredateyear\" value=\"" + posForm.ExpireYear.Substring(2, 2) + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"cardcvv2\" value=\"" + posForm.Cvc + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"taksitAdet\" value=\"" + posForm.Installment + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"bankaAd\" value=\"garanti\" />");
            htmlForm.AppendLine("</form>");
            htmlForm.AppendLine("</body>");
            htmlForm.AppendLine("</html>");

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Write(htmlForm.ToString());
            HttpContext.Current.Response.End();
        }

        public override PaymentResult ConfirmPayment(HttpServerUtility server, HttpRequest request, HttpSessionState session, PosInfo gpos)
        {
            GarantiPosInfo pos = (GarantiPosInfo)gpos;
            string strMode = request.Form.Get("mode");
            string strApiVersion = request.Form.Get("apiversion");
            string strTerminalProvUserID = request.Form.Get("terminalprovuserid");
            string strType = request.Form.Get("txntype");
            string strAmount = request.Form.Get("txnamount");
            string strCurrencyCode = request.Form.Get("txncurrencycode");
            string strInstallmentCount = request.Form.Get("txninstallmentcount");
            string strTerminalUserID = request.Form.Get("terminaluserid");
            string strOrderID = request.Form.Get("oid");
            string strCustomeripaddress = request.Form.Get("customeripaddress");
            string strcustomeremailaddress = request.Form.Get("customeremailaddress");
            string strTerminalID = request.Form.Get("clientid");
            string _strTerminalID = "0" + strTerminalID;
            string strTerminalMerchantID = request.Form.Get("terminalmerchantid");
            string strStoreKey = pos.Password;
            //HASH doğrulaması için 3D Secure şifreniz
            string strProvisionPassword = pos.ProvisionPassword;
            //HASH doğrulaması için TerminalProvUserID şifresini tekrar yazıyoruz
            string strSuccessURL = request.Form.Get("successurl");
            string strErrorURL = request.Form.Get("errorurl");
          
            string transid = request.Form.Get("transid");
            string strCardholderPresentCode = "13";
            //3D Model işlemde bu değer 13 olmalı
            string strMotoInd = "N";
            string strNumber = "";
            //Kart bilgilerinin boş gitmesi gerekiyor
            string strExpireDate = "";
            //Kart bilgilerinin boş gitmesi gerekiyor
            string strCVV2 = "";
            //Kart bilgilerinin boş gitmesi gerekiyor
            string strAuthenticationCode = server.UrlEncode(request.Form.Get("cavv"));
            string strSecurityLevel = server.UrlEncode(request.Form.Get("eci"));
            string strTxnID = server.UrlEncode(request.Form.Get("xid"));
            string strMD = server.UrlEncode(request.Form.Get("md"));
            string strMDStatus = request.Form.Get("mdstatus");
            string strMDStatusText = request.Form.Get("mderrormessage");
            string strHostAddress = pos.PosURL;

            //Provizyon için xml'in post edileceği adres
            string SecurityData = GetSHA1(strProvisionPassword + _strTerminalID).ToUpper();
            string HashData = GetSHA1(strOrderID + strTerminalID + strAmount + SecurityData).ToUpper();
            //Daha kısıtlı bilgileri HASH ediyoruz.

            //strMDStatus.Equals(1)
            //"Tam Doğrulama"
            //strMDStatus.Equals(2)
            //"Kart Sahibi veya bankası sisteme kayıtlı değil"
            //strMDStatus.Equals(3)
            //"Kartın bankası sisteme kayıtlı değil"
            //strMDStatus.Equals(4)
            //"Doğrulama denemesi, kart sahibi sisteme daha sonra kayıt olmayı seçmiş"
            //strMDStatus.Equals(5)
            //"Doğrulama yapılamıyor"
            //strMDStatus.Equals(6)
            //"3-D Secure Hatası"
            //strMDStatus.Equals(7)
            //"Sistem Hatası"
            //strMDStatus.Equals(8)
            //"Bilinmeyen Kart No"
            //strMDStatus.Equals(0)
            //"Doğrulama Başarısız, 3-D Secure imzası geçersiz."

            //Hashdata kontrolü için bankadan dönen secure3dhash değeri alınıyor.
            string strHashData = request.Form.Get("secure3dhash");
            string ValidateHashData = GetSHA1(strTerminalID + strOrderID + strAmount + strSuccessURL + strErrorURL + strType + strInstallmentCount + strStoreKey + SecurityData).ToUpper();

            //İlk gönderilen ve bankadan dönen HASH değeri yeni üretilenle eşleşiyorsa;

            if (strHashData == ValidateHashData)
            {
                
                //Tam Doğrulama, Kart Sahibi veya bankası sisteme kayıtlı değil, Kartın bankası sisteme kayıtlı değil
                //Doğrulama denemesi, kart sahibi sisteme daha sonra kayıt olmayı seçmiş responselarını alan
                //işlemler için Provizyon almaya çalışıyoruz
                if (strMDStatus == "1" | strMDStatus == "2" | strMDStatus == "3" | strMDStatus == "4")
                {
                  
                  return new PaymentResult() { Description = "Ödeme başarılı", TransID=transid, Result = true };
                   
                }
                else
                {
                    session["order"] = null;
                    return new PaymentResult() { Description = "İşlem başarısız", Result = false };
                }
            }
            else
            { 
                session["order"] = null;
                return new PaymentResult() { Result = false, Description = "İşlem başarısız. " + strMDStatusText + " Güvenlik Uyarısı. Sayısal Imza Geçerli Degil İşlem Başarısız" };
               
            }
        }

        private string GetSHA1(string SHA1Data)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            string HashedPassword = SHA1Data;
            byte[] hashbytes = Encoding.GetEncoding("ISO-8859-9").GetBytes(HashedPassword);
            byte[] inputbytes = sha.ComputeHash(hashbytes);
            return GetHexaDecimal(inputbytes);
        }
        private string GetHexaDecimal(byte[] bytes)
        {
            StringBuilder s = new StringBuilder();
            int length = bytes.Length;
            for (int n = 0; n <= length - 1; n++)
            {
                s.Append(String.Format("{0,2:x}", bytes[n]).Replace(" ", "0"));
            }
            return s.ToString();
        }
    }
}
