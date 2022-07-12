using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceProject.Business.BusinessContracts
{
    public interface IUrunYorumService : IGenericRepository<UrunYorum>
    {
        List<UrunYorum> GetUserComments(Guid kullaniciId);

        bool AddUserComment(Guid kullaniciId, int urunId, int puan, string yorum);
    }
}
