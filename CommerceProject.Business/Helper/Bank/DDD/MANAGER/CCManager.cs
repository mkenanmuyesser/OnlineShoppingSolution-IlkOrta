using ArbakCCLib.ENTITY;
using ArbakCCLib.ENTITY.ENUMS;
using ArbakCCLib.HELPER;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace ArbakCCLib.MANAGER
{
    public class CCManager
    {
        private static CCManager instance;

        public static CCManager Instance
        {
            get { return CCManager.instance; }
        }

        Dictionary<Banks, PosInfo> posDic;
        Dictionary<PosType, CCHelperBase> posTypeDic;

        public static CCManager CreateInstance(string okURL, string failURL, string receiveURL, List<PosInfo> posInfoList)
        {
            instance = new CCManager(okURL, failURL, receiveURL, posInfoList);
            return instance;
        }

        string okURL, failURL, receiveURL;
        private CCManager(string okURL, string failURL, string receiveURL, List<PosInfo> posInfoList)
        {
            posTypeDic = new Dictionary<PosType, CCHelperBase>();
            posTypeDic.Add(PosType.EST, new ESTHelper());
            posTypeDic.Add(PosType.GARANTI, new GarantiHelper());
            posTypeDic.Add(PosType.YAPIKREDI, new YKBHelper());

            posDic = new Dictionary<Banks, PosInfo>();
            this.okURL = okURL;
            this.failURL = failURL;
            this.receiveURL = receiveURL;
            for (int i = 0; i < posInfoList.Count; i++)
                AddToPosDic(posInfoList[i]);
        }

        public void SendPayment(HttpContext context, PosForm posForm, Banks bank)
        {
            PosInfo pos = posDic[bank];
            CCHelperBase helper = posTypeDic[pos.PosType];
            string url;

            if(helper is IReceiverBank)
                url = receiveURL + "?bank=" + (int)bank + "&&OrderID=" + posForm.OrderID;
            else
                url = okURL+"?bank="+(int)bank+"&&OrderID="+posForm.OrderID;

            helper.PaymentRequest(context, posForm, pos, url, failURL);
        }

        public void SendPayment(HttpContext context, PosForm posForm, string bank)
        {
            if (bank.IndexOf("0") == 0)
               bank = bank.Replace("0","");

            SendPayment(context, posForm, (Banks)Convert.ToInt32(bank));
        }

        public void ReceivePayment(HttpRequest request, HttpResponse response)
        {
            if (request.QueryString["bank"] != null && request.QueryString["OrderID"] != null)
            {
                string query = request.QueryString["bank"].ToString();
                string orderID = request.QueryString["OrderID"].ToString();
                PosInfo pos = posDic[(Banks)Convert.ToInt32(query)];
                IReceiverBank helper = (IReceiverBank)posTypeDic[pos.PosType];
                helper.ReceivePayment(request, response, pos, okURL + "?bank=" + query + "&&OrderID=" + orderID);
            }
        }

        public PaymentResult ConfirmPayment(HttpServerUtility server, HttpRequest request, HttpSessionState session)
        {
            if (request.QueryString["bank"] != null)
            {
                string query = request.QueryString["bank"].ToString();
                PosInfo pos = posDic[(Banks)Convert.ToInt32(query)];
                CCHelperBase helper = posTypeDic[pos.PosType];
                return helper.ConfirmPayment(server, request, session, pos);
            }

            return new PaymentResult() { Result = false, Description = "Ödeme işlemi başarısız." };
        }

        private void AddToPosDic(PosInfo pos)
        {
            posDic.Add(pos.Bank, pos);
        }
    }
}
