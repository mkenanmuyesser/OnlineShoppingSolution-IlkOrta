using CommerceProject.Business.BusinessContracts;
using CommerceProject.Business.BusinessServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CommerceProject.Business.Helper.Email
{
    public class EmailHelper
    {
        private static IIcerikAyarService IcerikAyarService = new IcerikAyarService();

        public static bool SendMail(string subject, string content, string sendMailAdress)
        {
            try
            {
                var icerikAyar = IcerikAyarService.GetFirst();

                SmtpClient smtpClient = new SmtpClient(icerikAyar.GonderilecekEpostaHost, icerikAyar.GonderilecekEpostaPort);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(icerikAyar.GonderilecekEpostaKullaniciAdi, icerikAyar.GonderilecekEpostaSifre);
                //smtpClient.EnableSsl = true;

                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.Subject = subject;
                mail.Body = content;               
                mail.From = new MailAddress(icerikAyar.GonderilecekEpostaKullaniciAdi, icerikAyar.GonderilecekEpostaTanim);
                mail.To.Add(new MailAddress(sendMailAdress));

                smtpClient.Send(mail);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
