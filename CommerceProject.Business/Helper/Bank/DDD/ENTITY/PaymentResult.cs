using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArbakCCLib.ENTITY
{
    public class PaymentResult
    {
        private bool result;
        public bool Result
        {
            get { return result; }
            set { result = value; }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private string transID;
        public string TransID
        {
            get { return transID; }
            set { transID = value; }
        }

        private string orderID;
        public string OrderID
        {
            get { return orderID; }
            set { orderID = value; }
        }
    }
}
