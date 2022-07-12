using ArbakCCLib.ENTITY.ENUMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArbakCCLib.ENTITY
{
   public class PosInfo
    {
        private string clientID;
        public string ClientID
        {
            get { return clientID; }
            set { clientID = value; }
        }

        private string bankName;
        public string BankName
        {
            get { return bankName; }
            set { bankName = value; }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        private string posURL;
        public string PosURL
        {
            get { return posURL; }
            set { posURL = value; }
        }

        private Banks bank;
        public Banks Bank
        {
            get { return bank; }
            set { bank = value; }
        }

        private PosType posType;
        public PosType PosType
        {
            get { return posType; }
            set { posType = value; }
        }
   }
}
