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
    public class HaberBulteniAbonelikService : GenericRepository<HaberBulteniAbonelik>, IHaberBulteniAbonelikService
    {
        public bool Subscribe(string email)
        {
            //abone kaydı daha önceden var mı? varsa yenisini oluşturma           
            if (this.FindBy(x => x.AktifMi && x.Eposta == email).Any())
            {
                return false;
            }
            else
            {
                HaberBulteniAbonelik haberBulteniAbonelik = new HaberBulteniAbonelik
                {
                    HaberBulteniAbonelikId = -1,
                    Eposta = email,
                    Tarih = DateTime.Now,
                    AktifMi = true
                };

                this.Add(haberBulteniAbonelik);
                return this.Save();
            }
        }
    }
}
