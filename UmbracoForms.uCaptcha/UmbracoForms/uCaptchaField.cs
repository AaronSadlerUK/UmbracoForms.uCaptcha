using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Newtonsoft.Json;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using UmbracoForms.uCaptcha.Enums;
using UmbracoForms.uCaptcha.Helpers;
using UmbracoForms.uCaptcha.UmbracoForms.Models;

namespace UmbracoForms.uCaptcha.UmbracoForms
{
    [Serializable]
    public sealed class uCaptchaField : FieldType
    {
        private static string ProviderName => AppSettingsManager.GetuCaptchaProvider();
        public uCaptchaField()
        {
            Id = new Guid("76fc6a38-4517-4fea-b928-9ff20c626adb");
            Name = "uCaptcha";
            Description = "hCaptcha or Google Captcha bot protection";
            Icon = "icon-eye";
            DataType = FieldDataType.Bit;
            SortOrder = 10;
            SupportsRegex = false;
        }

        public override string GetDesignView()
        {
            return "~/App_Plugins/UmbracoForms.uCaptcha/Backoffice/Common/FieldTypes/uCaptchafield.html";
        }

        [Setting("Show Label", Description = "Show the property label", View = "~/App_Plugins/UmbracoForms/Backoffice/Common/SettingTypes/checkbox.html")]
        public string ShowLabel { get; set; }

        [Setting("Theme", Description = "uCaptcha theme", PreValues = "dark,light", View = "~/App_Plugins/UmbracoForms/Backoffice/Common/SettingTypes/dropdownlist.html")]
        public string Theme { get; set; }

        [Setting("Size", Description = "uCaptcha size", PreValues = "normal,compact,invisible", View = "~/App_Plugins/UmbracoForms/Backoffice/Common/SettingTypes/dropdownlist.html")]
        public string Size { get; set; }

        [Setting("reCaptcha Badge Position", Description = "Reposition the reCAPTCHA badge", PreValues = "bottomright,bottomleft,inline", View = "~/App_Plugins/UmbracoForms/Backoffice/Common/SettingTypes/dropdownlist.html")]
        public string reCaptchaBadgePosition { get; set; }

        [Setting("Error Message", Description = "The error message to display when the user does not pass the uCaptcha check, the default message is: \"You must check the \"I am human\" checkbox to continue\"", View = "~/App_Plugins/UmbracoForms/Backoffice/Common/SettingTypes/textfield.html")]
        public string ErrorMessage { get; set; }

        public override bool HideLabel => !Parse.Bool(ShowLabel);

        public override IEnumerable<string> RequiredJavascriptFiles(Field field)
        {
            var javascriptFiles = base.RequiredJavascriptFiles(field).ToList();
            if (ProviderName == Provider.Name.hCaptcha.ToString())
            {
                javascriptFiles.Add(Consts.hCaptcha.JsResource);
            }
            else if (ProviderName == Provider.Name.reCaptcha.ToString())
            {
                javascriptFiles.Add(Consts.reCaptcha.JsResource);
            }

            if (field.Settings.ContainsKey("Size") && field.Settings["Size"] == "invisible")
            {
                if (ProviderName == Provider.Name.hCaptcha.ToString())
                {
                    javascriptFiles.Add($"~/App_Plugins/UmbracoForms.uCaptcha/Assets/{Consts.hCaptcha.LocalInvisibleJsResource}");
                }
                else if (ProviderName == Provider.Name.reCaptcha.ToString())
                {
                    javascriptFiles.Add($"~/App_Plugins/UmbracoForms.uCaptcha/Assets/{Consts.reCaptcha.LocalInvisibleJsResource}");
                }
            }
            else
            {
                if (ProviderName == Provider.Name.hCaptcha.ToString())
                {
                    javascriptFiles.Add($"~/App_Plugins/UmbracoForms.uCaptcha/Assets/{Consts.hCaptcha.LocalJsResource}");
                }
                else if (ProviderName == Provider.Name.reCaptcha.ToString())
                {
                    javascriptFiles.Add($"~/App_Plugins/UmbracoForms.uCaptcha/Assets/{Consts.reCaptcha.LocalJsResource}");
                }
            }

            return javascriptFiles;
        }

        public override IEnumerable<string> ValidateField(Form form, Field field, IEnumerable<object> postedValues, HttpContextBase context, IFormStorage formStorage)
        {
            string verifyUrl = null;
            string verifyPostParameter = null;
            if (ProviderName == Provider.Name.hCaptcha.ToString())
            {
                verifyUrl = Consts.hCaptcha.VerifyUrl;
                verifyPostParameter = Consts.hCaptcha.VerifyPostParameter;
            }
            else if (ProviderName == Provider.Name.reCaptcha.ToString())
            {
                verifyUrl = Consts.reCaptcha.VerifyUrl;
                verifyPostParameter = Consts.reCaptcha.VerifyPostParameter;
            }

            if (verifyUrl == null)
            {
                throw new Exception("\"uCaptchaProvider\" is missing or incorrect in AppSettings.");
            }

            string errorMessage;
            if (field.Settings.ContainsKey("ErrorMessage") && !string.IsNullOrEmpty(field.Settings["ErrorMessage"]))
            {
                errorMessage = field.Settings["ErrorMessage"];
            }
            else
            {
                errorMessage = "You must check the \"I am human\" checkbox to continue";
            }
            var returnStrings = new List<string>();

            var secretKey = AppSettingsManager.GetuCaptchaSecretKey();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                client.DefaultRequestHeaders.Add("Accept", "*/*");

                var parameters = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("response", context.Request.Form[verifyPostParameter]),
                    new KeyValuePair<string, string>("secret", secretKey),
                    new KeyValuePair<string, string>("remoteip", context.Request.UserHostAddress)
                };

                var request = new HttpRequestMessage(HttpMethod.Post, verifyUrl)
                {
                    Content = new FormUrlEncodedContent(parameters)
                };

                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = response.Content.ReadAsStringAsync();
                    jsonString.Wait();

                    var result = JsonConvert.DeserializeObject<uCaptchaVerifyResponse>(jsonString.Result);

                    if (!result.Success)
                    {
                        returnStrings.Add(errorMessage);
                    }
                }

                if (!response.IsSuccessStatusCode)
                {
                    returnStrings.Add(errorMessage);
                }
            }

            return returnStrings;
        }
    }
}