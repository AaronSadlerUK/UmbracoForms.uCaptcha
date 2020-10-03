using System;
using System.Configuration;

namespace UmbracoForms.uCaptcha
{
    public static class AppSettingsManager
    {
        public static string GetuCaptchaSiteKey()
        {
            if (ConfigurationManager.AppSettings["uCaptchaSiteKey"] != null)
                return ConfigurationManager.AppSettings["uCaptchaSiteKey"];

            throw new Exception("\"uCaptchaSiteKey\" is missing in AppSettings.");
        }

        public static string GetuCaptchaSecretKey()
        {
            if (ConfigurationManager.AppSettings["uCaptchaSecretKey"] != null)
                return ConfigurationManager.AppSettings["uCaptchaSecretKey"];

            throw new Exception("\"uCaptchaSecretKey\" is missing in AppSettings.");
        }

        public static string GetuCaptchaProvider()
        {
            if (ConfigurationManager.AppSettings["uCaptchaProvider"] != null)
                return ConfigurationManager.AppSettings["uCaptchaProvider"];

            throw new Exception("\"uCaptchaProvider\" is missing in AppSettings.");
        }
    }
}
