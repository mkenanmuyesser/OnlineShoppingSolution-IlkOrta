using System;
using System.Collections.Generic;
using System.Linq;
using CommerceProject.Business.BusinessContracts;
using System.Web;
using System.Web.Mvc;
using CommerceProject.Business.Entities;
using System.Data;
using CommerceProject.Admin.Helper;
using CommerceProject.Business.Custom;

namespace CommerceProject.Admin.Controllers
{
    public class AdresController : BaseController
    {
        IIcerikAyarService IcerikAyarService;
        IKullaniciService KullaniciService;
        IAdresService AdresService;
        public AdresController(IIcerikAyarService iIcerikAyarService,
                               IKullaniciService iKullaniciService,
                               IAdresService iAdresService) : base(iIcerikAyarService,
                                                                   iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            AdresService = iAdresService;
        }

        #region Actions
        public ActionResult AdresDetayPartial(int id)
        {
            ViewBag.Adres = AdresService.GetSingle(x => x.AdresId == id, true, new string[] { "FaturaTip", "AdresIl", "AdresIlce" });

            return PartialView("~/Views/Adres/Partials/AdresDetayPartial.cshtml");
        }
        #endregion
    }
}