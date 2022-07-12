using ArbakCCLib.ENTITY;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace ArbakCCLib.HELPER
{
    public class ESTHelper : CCHelperBase
    {
        public override void PaymentRequest(HttpContext context, PosForm posForm, PosInfo pos, string okUrl, string failUrl)
        {
            string oid = posForm.OrderID;      //Sipariş numarası         
            string installment = "";
            string ins = posForm.Installment.ToString();
            if (posForm.Installment != 0)
                installment = posForm.Installment.ToString();
           string rnd = DateTime.Now.ToString();        //Güvenlik ve kontrol amaçlı tarih yada sürekli değişen bir değer

            string storekey = pos.Password;                    //işyeri anahtarı3dgate_path
            string storetype = "3d_pay";
            string hashstr = pos.ClientID + oid + posForm.Amount.ToString() + okUrl + failUrl +"Auth"+installment+ rnd + storekey;
            System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] hashbytes = System.Text.Encoding.GetEncoding("ISO-8859-9").GetBytes(hashstr);
            byte[] inputbytes = sha.ComputeHash(hashbytes);

            string hash = Convert.ToBase64String(inputbytes);   //Günvelik amaçlı oluşturulan hash

            string formId = "myForm1";

            StringBuilder htmlForm = new StringBuilder();
            htmlForm.AppendLine("<html>");
            htmlForm.AppendLine(String.Format("<body onload='document.forms[\"{0}\"].submit()'>", formId));
            htmlForm.AppendLine(String.Format("<form id=\"{0}\" method=\"POST\" action=\"{1}\">", formId, pos.PosURL));
            htmlForm.AppendLine("<input type=\"hidden\" name=\"clientid\" value=\"" + pos.ClientID + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"amount\" value=\"" + posForm.Amount.ToString() + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"oid\" value=\"" + oid + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"okUrl\" value=\"" + okUrl + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"failUrl\" value=\"" + failUrl + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"rnd\" value=\"" + rnd + "\"/>");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"hash\" value=\"" + hash + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"storetype\" value=\"" + storetype + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"lang\" value=\"tr\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"islemtipi\" value=\"Auth\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"total1\" value=\"" + posForm.Amount.ToString() + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"price1\" value=\""+posForm.Amount.ToString()+"\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"currency\" value=\"949\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"pan\" value=\"" + posForm.CcNumber + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"Ecom_Payment_Card_ExpDate_Year\" value=\"" + posForm.ExpireYear + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"Ecom_Payment_Card_ExpDate_Month\" value=\"" + (posForm.ExpireMonth.Length == 1 ? "0" + posForm.ExpireMonth : posForm.ExpireMonth) + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"cv2\" value=\"" + posForm.Cvc + "\" />");
            htmlForm.AppendLine("<input type=\"hidden\" name=\"taksit\" value=\"" + installment + "\" />");
            htmlForm.AppendLine("</form>");
            htmlForm.AppendLine("</body>");
            htmlForm.AppendLine("</html>");

            context.Response.Clear();
            context.Response.Write(htmlForm.ToString());
            context.Response.End();

        }

        public override PaymentResult ConfirmPayment(HttpServerUtility server, HttpRequest request, HttpSessionState session, PosInfo pos)
        {
            string orderID="";
            String[] parameters = new String[] { "AuthCode", "Response", "HostRefNum", "ProcReturnCode", "TransId", "ErrMsg" };
            IEnumerator e = request.Form.GetEnumerator();
            while (e.MoveNext())
            {
                String xkey = (String)e.Current;
                String xval = request.Form.Get(xkey);
                bool ok = true;
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (xkey.Equals(parameters[i]))
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok && xkey == "oid")
                    orderID = xval;
                    
            }

            String hashparams = request.Form.Get("HASHPARAMS");
            String hashparamsval = request.Form.Get("HASHPARAMSVAL");
            String storekey = pos.Password;
            String paramsval = "";
            int index1 = 0, index2 = 0;
            // hash hesaplamada kullanılacak değerler ayrıştırılıp değerleri birleştiriliyor.
            do
            {
                index2 = hashparams.IndexOf(":", index1);
                String val = request.Form.Get(hashparams.Substring(index1, index2 - index1)) == null ? "" : request.Form.Get(hashparams.Substring(index1, index2 - index1));
                paramsval += val;
                index1 = index2 + 1;
            }
            while (index1 < hashparams.Length);

            String hashval = paramsval + storekey;
            String hashparam = request.Form.Get("HASH");

            System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] hashbytes = System.Text.Encoding.GetEncoding("ISO-8859-9").GetBytes(hashval);
            byte[] inputbytes = sha.ComputeHash(hashbytes);

            String hash = Convert.ToBase64String(inputbytes); //Güvenlik ve kontrol amaçlı oluşturulan hash

            if (!paramsval.Equals(hashparamsval) || !hash.Equals(hashparam)) //oluşturulan hash ile gelen hash ve hash parametreleri değerleri ile ayrıştırılıp edilen edilen aynı olmalı.
            {
                return new PaymentResult() { Result = false, Description = "İmza geçerli değil." };
            }

            String mdStatus = request.Form.Get("mdStatus"); // 3d işlemin sonucu
            if (mdStatus.Equals("1") || mdStatus.Equals("2") || mdStatus.Equals("3") || mdStatus.Equals("4"))
            {
                string transID = "";
                for (int i = 0; i < parameters.Length; i++)
                {
                    String paramname = parameters[i];
                    String paramval = request.Form.Get(paramname);
                    if (paramname == "TransId")
                    {
                        transID = paramval;
                        break;
                    }
                }

                session["OrderID"] = null;
                return new PaymentResult() { Description = "Ödeme işlemi başarılı.", Result = true, TransID = transID, OrderID = orderID };

            }

            return new PaymentResult() { Description = "Ödeme işlemi başarısız.", Result = false };
        }
    }
}
