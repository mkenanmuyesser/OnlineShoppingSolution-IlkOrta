using CommerceProject.Business.BusinessContracts;
using CommerceProject.Business.Entities;
using CommerceProject.Business.Custom;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Reflection;

namespace CommerceProject.Admin.Helper
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

            public static Kullanici LoggedInUser(IKullaniciService iKullaniciService)
            {
                return iKullaniciService.GetAuthenticatedUser();
            }

            public static PageProperties SetPageProperties(string breadcrumbLevel1,
                                                    string breadcrumbLevel2,
                                                    string pageHeader,
                                                    string pageDescription)
            {
                PageProperties pageProperties = new PageProperties
                {
                    BreadcrumbLevel1 = breadcrumbLevel1,
                    BreadcrumbLevel2 = breadcrumbLevel2,
                    PageHeader = pageHeader,
                    PageDescription = pageDescription
                };

                return pageProperties;
            }
        }
        #endregion

        #region Helper Methods
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                dataTable.Columns.Add(prop.Name);
            }

            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }

                dataTable.Rows.Add(values);

            }

            return dataTable;
        }

        public List<KategoriTreeNodeDataObj> FillRecursive(List<Kategori> flatObjects, int? parentId = null)
        {
            return flatObjects.Where(x => x.UstKategoriId.Equals(parentId)).Select(item => new KategoriTreeNodeDataObj
            {
                KategoriAdi = item.Adi,
                KategoriId = item.KategoriId,
                Children = FillRecursive(flatObjects, item.KategoriId)
            }).ToList();
        }

        #endregion

        #region Enums
        public enum Pages
        {
            home_index = 1,
            anket_index,
            anket_save,
            banka_accountnumbersave,
            banka_accountnumbersearch,
            banka_installmentsave,
            banka_installmentsearch,
            banka_virtualpossave,
            banner_index,
            banner_save,
            cachetemizleme_index,
            epostaislem_index,
            gonderim_index,
            gonderim_save,
            haberblog_index,
            haberblog_save,
            haberbulteni_index,
            iadetalep_index,
            iadetalep_save,
            iskonto_index,
            iskonto_save,
            isteklistesi_index,
            isteklistesi_save,
            kampanya_index,
            kampanya_save,
            siparisgonderim_index,
            siparisgonderim_save,
            kategori_index,
            kategori_save,
            kategori_list,
            kisalink_index,
            kisalink_save,
            kullanici_index,
            kullanici_save,
            login_index,
            marka_index,
            marka_save,
            nitelik_index,
            nitelik_save,
            nitelikgrup_index,
            nitelikgrup_save,
            odemeyontemi_save,
            olcu_index,
            olcu_save,
            paket_index,
            paket_save,
            programayar_index,
            programlog_index,
            rapor_index,
            rapor_onlineusers,
            rol_index,
            rol_save,
            sepet_index,
            siparis_index,
            siparis_save,
            sirket_index,
            sirket_save,
            smsislem_index,
            stokhareket_index,
            stokhareket_savestockin,
            stokhareket_savestockout,
            stokhareket_search,
            urun_index,
            urun_save,
            urun_card,
            urun_search,
            vergi_index,
            vergi_save,
            yedeklemebakim_index
        }
        #endregion
    }
}