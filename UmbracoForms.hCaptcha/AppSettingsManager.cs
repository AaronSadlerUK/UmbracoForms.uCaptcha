using System;
using System.Configuration;

namespace UmbracoForms.HCaptcha
{
    public static class AppSettingsManager
    {
        public static string GethCaptchaSiteKey()
        {
            if (ConfigurationManager.AppSettings["hCaptchaSiteKey"] != null)
                return ConfigurationManager.AppSettings["hCaptchaSiteKey"];

            throw new Exception("\"hCaptchaSiteKey\" is missing in AppSettings.");
        }

        public static string GethCaptchaSecretKey()
        {
            if (ConfigurationManager.AppSettings["hCaptchaSecretKey"] != null)
                return ConfigurationManager.AppSettings["hCaptchaSecretKey"];

            throw new Exception("\"hCaptchaSecretKey\" is missing in AppSettings.");
        }
    }
}
