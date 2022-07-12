using CommerceProject.Business.BusinessContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;

namespace CommerceProject.Business.BusinessServices
{
    public class SiparisService : GenericRepository<Siparis>, ISiparisService
    {
        public List<Siparis> GetUserOrders(Guid userId)
        {
            var siparisler = this.FindBy(x => x.AktifMi == true &&
                                              x.KullaniciId == userId,
                                              false,
                                              new string[] { "SiparisDetay", "SiparisDurumTip", "Adres" }).ToList();
            return siparisler;
        }
    }
}
