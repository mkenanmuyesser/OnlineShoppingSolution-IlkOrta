using CommerceProject.Business.BusinessContracts;
using CommerceProject.Business.Entities;
using System.ComponentModel;

namespace CommerceProject.Presentation.IlkOrta.Helper
{
    public class PageHelper
    {
        #region Classes
        public class PageProperties
        {
            public string BreadcrumbLevel1 { get; set; }
            public string BreadcrumbLevel2 { get; set; }
            public string PageHeader { get; set; }
            public string PageDescription { get; set; }
            public string PageLink { get; set; }
            public string Title { get; set; }
            public string Keywords { get; set; }
            public string Description { get; set; }
            public string Author { get; set; }

            public static PageProperties SetPageProperties(string breadcrumbLevel1,
                                                           string breadcrumbLevel2,
                                                           string pageHeader,
                                                           string pageDescription,
                                                           string pageLink)
            {
                PageProperties pageProperties = new PageProperties
                {
                    BreadcrumbLevel1 = breadcrumbLevel1,
                    BreadcrumbLevel2 = breadcrumbLevel2,
                    PageHeader = pageHeader,
                    PageDescription = pageDescription,
                    PageLink = pageLink
                };

                return pageProperties;
            }
        }
        #endregion

        #region Enums
        public enum BannerTipEnum
        {
            [Description("Anasayfa Üst Slider Banner")]
            AnasayfaUstSliderBanner = 1,

            [Description("Ürün Sayfası Üst Slider Banner")]
            UrunSayfasiUstSliderBanner = 2,

            [Description("Ürün Sayfası Üst Banner")]
            UrunSayfasiUstBanner = 3,
        }
        #endregion
    }
}