using CommerceProject.Business.Entities;
using CommerceProject.Business.HelperClasses;

namespace CommerceProject.Business.BusinessContracts
{
    public interface IKullaniciService : IGenericRepository<Kullanici>
    {
        bool IsAuthenticated();

        bool LoginUser(string email, string password, bool rememberMe = false);

        bool LoginAdminUser(string email, string password, bool rememberMe = false);

        void LogoutUser();

        Kullanici GetAuthenticatedUser(bool asNoTracking = false);

        bool CreateUser(string ad, string soyad, string sifre, string eposta, int? sirketId = null);

        bool UpdateUserPassword(string eskiSifre, string yeniSifre);

        string GenerateUserPassword(string eposta);
    }
}
