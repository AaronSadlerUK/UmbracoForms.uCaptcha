using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Newtonsoft.Json;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using UmbracoForms.HCaptcha;
using UmbracoForms.HCaptcha.UmbracoForms.Models;

public sealed class hCaptchaField : FieldType
{
    public hCaptchaField()
    {
        Id = new Guid("76fc6a38-4517-4fea-b928-9ff20c626adb");
        Name = "hCaptcha";
        Description = string.Empty;
        Icon = "icon-eye";
        DataType = FieldDataType.Bit;
        SortOrder = 10;
        SupportsRegex = false;
    }

    [Setting("Theme", Description = "hCaptcha theme", PreValues = "dark,light", View = "~/App_Plugins/UmbracoForms/Backoffice/Common/SettingTypes/dropdownlist.html")]
    public string Theme { get; set; }

    [Setting("Size", Description = "hCaptcha size", PreValues = "normal,compact", View = "~/App_Plugins/UmbracoForms/Backoffice/Common/SettingTypes/dropdownlist.html")]
    public string Size { get; set; }

    [Setting("ErrorMessage", Description = "The error message to display when the user does not pass the hCaptcha check, the default message is: \"You must check the \"I am human\" checkbox to continue\"", View = "~/App_Plugins/UmbracoForms/Backoffice/Common/SettingTypes/textfield.html")]
    public string ErrorMessage { get; set; }

    public override IEnumerable<string> ValidateField(Form form, Field field, IEnumerable<object> postedValues, HttpContextBase context, IFormStorage formStorage)
    {
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

        var secretKey = AppSettingsManager.GethCaptchaSecretKey();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            client.DefaultRequestHeaders.Add("Accept", "*/*");

            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("response", context.Request.Form["h-captcha-response"]),
                new KeyValuePair<string, string>("secret", secretKey),
                new KeyValuePair<string, string>("remoteip", context.Request.UserHostAddress)
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://hcaptcha.com/siteverify")
            {
                Content = new FormUrlEncodedContent(parameters)
            };

            var response = client.SendAsync(request).Result;

            if (response.IsSuccessStatusCode)
            {
                var jsonString = response.Content.ReadAsStringAsync();
                jsonString.Wait();

                var result = JsonConvert.DeserializeObject<HCaptchaVerifyResponse>(jsonString.Result);

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