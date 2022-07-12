using BotDetect.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceProject.Presentation.IlkOrta.Helper
{
    public static class CaptchaHelper
   {
        public static MvcCaptcha GetCaptcha(string captchaName,string inputName)
        {
            MvcCaptcha captcha = new MvcCaptcha(captchaName);
            captcha.UserInputID = inputName;            
            captcha.SoundEnabled = false;
            captcha.ImageStyle = BotDetect.ImageStyle.SunAndWarmAir;
            captcha.ImageSize = new System.Drawing.Size(250, 50);
            return captcha;
        }
    }
}