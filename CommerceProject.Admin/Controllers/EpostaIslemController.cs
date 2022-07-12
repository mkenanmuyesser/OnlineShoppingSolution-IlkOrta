using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommerceProject.Admin.Helper;
using CommerceProject.Business.BusinessContracts;

namespace CommerceProject.Admin.Controllers
{
    public class EpostaIslemController : BaseController
    {
        IIcerikAyarService IcerikAyarService;
        IKullaniciService KullaniciService;
        public EpostaIslemController(IIcerikAyarService iIcerikAyarService,
                                     IKullaniciService iKullaniciService) : base(iIcerikAyarService,
                                                                                 iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.epostaislem_index;
            ViewBag.PageProperties = Helper.PageHelper.PageProperties.SetPageProperties("E-posta İşlemleri", "", "E-posta İşlemleri", "");

            return View();
        }
        #endregion
    }
}