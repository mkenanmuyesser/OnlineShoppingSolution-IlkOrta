using System;
using System.Collections.Generic;
using System.Linq;
using CommerceProject.Business.BusinessContracts;
using System.Web;
using System.Web.Mvc;
using CommerceProject.Business.Entities;
using System.Data;
using CommerceProject.Admin.Helper;

namespace CommerceProject.Admin.Controllers
{
    public class RolController : BaseController
    {
        #region Excel Action
        public class ExcelActionResult : ActionResult
        {
            private readonly DataTable _content;

            public ExcelActionResult(DataTable content)
            {
                _content = content;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                context.HttpContext.Response.Clear();
                context.HttpContext.Response.ClearContent();
                context.HttpContext.Response.ClearHeaders();
                context.HttpContext.Response.Buffer = true;
                context.HttpContext.Response.Write(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">");
                context.HttpContext.Response.Write(@"<meta http-equiv=""content-type"" content=""application/vnd.ms-excel; charset=UTF-8"">");
                context.HttpContext.Response.AddHeader("Content-Disposition", "attachment;filename=Rapor.xls");

                //sets font
                context.HttpContext.Response.Write("<font style='font-size:10.0pt; font-family:Calibri;'>");
                context.HttpContext.Response.Write("<BR><BR><BR>");
                //sets the table border, cell spacing, border color, font of the text, background, foreground, font height
                context.HttpContext.Response.Write("<Table border='1' bgColor='#ffffff' " +
                  "borderColor='#000000' cellSpacing='0' cellPadding='0' " +
                  "style='font-size:11.0pt; font-family:Calibri; background:white;'> <TR>");

                //am getting my grid's column headers
                int columnscount = _content.Columns.Count;

                for (int j = 0; j < columnscount; j++)
                {   //write in new column
                    context.HttpContext.Response.Write("<Td bgColor='#cacaca'>");
                    //Get column headers  and make it as bold in excel columns
                    context.HttpContext.Response.Write("<B>");
                    context.HttpContext.Response.Write(_content.Columns[j].ColumnName.ToString());
                    context.HttpContext.Response.Write("</B>");
                    context.HttpContext.Response.Write("</Td>");
                }
                context.HttpContext.Response.Write("</TR>");
                foreach (DataRow row in _content.Rows)
                {
                    context.HttpContext.Response.Write("<TR>");

                    for (int i = 0; i < _content.Columns.Count; i++)
                    {
                        context.HttpContext.Response.Write("<Td>");
                        context.HttpContext.Response.Write(row[i].ToString());
                        context.HttpContext.Response.Write("</Td>");
                    }

                    context.HttpContext.Response.Write("</TR>");
                }
                context.HttpContext.Response.Write("</Table>");
                context.HttpContext.Response.Write("</font>");
                context.HttpContext.Response.Flush();
                context.HttpContext.Response.End();
            }
        }
        #endregion

        IIcerikAyarService IcerikAyarService;
        IKullaniciService KullaniciService;
        IRolService RolService;
        IKullaniciRolService KullaniciRolService;
        IKullaniciDetayService KullaniciDetayService;
        public RolController(IIcerikAyarService iIcerikAyarService,
                             IKullaniciService iKullaniciService,
                             IRolService iRolService,
                             IKullaniciRolService iKullaniciRolService,
                             IKullaniciDetayService iKullaniciDetayService) : base(iIcerikAyarService,
                                                                                   iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;

            RolService = iRolService;
            KullaniciRolService = iKullaniciRolService;
            KullaniciDetayService = iKullaniciDetayService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.rol_index;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Rol", "Arama", "Rol Arama İşlemleri", "");

            return View();
        }      

        [AuthorizeManager]
        public ActionResult Save(int? id = null)
        {
            ViewBag.PageName = PageHelper.Pages.rol_save;

            if (id == null)
            {
                ViewBag.Rol = new Rol();
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Rol", "Kayıt", "Rol Kayıt İşlemleri", "");
            }
            else
            {
                ViewBag.Rol = RolService.FindBy(x => x.RolId == id).SingleOrDefault();
                ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Rol", "Güncelleme", "Rol Güncelleme İşlemleri", "");

                ViewBag.KullaniciListesi = (from kr in KullaniciRolService.GetAll(true).ToList()
                                            join k in KullaniciService.GetAll(true).ToList() on kr.KullaniciId equals k.KullaniciId
                                            join d in KullaniciDetayService.GetAll(true).ToList() on k.KullaniciId equals d.KullaniciDetayId
                                            where kr.RolId == id
                                            select new Kullanici()
                                            {
                                                Eposta = k.Eposta,
                                                SonGirisTarihi = k.SonGirisTarihi,
                                                UyelikTarihi = k.UyelikTarihi,
                                                AktifMi = k.AktifMi,
                                                KullaniciDetay = new KullaniciDetay()
                                                {
                                                    Ad = d.Ad,
                                                    Soyad = d.Soyad
                                                }
                                            }
                                            ).ToList();
            }

            return View();
        }

