using CommerceProject.Business.BusinessContracts;
using CommerceProject.Business.BusinessServices;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace CommerceProject.Admin
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

            container.RegisterType<IVergiService, VergiService>();
            container.RegisterType<IGonderimService, GonderimService>();
            container.RegisterType<ISirketService, SirketService>();
            container.RegisterType<IOlcuService, OlcuService>();
            container.RegisterType<IOlcuTipService, OlcuTipService>();
            container.RegisterType<IMarkaService, MarkaService>();
            container.RegisterType<IHaberBulteniAbonelikService, HaberBulteniAbonelikService>();
            container.RegisterType<IHaberBlogService, HaberBlogService>();
            container.RegisterType<IHaberBlogResimService, HaberBlogResimService>();
            container.RegisterType<IHaberBlogTipService, HaberBlogTipService>();
            container.RegisterType<IHaberBlogYorumService, HaberBlogYorumService>();
            container.RegisterType<IKullaniciService, KullaniciService>();
            container.RegisterType<IKullaniciDetayService, KullaniciDetayService>();
            container.RegisterType<IKullaniciAdresService, KullaniciAdresService>();
            container.RegisterType<IKampanyaService, KampanyaService>();
            container.RegisterType<IKampanyaResimService, KampanyaResimService>();
            container.RegisterType<IAnketService, AnketService>();
            container.RegisterType<IAnketSoruService, AnketSoruService>();
            container.RegisterType<IAnketCevapService, AnketCevapService>();
            container.RegisterType<INitelikService, NitelikService>();
            container.RegisterType<INitelikGrupService, NitelikGrupService>();
            container.RegisterType<IUrunNitelikService, UrunNitelikService>();
            container.RegisterType<IIcerikAyarService, IcerikAyarService>();
            container.RegisterType<IIstekListesiService, IstekListesiService>();
            container.RegisterType<IUrunService, UrunService>();
            container.RegisterType<IKategoriService, KategoriService>();
            container.RegisterType<IUrunKategoriService, UrunKategoriService>();
            container.RegisterType<ISanalPosService, SanalPosService>();
            container.RegisterType<IBankaService, BankaService>();
            container.RegisterType<IHesapNumarasiService, HesapNumarasiService>();
            container.RegisterType<ITaksitService, TaksitService>();
            container.RegisterType<IKisaLinkService, KisaLinkService>();
            container.RegisterType<IIskontoService, IskontoService>();
            container.RegisterType<IIskontoTipService, IskontoTipService>();
            container.RegisterType<IIskontoKategoriService, IskontoKategoriService>();
            container.RegisterType<IIskontoKullanimHareketService, IskontoKullanimHareketService>();
            container.RegisterType<IIskontoMarkaService, IskontoMarkaService>();
            container.RegisterType<IIskontoRolService, IskontoRolService>();
            container.RegisterType<IIskontoSirketService, IskontoSirketService>();
            container.RegisterType<IIskontoUrunService, IskontoUrunService>();
            container.RegisterType<ISiparisService, SiparisService>();
            container.RegisterType<ISiparisDurumTipService, SiparisDurumTipService>();
            container.RegisterType<ISiparisGonderimService, SiparisGonderimService>();
            container.RegisterType<IRolService, RolService>();
            container.RegisterType<IKullaniciRolService, KullaniciRolService>();
            container.RegisterType<IAdresService, AdresService>();
            container.RegisterType<IAdresIlService, AdresIlService>();
            container.RegisterType<IAdresIlceService, AdresIlceService>();
            container.RegisterType<IUrunResimService, UrunResimService>();
            container.RegisterType<IUrunYorumService, UrunYorumService>();
            container.RegisterType<IUrunResimTipService, UrunResimTipService>();
            container.RegisterType<IIlgiliUrunService, IlgiliUrunService>();
            container.RegisterType<IPaketService, PaketService>();
            container.RegisterType<IPaketUrunService, PaketUrunService>();
            container.RegisterType<IPaketResimService, PaketResimService>();
            container.RegisterType<IPaketKategoriService, PaketKategoriService>();
            container.RegisterType<IPaketNitelikService, PaketNitelikService>();
            container.RegisterType<ISepetService, SepetService>();
            container.RegisterType<ISepetTipService, SepetTipService>();
            container.RegisterType<IIadeTalepService, IadeTalepService>();
            container.RegisterType<IIadeTalepDurumTipService, IadeTalepDurumTipService>();
            container.RegisterType<IIadeTalepIstekTipService, IadeTalepIstekTipService>();
            container.RegisterType<IIadeTalepNedenTipService, IadeTalepNedenTipService>();
            container.RegisterType<IBannerService, BannerService>();
            container.RegisterType<IBannerTipService, BannerTipService>();
            container.RegisterType<IStokHareketService, StokHareketService>();
            container.RegisterType<IStokHareketTipService, StokHareketTipService>();
            container.RegisterType<IOdemeDurumTipService, OdemeDurumTipService>();
            container.RegisterType<ISiparisOdemeTipService, SiparisOdemeTipService>();
            container.RegisterType<ISiparisDetayService, SiparisDetayService>();
            container.RegisterType<ISiparisHareketService, SiparisHareketService>();
            container.RegisterType<ITeslimZamaniService, TeslimZamaniService>();
            container.RegisterType<IFaturaTipService, FaturaTipService>();
            container.RegisterType<IOzellikService, OzellikService>();
            container.RegisterType<IUrunOzellikService, UrunOzellikService>();
            container.RegisterType<IKategoriOzellikService, KategoriOzellikService>();
            container.RegisterType<IKomisyonIskontoTipService, KomisyonIskontoTipService>();
            container.RegisterType<ICacheService, CacheService>();
            container.RegisterType<ILogService, LogService>();
            container.RegisterType<IIpAdresService, IpAdresService>();
            container.RegisterType<IYetkiService, YetkiService>();
            container.RegisterType<IKullaniciYetkiService, KullaniciYetkiService>();
        }
    }
}