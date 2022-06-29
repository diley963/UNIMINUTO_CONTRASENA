using System.Configuration;
using System.Net;
using System.Web.Mvc;

namespace PasswordChange.Controllers
{
    public class pruebaController : Controller
    {
        public class RECaptcha
        {
            public string Response { get; set; }
        }

        [HttpPost]
        public JsonResult AjaxMethod(string response)
        {
            RECaptcha recaptcha = new RECaptcha();
            var secretKey = ConfigurationManager.AppSettings["reCaptcha"];
            string url = "https://www.google.com/recaptcha/api/siteverify?secret=" + secretKey + "&response=" + response;
            recaptcha.Response = (new WebClient()).DownloadString(url);
            var result = recaptcha;
            return Json(result);
        }
    }
}