        public ActionResult ExcelRaporuAl(string adi, int aktiflik)
        {
            var kullaniciRoller = KullaniciRolService.GetAll(true).ToList();

            var sonucListesi = RolService.FindBy(x =>
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true)))
            ).ToList()
            .Select(x => new
            {
                Adi = x.Adi,
                AktifKullaniciSayisi = kullaniciRoller.Where(u => u.RolId == x.RolId && u.AktifMi == true).ToList().Count(),
                PasifKullaniciSayisi = kullaniciRoller.Where(u => u.RolId == x.RolId && u.AktifMi == false).ToList().Count(),
                AktifMi = x.AktifMi ? "Aktif" : "Pasif"
            })
            .ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region Ajax Methods
        [HttpPost]
        public JsonResult KaydetGuncelle(Rol rol)
        {
            try
            {
                Rol _rol;

                if (rol.RolId == 0)
                {
                    _rol = new Rol();
                    _rol.Adi = rol.Adi;
                    _rol.AktifMi = rol.AktifMi;

                    RolService.Add(_rol);
                }
                else
                {
                    _rol = RolService.FindBy(x => x.RolId == rol.RolId).SingleOrDefault();

                    if (_rol == null)
                        return Json(false, JsonRequestBehavior.DenyGet);

                    _rol.Adi = rol.Adi;
                    _rol.AktifMi = rol.AktifMi;

                    RolService.Edit(_rol);
                }

                var flag = RolService.Save();

                if (flag)
                    return Json(_rol.RolId, JsonRequestBehavior.DenyGet);
                return Json(0, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult Sil(int id)
        {
            try
            {
                if (id == 0)
                    return Json(false, JsonRequestBehavior.DenyGet);

                var _rol = RolService.FindBy(x => x.RolId == id).SingleOrDefault();

                if (_rol == null)
                    return Json(false, JsonRequestBehavior.DenyGet);

                _rol.AktifMi = false;

                RolService.Edit(_rol);

                var flag = RolService.Save();

                // Role ait kullanıcı rolleri pasif
                var kullaniciRoller = KullaniciRolService.FindBy(x => x.RolId == id && x.AktifMi == true).ToList();

                if (kullaniciRoller.Count > 0)
                {
                    foreach (var kRol in kullaniciRoller)
                    {
                        var _kRol = KullaniciRolService.FindBy(x => x.KullaniciRolId == kRol.KullaniciRolId).SingleOrDefault();

                        if (_kRol != null)
                        {
                            _kRol.AktifMi = false;

                            KullaniciRolService.Edit(_kRol);

                            KullaniciRolService.Save();
                        }
                    }
                }

                return Json(flag, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpGet]
        public JsonResult Ara(string adi, int aktiflik, int sayfaSayisi, int sayfaSirasi)
        {
            var kullanicilar = KullaniciService.GetAll(true).ToList();
            var kullaniciRoller = KullaniciRolService.GetAll(true).ToList();

            var tempList = RolService.FindBy(x =>
            (string.IsNullOrEmpty(adi) || x.Adi.Contains(adi)) &&
            (aktiflik == 2 || ((aktiflik == 0 && x.AktifMi == false) || (aktiflik == 1 && x.AktifMi == true))));

            var count = tempList.Count();

            var sonucListesi = tempList.OrderBy(x => x.RolId).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    RolId = x.RolId,
                    Adi = x.Adi,
                    AktifKullaniciSayisi = (from kr in kullaniciRoller
                                            join k in kullanicilar on kr.KullaniciId equals k.KullaniciId
                                            where kr.RolId == x.RolId && k.AktifMi == true
                                            select kr).Count(),
                    PasifKullaniciSayisi = (from kr in kullaniciRoller
                                            join k in kullanicilar on kr.KullaniciId equals k.KullaniciId
                                            where kr.RolId == x.RolId && k.AktifMi == false
                                            select kr).Count(),
                    AktifMi = x.AktifMi
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RoleAitKullanicilariGetir(int id, bool aktifMi)
        {
            var kullanicilar = (from kr in KullaniciRolService.GetAll(true).ToList()
                                join k in KullaniciService.GetAll(true).ToList() on kr.KullaniciId equals k.KullaniciId
                                join d in KullaniciDetayService.GetAll(true).ToList() on k.KullaniciId equals d.KullaniciDetayId
                                where kr.RolId == id && k.AktifMi == aktifMi
                                select new
                                {
                                    Ad = d.Ad,
                                    Soyad = d.Soyad,
                                    Eposta = k.Eposta,
                                    EpostaOnayliMi = k.EpostaOnayliMi,
                                    HesapKilitliMi = k.HesapKilitliMi,
                                    AktifMi = k.AktifMi
                                }).ToList();

            return Json(new
            {
                kullaniciListesi = kullanicilar
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}