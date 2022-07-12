using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommerceProject.Admin.Helper;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using CommerceProject.Business.BusinessContracts;

namespace CommerceProject.Admin.Controllers
{
    public class YedeklemeBakimController : BaseController
    {
        private const string UploadDirectory = "/App_Data/";

        IIcerikAyarService IcerikAyarService;
        IKullaniciService KullaniciService;
        public YedeklemeBakimController(IIcerikAyarService iIcerikAyarService,
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
            ViewBag.PageName = PageHelper.Pages.yedeklemebakim_index;
            ViewBag.PageProperties = PageHelper.PageProperties.SetPageProperties("Yedekleme ve Bakım", "", "Yedekleme ve Bakım İşlemleri", "");

            return View();
        }
        #endregion

        #region Ajax Methods
        [HttpGet]
        public JsonResult Rebuild()
        {
            string rebuildScript = @"DECLARE @DatabaseName SYSNAME   = 'CommerceProjectDB'
                                    DECLARE @TableName VARCHAR(256)
                                    --DECLARE @FILLFACTOR INT = 85
                                    DECLARE @SQL NVARCHAR(MAX) =

                                     'DECLARE curAllIndex CURSOR FOR SELECT TABLE_SCHEMA +
                                     ''.'' + TABLE_NAME AS TABLENAME
                                     FROM ' + @DatabaseName + '.INFORMATION_SCHEMA.TABLES WHERE
                                     TABLE_TYPE = ''BASE TABLE'''  


                                    BEGIN
                                      EXEC sp_executeSQL @SQL
                                      OPEN curAllIndex
                                      FETCH NEXT FROM curAllIndex INTO @TableName
                                      WHILE(@@FETCH_STATUS = 0)
                                      BEGIN
                                           /* -- For using FillFactor setting. 
	                                       SET @DynamicSQL = 'ALTER INDEX ALL ON ' + @TableName +  
	                                       ' REBUILD WITH (FILLFACTOR = ' + CONVERT(VARCHAR,@FILLFACTOR) + ')' 
	                                       */
                                           SET @SQL = 'ALTER INDEX ALL ON ' + @TableName +
                                           ' REBUILD '
                                           PRINT @SQL
                                           EXEC sp_executeSQL @SQL
                                           FETCH NEXT FROM curAllIndex INTO @TableName
                                       END
                                       CLOSE curAllIndex
                                       DEALLOCATE curAllIndex
                                    END";

            var flag = false;
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbSql"].ConnectionString;
                string dbName = connectionString.Split(';')[1].Split('=')[1];
                SqlConnection cnn = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand(rebuildScript, cnn);

                cnn.Open();
                cmd.ExecuteNonQuery();
                cnn.Close();

                flag = true;
            }
            catch (Exception ex)
            {

            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Reorganize()
        {
            string rebuildScript = @"DECLARE @DatabaseName SYSNAME   = 'CommerceProjectDB'
                                    DECLARE @TableName VARCHAR(256)
                                    --DECLARE @FILLFACTOR INT = 85
                                    DECLARE @SQL NVARCHAR(MAX) =

                                     'DECLARE curAllIndex CURSOR FOR SELECT TABLE_SCHEMA +
                                     ''.'' + TABLE_NAME AS TABLENAME
                                     FROM ' + @DatabaseName + '.INFORMATION_SCHEMA.TABLES WHERE
                                     TABLE_TYPE = ''BASE TABLE'''  


                                    BEGIN
                                      EXEC sp_executeSQL @SQL
                                      OPEN curAllIndex
                                      FETCH NEXT FROM curAllIndex INTO @TableName
                                      WHILE(@@FETCH_STATUS = 0)
                                      BEGIN
                                           /* -- For using FillFactor setting. 
	                                       SET @DynamicSQL = 'ALTER INDEX ALL ON ' + @TableName +  
	                                       ' REORGANIZE WITH (FILLFACTOR = ' + CONVERT(VARCHAR,@FILLFACTOR) + ')' 
	                                       */
                                           SET @SQL = 'ALTER INDEX ALL ON ' + @TableName +
                                           ' REORGANIZE '
                                           PRINT @SQL
                                           EXEC sp_executeSQL @SQL
                                           FETCH NEXT FROM curAllIndex INTO @TableName
                                       END
                                       CLOSE curAllIndex
                                       DEALLOCATE curAllIndex
                                    END";

            var flag = false;
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbSql"].ConnectionString;
                string dbName = connectionString.Split(';')[1].Split('=')[1];
                SqlConnection cnn = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand(rebuildScript, cnn);

                cnn.Open();
                cmd.ExecuteNonQuery();
                cnn.Close();

                flag = true;
            }
            catch (Exception ex)
            {

            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult YedekDosyalariAra(int sayfaSayisi, int sayfaSirasi)
        {
            string backupDirectory = Server.MapPath(UploadDirectory);
            DirectoryInfo directoryInfo = new DirectoryInfo(backupDirectory);

            var sonucListesi = directoryInfo.GetFiles("*.bak").ToList();

            var count = sonucListesi.Count;

            sonucListesi = sonucListesi.Skip((sayfaSirasi - 1) * sayfaSayisi).Take(sayfaSayisi).ToList();

            return Json(new
            {
                sonucListesi = sonucListesi.Select(x => new
                {
                    Adi = x.Name.Split('.').First(),
                    Tarih = x.CreationTime.ToString("dd.MM.yyyy HH.mm"),
                }).ToList(),
                sayfaSayisi = sayfaSayisi,
                sayfaSirasi = sayfaSirasi,
                toplamKayitSayisi = count
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult YedekAl()
        {
            var flag = false;
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbSql"].ConnectionString;
                string dbName = connectionString.Split(';')[1].Split('=')[1];
                SqlConnection cnn = new SqlConnection(connectionString);

                string backupDirectory = Server.MapPath(UploadDirectory);
                string command = "Backup Database " + dbName + " to disk = '" + backupDirectory + "\\" + DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".bak'";
                SqlCommand cmd = new SqlCommand(command, cnn);

                cnn.Open();
                cmd.ExecuteNonQuery();
                cnn.Close();

                flag = true;
            }
            catch (Exception ex)
            {

            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public FileResult YedekIndir(string id)
        {
            string dosyaAdi = id + ".bak";

            string dosyaYolu = Server.MapPath(UploadDirectory + dosyaAdi);

            byte[] fileBytes = System.IO.File.ReadAllBytes(dosyaYolu);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, dosyaAdi);
        }

        [HttpGet]
        public JsonResult YedekSil(string id)
        {
            string dosyaAdi = id + ".bak";

            var flag = false;
            try
            {
                FileInfo dosya = new FileInfo(Server.MapPath(UploadDirectory + dosyaAdi));
                if (dosya != null)
                {
                    dosya.Delete();
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