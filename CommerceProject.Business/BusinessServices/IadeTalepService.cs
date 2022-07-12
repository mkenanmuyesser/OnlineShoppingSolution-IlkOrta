using CommerceProject.Business.BusinessContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;

namespace CommerceProject.Business.BusinessServices
{
    public class IadeTalepService : GenericRepository<IadeTalep>, IIadeTalepService
    {
        public List<IadeTalep> GetUserReturnList(Guid userId)
        {
            var iadeListesi = this.FindBy(x => x.AktifMi == true &&
                                                x.SiparisDetay.Siparis.KullaniciId == userId,
                                                false,
                                                new string[] { "SiparisDetay", "SiparisDetay", "SiparisDetay.Urun", "IadeTalepDurumTip" }).ToList();

            return iadeListesi;
        }
    }
}
