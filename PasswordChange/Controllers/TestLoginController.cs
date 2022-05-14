using Helpers.Implementations;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PasswordChange.Controllers
{
    public class TestLoginController : Controller
    {
        // GET: TestLogin
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public /*HttpStatusCodeResult*/HttpStatusCodeResult ValidateLogin(string userName , string password)
        {
            HttpStatusCodeResult rpt = null;

            //Hago un split por cada modulo
            string[] splitUserName = userName.Split('@');

            string dominio, user, pass;

            dominio = splitUserName[1];
            user = splitUserName[0];
            pass = password;//"@Admin1234";

            //Aquí va el path URL del servicio de directorio LDAP
  
            string path = "LDAP://192.168.101.200:389/OU=EGRESADO,OU=ACADEMICO,OU=USUARIOS UNIMINUTO,DC=UMDQA,DC=LOCAL";

            ChangeOfPasswordsDA obj = new ChangeOfPasswordsDA();


                if (obj.estaAutenticado(dominio, user, pass, path) == true)
                {
                    //Console.WriteLine("Autenticado en LDAP!");
                    rpt = new HttpStatusCodeResult(200);
                }
                else
                {
                    // Console.WriteLine("Error al Autenticar");
                    rpt = new HttpStatusCodeResult(404);

                }
                return rpt;

           // return Json( new { rpt } , JsonRequestBehavior.AllowGet);
        }
    }
}