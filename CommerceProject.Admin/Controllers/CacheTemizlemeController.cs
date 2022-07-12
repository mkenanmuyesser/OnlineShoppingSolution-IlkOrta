using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommerceProject.Admin.Helper;
using CommerceProject.Business.BusinessContracts;
using CommerceProject.Business.Helper.Caching;
using CommerceProject.Business.Custom;

namespace CommerceProject.Admin.Controllers
{
    public class CacheTemizlemeController : BaseController
    {
        IIcerikAyarService IcerikAyarService;
        IKullaniciService KullaniciService;
        ICacheService CacheService;
        public CacheTemizlemeController(IIcerikAyarService iIcerikAyarService,
                                        IKullaniciService iKullaniciService,
                                        ICacheService iCacheService) : base(iIcerikAyarService,
                                                                            iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            CacheService = iCacheService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.cachetemizleme_index;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Cache Temizleme İşlemleri", "", "Cache Temizleme İşlemleri", "");

            ViewBag.CacheList = CacheService.FindBy(x => x.AktifMi).OrderBy(x => x.Sira).ToList();

            return View();
        }
        #endregion

        #region Ajax Methods
        [HttpPost]
        public JsonResult Temizle(int[] cacheList)
        {
            var flag = false;
            try
            {
                foreach (var cache in cacheList)
                {
                    CacheDataObj cacheDataObj = (CacheDataObj)cache;
                    CacheHelper.ClearCache(cacheDataObj);
                }

                flag = true;
            }
            catch (Exception ex)
            {

            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}