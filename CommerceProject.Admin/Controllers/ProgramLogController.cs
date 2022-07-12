using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommerceProject.Admin.Helper;
using CommerceProject.Business.BusinessContracts;
using System.Data;

namespace CommerceProject.Admin.Controllers
{
    public class ProgramLogController : BaseController
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
        ILogService LogService;
        public ProgramLogController(IIcerikAyarService iIcerikAyarService,
                                    IKullaniciService iKullaniciService,
                                    ILogService iLogService) : base(iIcerikAyarService,
                                                                                iKullaniciService)
        {
            IcerikAyarService = iIcerikAyarService;
            KullaniciService = iKullaniciService;
            LogService = iLogService;
        }

        #region Actions
        [AuthorizeManager]
        public ActionResult Index()
        {
            ViewBag.PageName = PageHelper.Pages.programlog_index;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Program Log", "", "Program Log Kayıtları", "");

            ViewBag.KullaniciListesi = KullaniciService.GetAll(true, new string[] { "KullaniciDetay" }).ToList().OrderBy(x => x.KullaniciDetay.Ad).ThenBy(x => x.KullaniciDetay.Soyad).Select(x => new { id = x.Eposta, text = x.KullaniciDetay.Ad + " " + x.KullaniciDetay.Soyad + "(" + x.Eposta + ")", aktif = x.AktifMi }).ToList();

            return View();
        }

        public ActionResult ExcelRaporuAl(string logMesaji, string logTipi, string kullaniciEposta, DateTime? baslangicTarihi, DateTime? bitisTarihi)
        {
            var sonucListesi = LogService.FindBy(x =>
            (string.IsNullOrEmpty(logMesaji) || x.log_message.ToLower().Contains(logMesaji)) &&
            (string.IsNullOrEmpty(logTipi) || x.log_level == logTipi) &&
            (string.IsNullOrEmpty(kullaniciEposta) || x.log_user_name == kullaniciEposta) &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                ((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= x.entered_date && bitisTarihi >= x.entered_date)) ||
                ((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= x.entered_date)) ||
                ((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= x.entered_date))
            )
            ).ToList().Select(x => new
            {
                Entered_Date = x.entered_date.HasValue ? x.entered_date.Value.ToString("dd.MM.yyyy HH:mm") : "",
                Log_Application = x.log_application,
                Log_Date = x.log_date,
                Log_Level = x.log_level,
                Log_Logger = x.log_logger,
                Log_Message = x.log_message,
                Log_User = x.log_user_name,
                Log_Machine = x.log_machine_name,
                Log_CallSite = x.log_call_site,
                Log_Thread = x.log_thread,
                Log_Exception = x.log_exception,
                Log_StackTrace = x.log_stacktrace
            }).ToList();

            var dt = new PageHelper().ToDataTable(sonucListesi);

            return new ExcelActionResult(dt);
        }
        #endregion

        #region Ajax Methods
        [HttpGet]
        public JsonResult Ara(string logMesaji, string logTipi, string kullaniciEposta, DateTime? baslangicTarihi, DateTime? bitisTarihi, int sayfaSayisi, int sayfaSirasi)
        {
            var tempList = LogService.FindBy(x =>
            (string.IsNullOrEmpty(logMesaji) || x.log_message.ToLower().Contains(logMesaji)) &&
            (string.IsNullOrEmpty(logTipi) || x.log_level == logTipi) &&
            (string.IsNullOrEmpty(kullaniciEposta) || x.log_user_name == kullaniciEposta) &&
            (
                (baslangicTarihi == null && bitisTarihi == null) ||
                ((baslangicTarihi != null && bitisTarihi != null) && (baslangicTarihi <= x.entered_date && bitisTarihi >= x.entered_date)) ||
                ((baslangicTarihi != null && bitisTarihi == null) && (baslangicTarihi <= x.entered_date)) ||
                ((baslangicTarihi == null && bitisTarihi != null) && (bitisTarihi >= x.entered_date))
            ));

            var count = tempList.Count();

            var sonucListesi = tempList.OrderByDescending(x => x.log_date).Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    Date = x.entered_date.HasValue ? x.entered_date.Value.ToString("dd.MM.yyyy HH:mm") : "",
                    LogType = x.log_level,
                    LogApplication = x.log_application,
                    User = x.log_user_name,
                    Machine = x.log_machine_name,
                    CallSite = x.log_call_site,
                    Thread = x.log_thread,
                    Message = x.log_message,
                    Exception = x.log_exception,
                    StackTrace = x.log_stacktrace
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}