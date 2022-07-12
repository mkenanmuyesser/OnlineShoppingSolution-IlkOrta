using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArbakCCLib.ENTITY
{
    public class PosForm
    {

        string ccOwnerName;
        public string CcOwnerName
        {
            get { return ccOwnerName; }
            set { ccOwnerName = value; }
        }

        string ccNumber;
        public string CcNumber
        {
            get { return ccNumber; }
            set { ccNumber = value; }
        }

        string cvc;
        public string Cvc
        {
            get { return cvc; }
            set { cvc = value; }
        }

        private string expireYear;
        public string ExpireYear
        {
            get { return expireYear; }
            set { expireYear = value; }
        }

        private string expireMonth;
        public string ExpireMonth
        {
            get { return expireMonth; }
            set { expireMonth = value; }
        }

        decimal amount;
        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        int installment;
        public int Installment
        {
            get { return installment; }
            set { installment = value; }
        }

        string orderID;
        public string OrderID
        {
            get { return orderID; }
            set { orderID = value; }
        }

        private string ipAdress;
        public string IPAdress
        {
            get { return ipAdress; }
            set { ipAdress = value; }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        private string telephone;
        public string Telephone
        {
            get { return telephone; }
            set { telephone = value; }
        }
    }
}
