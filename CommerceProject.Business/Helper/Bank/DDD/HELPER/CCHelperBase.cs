using ArbakCCLib.ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace ArbakCCLib.HELPER
{
    public abstract class CCHelperBase
    {
        public abstract void PaymentRequest(HttpContext context, PosForm posForm, PosInfo pos, string okUrl, string failUrl);

        public abstract PaymentResult ConfirmPayment(HttpServerUtility server, HttpRequest request, HttpSessionState session, PosInfo pos);
    }
}
