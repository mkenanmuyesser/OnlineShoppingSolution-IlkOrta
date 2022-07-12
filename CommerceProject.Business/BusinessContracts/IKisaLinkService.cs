﻿using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceProject.Business.BusinessContracts
{
    public interface IKisaLinkService : IGenericRepository<KisaLink>
    {
        string GenerateShortLink();

        string GetLongLink(string shortUrl);

        bool AddVisitor(string shortUrl);
    }
}
