using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceProject.Business.Helper.Bank
{
    public class VirtualPosResultForm
    {
        public string sistemHatasi = "Bankayla bağlantı kurulamadı ! Lütfen daha sonra tekrar deneyin.";

        public bool sonuc { get; set; }
        public string hataMesaji { get; set; }
        public string hataKodu { get; set; }

        // Bankadan geri dönen değerler.
        public string code { get; set; }
        public string groupId { get; set; }
        public string transId { get; set; }
        public string referansNo { get; set; }
    }
}
