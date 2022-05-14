using System.Web;
using System.Web.Optimization;

namespace PasswordChange
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            //Template css files are added
            bundles.Add(new StyleBundle("~/ContentTemplate/css").Include(
                     "~/Content/Template/bootstrap/css/bootstrap.min.css",
                    "~/Content/Template/assets/css/plugins.css",
                    "~/Content/Template/assets/css/scrollspyNav.css",
                    "~/Content/Template/assets/css/components/tabs-accordian/custom-accordions.css",
                    "~/Content/Template/plugins/bootstrap-select/bootstrap-select.min.css",
                     "~/Content/Template/assets/css/authentication/form-1.css",
                     "~/Content/Template/assets/css/forms/theme-checkbox-radio.css",
                     "~/Content/Template/assets/css/forms/switches.css",
                     "~/Content/Template/assets/css/elements/alert.css"));
            

            //Template css files are added
            bundles.Add(new StyleBundle("~/ContentTemplate2/css").Include(
                     "~/Content/Template/bootstrap/css/bootstrap.min.css",
                    "~/Content/Template/assets/css/plugins.css",
                    "~/Content/Template/assets/css/scrollspyNav.css",
                    "~/Content/Template/assets/css/components/tabs-accordian/custom-accordions.css",
                    "~/Content/Template/plugins/bootstrap-select/bootstrap-select.min.css",
                     "~/Content/Template/assets/css/authentication/form-2.css",
                     "~/Content/Template/assets/css/forms/theme-checkbox-radio.css",
                     "~/Content/Template/assets/css/forms/switches.css",
                     "~/Content/Template/assets/css/elements/alert.css"));


            //Template Js Files are added
            bundles.Add(new ScriptBundle("~/ContentTemplate/js").Include(
                      "~/Content/Template/assets/js/libs/jquery-3.1.1.min.js",
                      "~/Content/Template/bootstrap/js/popper.min.js",
                      "~/Content/Template/bootstrap/js/bootstrap.min.js",
                      "~/Content/Template/assets/js/authentication/form-1.js",
                      "~/Content/Template/plugins/perfect-scrollbar/perfect-scrollbar.min.js",
                      "~/Content/Template/assets/js/app.js"));

            //Template Js Files are added
            bundles.Add(new ScriptBundle("~/ContentTemplate2/js").Include(
                      "~/Content/Template/assets/js/libs/jquery-3.1.1.min.js",
                      "~/Content/Template/bootstrap/js/popper.min.js",
                      "~/Content/Template/bootstrap/js/bootstrap.min.js",
                      "~/Content/Template/assets/js/authentication/form-1.js",
                      "~/Content/Template/plugins/perfect-scrollbar/perfect-scrollbar.min.js",
                      "~/Content/Template/assets/js/app.js"));



        }
    }
}
