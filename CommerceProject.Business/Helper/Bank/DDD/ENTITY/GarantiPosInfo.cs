using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArbakCCLib.ENTITY
{
    public class GarantiPosInfo : PosInfo
    {
        private string terminalID;
        public string TerminalID
        {
            get { return terminalID; }
            set { terminalID = value; }
        }

        private string terminalUserID;
        public string TerminalUserID
        {
            get { return terminalUserID; }
            set { terminalUserID = value; }
        }

        private string merchantID;
        public string MerchantID
        {
            get { return merchantID; }
            set { merchantID = value; }
        }

        private string provisionPassword;
        public string ProvisionPassword
        {
            get { return provisionPassword; }
            set { provisionPassword = value; }
        }
    }
}
