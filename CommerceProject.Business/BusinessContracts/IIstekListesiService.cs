using CommerceProject.Business.Custom;
using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;
using System;
using System.Collections.Generic;

namespace CommerceProject.Business.BusinessContracts
{
    public interface IIstekListesiService : IGenericRepository<IstekListesi>
    {
        List<IstekListesi> GetUserWishList(Guid userId);
    }
}
