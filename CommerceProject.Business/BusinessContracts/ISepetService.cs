using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceProject.Business.BusinessContracts
{
    public interface ISepetService : IGenericRepository<Sepet>
    {
        List<Sepet> GetUserProductsFromBasket(Guid kullaniciId);

        bool SetUserProductsFromBasket(Guid kullaniciId, int urunId, int adet);

        bool SetUserPackagesFromBasket(Guid kullaniciId, int adet, Paket paket);

        bool RemoveUserProductsFromBasket(Guid kullaniciId, int urunId, int adet);

        bool ClearUserProductsFromBasket(Guid kullaniciId);

        //bu silinecek
        bool RemoveProductsFromBasket(Guid kullaniciId, int urunId);

        bool SetProcessUserProductsFromBasket(Guid kullaniciId);
    }
}
