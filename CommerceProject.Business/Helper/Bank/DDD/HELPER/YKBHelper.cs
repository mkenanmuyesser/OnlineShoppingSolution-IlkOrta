using _PosnetDotNetTDSOOSModule;
using ArbakCCLib.ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ArbakCCLib.HELPER
{
    public class YKBHelper : CCHelperBase, IReceiverBank
    {

        public override void PaymentRequest(HttpContext context, PosForm posForm, PosInfo pos, string okUrl, string failUrl)
        {
            YKBPosInfo yPos = (YKBPosInfo)pos;
            C_PosnetOOSTDS posnetObj = new C_PosnetOOSTDS();
            posnetObj.SetURL(yPos.PosURL);
            posnetObj.SetTid(yPos.TerminalID);
            posnetObj.SetMid(yPos.MerchantID);
            posnetObj.SetPosnetID(yPos.ClientID);
            posnetObj.SetKey(yPos.Key);

            string amount = posForm.Amount.ToString();
            if (amount.IndexOf(",") != -1)
                amount = amount.Replace(",", "");
            else
                amount += "00";

            string str = "";
            for (int i = 0; i < 20; i++)
                str += "7";

            bool result = posnetObj.CreateTranRequestDatas(posForm.CcOwnerName, amount, "TL", posForm.Installment.ToString(), str, "Sale", posForm.CcNumber, posForm.ExpireYear.Substring(2, 2) + posForm.ExpireMonth, posForm.Cvc);
            string strs = posnetObj.GetResponseText();
            if (result)
            {
                StringBuilder htmlForm = new StringBuilder();
                htmlForm.AppendLine("<html>");
                htmlForm.AppendLine("<head>");
                htmlForm.AppendLine("</head>");
                htmlForm.AppendLine(String.Format("<body onload='document.forms[\"{0}\"].submit()'>", "form"));
                htmlForm.AppendLine(String.Format("<form id=\"form\" method=\"POST\" action=\"{0}\">", pos.PosURL));
                htmlForm.AppendLine("<input type=\"hidden\" name=\"mid\" value=\"" + yPos.MerchantID + "\" />");
                htmlForm.AppendLine("<input type=\"hidden\" name=\"posnetID\" value=\"" + yPos.ClientID + "\" />");
                htmlForm.AppendLine("<input type=\"hidden\" name=\"posnetData\" value=\"" + posnetObj.GetPosnetData() + "\" />");
                htmlForm.AppendLine("<input type=\"hidden\" name=\"posnetData2\" value=\"" + posnetObj.GetPosnetData2() + "\" />");
                htmlForm.AppendLine("<input type=\"hidden\" name=\"digest\" value=\"" + posnetObj.GetSign() + "\" />");
                htmlForm.AppendLine("<input type=\"hidden\" name=\"vftCode\" value=\"" + "\"/>");
                htmlForm.AppendLine("<input type=\"hidden\" name=\"merchantReturnURL\" value=\"" + okUrl + "\" />");
                htmlForm.AppendLine("<input type=\"hidden\" name=\"openANewWindow\" value=\"1\" />");
                htmlForm.AppendLine("</form>");
                htmlForm.AppendLine("</body>");
                htmlForm.AppendLine("</html>");

                context.Response.Clear();
                context.Response.Write(htmlForm.ToString());
                context.Response.End();
            }
            else
            {

                context.Response.Write(strs);
                //                context.Response.Redirect("PaymentFail.aspx", true);
            }
        }

        public void ReceivePayment(HttpRequest Request, HttpResponse Response, PosInfo pos, string okURL)
        {
            
            YKBPosInfo posInfo = (YKBPosInfo)pos;
            C_PosnetOOSTDS posnetOOSTDSObj = new C_PosnetOOSTDS();

            string merchantPacket = null;
            string bankPacket = null;
            string sign = null;
            string tranType = null;

            //Banka tafafından yönlendirilen işlem bilgilerini alınır
            merchantPacket = Request.Form.Get("MerchantPacket");
            bankPacket = Request.Form.Get("BankPacket");
            sign = Request.Form.Get("Sign");
            tranType = Request.Form.Get("TranType");


            posnetOOSTDSObj.SetURL("http://setmpos.ykb.com/PosnetWebService/XML");
            posnetOOSTDSObj.SetTid(posInfo.TerminalID);
            posnetOOSTDSObj.SetMid(posInfo.MerchantID);
            posnetOOSTDSObj.SetPosnetID(posInfo.ClientID);
            posnetOOSTDSObj.SetKey(posInfo.Key);

            posnetOOSTDSObj.SetProxyConfig(false,"", "","");

            //İşlem bilgilerinin çözümlenmesi

            //Aynı sayfaya iki kere çağrılır, "else" blogunda banka tarafından yönlendirilen, yukarıda alınan işlem bilgileri
            //doğrulanır. "If" bloğunda ise doğrulanmış ise finansallaştırma işlemi başlatılır.
                posnetOOSTDSObj.CheckAndResolveMerchantData(merchantPacket, bankPacket, sign);

                string pAmount = posnetOOSTDSObj.GetTotalPointAmount();
                string tempAmount = posnetOOSTDSObj.GetAmount();
      
                string orderID = posnetOOSTDSObj.GetXID();
                string amount = CurrencyFormat(posnetOOSTDSObj.GetAmount(), posnetOOSTDSObj.GetCurrencyCode(), true);
                string error = posnetOOSTDSObj.GetResponseText();

                string mn = pAmount;
                string tm = tempAmount;
                string om = orderID;
                string am = amount;
                string em = error;

                string WPAmount = tempAmount.ToString();
                if ((!string.IsNullOrEmpty(WPAmount)))
                {
                    posnetOOSTDSObj.SetPointAmount(WPAmount);
                }

                Response.Write("<html>");
                Response.Write("<head>");
                Response.Write("<META HTTP-EQUIV='Content-Type' content='text/html; charset=Windows-1254'>");

                Response.Write("<script language='JavaScript'>");

                Response.Write("function submitForm(form) {");
                Response.Write("\t form.submit();");
                Response.Write("}");

                Response.Write("</script>");

                Response.Write("<title>YKB - POSNET Redirector</title></head>");
                Response.Write("<body bgcolor='#02014E' OnLoad='submitForm(document.form2);' >");

                //3DS Kredi kartı onay İşlemini başlat
                posnetOOSTDSObj.ConnectAndDoTDSTransaction(merchantPacket, bankPacket, sign);

                Response.Write(" <form name='form2' method='post' action='" + okURL + "' >");
                Response.Write("   <input type='hidden' name='XID' value='" + posnetOOSTDSObj.GetXID() + "'>");
                Response.Write("   <input type='hidden' name='Amount' value='" + posnetOOSTDSObj.GetAmount() + "'>");
                Response.Write("   <input type='hidden' name='WPAmount' value='" + WPAmount + "'>");
                Response.Write("   <input type='hidden' name='Currency' value='" + posnetOOSTDSObj.GetCurrencyCode() + "'>");

                Response.Write("   <input type='hidden' name='ApprovedCode' value='" + posnetOOSTDSObj.GetApprovedCode() + "'>");
                Response.Write("   <input type='hidden' name='AuthCode' value='" + posnetOOSTDSObj.GetAuthcode() + "'>");
                Response.Write("   <input type='hidden' name='HostLogKey' value='" + posnetOOSTDSObj.GetHostlogkey() + "'>");
                Response.Write("   <input type='hidden' name='RespCode' value='" + posnetOOSTDSObj.GetResponseCode() + "'>");
                Response.Write("   <input type='hidden' name='RespText' value='" + posnetOOSTDSObj.GetResponseText() + "'>");

                Response.Write("   <input type='hidden' name='Point' value='" + posnetOOSTDSObj.GetPoint() + "'>");
                Response.Write("   <input type='hidden' name='PointAmount' value='" + posnetOOSTDSObj.GetPointAmount() + "'>");
                Response.Write("   <input type='hidden' name='TotalPoint' value='" + posnetOOSTDSObj.GetTotalPoint() + "'>");
                Response.Write("   <input type='hidden' name='TotalPointAmount' value='" + posnetOOSTDSObj.GetTotalPointAmount() + "'>");

                Response.Write("   <input type='hidden' name='InstalmentNumber' value='" + posnetOOSTDSObj.GetInstalmentNumber() + "'>");
                Response.Write("   <input type='hidden' name='InstalmentAmount' value='" + posnetOOSTDSObj.GetInstalmentAmount() + "'>");

                Response.Write("   <input type='hidden' name='VftAmount' value='" + posnetOOSTDSObj.GetVFTAmount() + "'>");
                Response.Write("   <input type='hidden' name='VftDayCount' value='" + posnetOOSTDSObj.GetVFTDayCount() + "'>");
                Response.Write(" </form>");
                Response.Write(" </body>");
                Response.Write(" </html>");
                Response.End();
            

        }

        public double ConvertYTLToYKR(string amount)
        {
            if (amount != null && amount.Length > 0)
            {
                return double.Parse(amount.Replace(".", "")) * 100;
            }
            else
            {
                return 0;
            }
        }


        public string CurrencyFormat(string amount, string currencyCode, bool addCurrency)
        {

            if (amount == null || amount == "")
            {
                return "";
            }

            if (amount == "-1")
            {
                return "";
            }
            else
            {
                if (amount.Length == 2)
                {
                    amount = "0" + amount;
                }
                else if (amount.Length < 2)
                {
                    amount = "00" + amount;
                }


                if (currencyCode == "YT" || currencyCode == "TL" || currencyCode == "US" || currencyCode == "EU")
                {
                    string currencyFormat = amount.Substring(0, amount.Length - 2) + "," + amount.Substring(amount.Length - 2, 2);
                    if (addCurrency)
                    {
                        return currencyFormat + " " + GetCurrencyText(currencyCode);
                    }
                    else
                    {
                        return currencyFormat;
                    }
                }
                else
                {
                    return amount;
                }
            }
        }

        public  string GetCurrencyText(string currencyCode)
        {

            if (currencyCode == "YT" || currencyCode == "TL")
            {
                return "TL";
            }
            else if (currencyCode == "US")
            {
                return "USD";
            }
            else if (currencyCode == "EU")
            {
                return "EUR";
            }
            else
            {
                return "";
            }
        }

        public override PaymentResult ConfirmPayment(HttpServerUtility server, HttpRequest request, HttpSessionState session, PosInfo pos)
        {
            PaymentResult result = new PaymentResult();
            for (int i = 0; i <= request.Form.Count - 1; i++)
            {
                result.Description += request.Form.Keys.Get(i) + ": " + request.Form.Get(i) + "\n";
               
            }

            return result;
        }

       
    }
}
