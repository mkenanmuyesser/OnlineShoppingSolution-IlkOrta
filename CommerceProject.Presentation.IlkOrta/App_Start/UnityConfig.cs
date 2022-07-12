using CommerceProject.Business.BusinessContracts;
using CommerceProject.Business.BusinessServices;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace CommerceProject.Presentation.IlkOrta
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            // e.g. container.RegisterType<ITestService, TestService>();
            
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            container.RegisterType<IIcerikAyarService, IcerikAyarService>();
            container.RegisterType<ISepetService, SepetService>();
            container.RegisterType<IKullaniciService, KullaniciService>();
            container.RegisterType<IKullaniciDetayService, KullaniciDetayService>();
            container.RegisterType<IBankaService, BankaService>();
            container.RegisterType<IHesapNumarasiService, HesapNumarasiService>();
            container.RegisterType<ISSSService, SSSService>();
            container.RegisterType<IKategoriService, KategoriService>();
            container.RegisterType<IBannerService, BannerService>();
            container.RegisterType<IUrunService, UrunService>();
            container.RegisterType<IUrunResimService, UrunResimService>();
            container.RegisterType<IMarkaService, MarkaService>();
            container.RegisterType<IHaberBlogService, HaberBlogService>();
            container.RegisterType<ITaksitService, TaksitService>();
            container.RegisterType<ISiparisService, SiparisService>();
            container.RegisterType<IKisaLinkService, KisaLinkService>();
            container.RegisterType<IIstekListesiService, IstekListesiService>();
            container.RegisterType<IUrunYorumService, UrunYorumService>();
            container.RegisterType<IAdresService, AdresService>();
            container.RegisterType<IAdresIlService, AdresIlService>();
            container.RegisterType<IAdresIlceService, AdresIlceService>();
            container.RegisterType<IKullaniciAdresService, KullaniciAdresService>();
            container.RegisterType<IPaketService, PaketService>();
            container.RegisterType<IHaberBulteniAbonelikService, HaberBulteniAbonelikService>();
            container.RegisterType<IGonderimService, GonderimService>();
            container.RegisterType<ISiparisOdemeTipService, SiparisOdemeTipService>();
            container.RegisterType<IFaturaTipService, FaturaTipService>();
            container.RegisterType<IIadeTalepService, IadeTalepService>();
            container.RegisterType<IIadeTalepNedenTipService, IadeTalepNedenTipService>();
            container.RegisterType<IIadeTalepIstekTipService, IadeTalepIstekTipService>();
            container.RegisterType<ISiparisDetayService, SiparisDetayService>();
            container.RegisterType<IAnketService, AnketService>();
            container.RegisterType<IAnketCevapService, AnketCevapService>();
            container.RegisterType<IKampanyaService, KampanyaService>();
            container.RegisterType<ISirketService, SirketService>();
            container.RegisterType<ISanalPosService, SanalPosService>();
        }
    }
}