using Business.Implementations;
using Helpers;
using Helpers.Implementations;
using Microsoft.Web.Helpers;
using Model;
using PasswordChange.Filter;
using PasswordChange.Models;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PasswordChange.Controllers
{ 
    public class HomeController : Controller
    {
        //cambio desde el local
        #region PROPIEDADES
        int contador = 0;
        //variable to block Process and become 1 to 1
        private static readonly object LockEmail = new object();
        private UserLockedBusiness userlockedB = new UserLockedBusiness();
        private CodeByUserBusiness codebyuserB = new CodeByUserBusiness();

        public string attemps = ConfigurationManager.AppSettings["Ini:attemps_number"];
        public string retry_change_pass_minutes = ConfigurationManager.AppSettings["Ini:retry_change_pass_minutes"];
        public string expiration_time_minutes = ConfigurationManager.AppSettings["Ini:expiration_time_minutes"];

        #endregion

        #region VISTAS
        public ActionResult Index()
        {
            //will store the count of the number of attempts to enter a verification code.
            Session["missingAttempts"] = 0;
            //will store the user's account type 
            Session["typeAccount"] = "";
            return View();
        }

        [Seguridad]
        /// <summary>
        /// form Receiving the Document and user's email to Return your information
        /// </summary>
        /// <param name="user"></param>
        /// <returns>returns User Information</returns>
        public ActionResult FormMethodOfShipment()
        {
            User user = (User)Session["DataUser"];

            //We verify that if the user arrives null it is redirected to an exit view.
            if (user.nDocument == null)
            {
                return RedirectToAction("ExitPage", new { code = 404, mensaje = "Debe ingresar sus datos en la página inicial" });
            }

            try
            {
                //We verify that if the user is blocked we take him to an exit page.
                UserLocked userlocked = userlockedB.GetUserBlocked(user.nDocument, user.email);
                if (userlocked.cedula != null && userlocked.email != null && userlocked.fDesbloqueo > DateTime.Now)
                {
                    return RedirectToAction("ExitPage", new
                    {
                        code = 300,
                        mensaje = "Usuario bloqueado temporalmente por " + retry_change_pass_minutes + " minutos"
                    });
                }

                //if the user is not locked, the code-locked flag is set to false
                // codebyuserB.UpdateFlagCodBloqueado(user.nDocument, user.email, false);
            }
            catch (Exception e)
            {
                new ExceptionHelper(e, "FormMethodOfShipment");
                return RedirectToAction("ExitPage", new { code = 470, mensaje = "Ocurrió un fallo verificando si el usuario está bloqueado" });

            }
            //we fill the ViewBag with the values of the Enum of the sending method
            ViewBag.sendbyemail = MethodSendCode.email;
            ViewBag.sendbymobile = MethodSendCode.mobile;
            ViewBag.tengoCodigo = MethodSendCode.tengoCodigo;
            ViewBag.mobileReplace = "Indefinido";
            ViewBag.emailReplace = "Indefinido";

            string number = user.mobile;
            string email = user.alternativeEmail;

            if (!string.IsNullOrEmpty(number))
            {
                ViewBag.mobileReplace = number.Replace(number.Substring(number.Length - 4), "****");
            }

            if (!string.IsNullOrEmpty(email) && email.Contains("@"))
            {
                string[] splitemail = email.Split('@');
                string email1 = splitemail[0].Replace(splitemail[0].Substring(splitemail[0].Length - 4), "****");
                ViewBag.emailReplace = email1 + "@" + splitemail[1];
            }

            return View();
        }

        [Seguridad]
        /// <summary>
        /// form where you enter the verification code sent to your email or phone number
        /// </summary>
        /// <returns></returns>
        public ActionResult CodeConfirmationForm()
        {
            User user = (User)Session["DataUser"];
            ViewBag.attemps = attemps;
            ViewBag.retry_change_pass_minutes = retry_change_pass_minutes;
            if (user.nDocument == null)
            {
                return RedirectToAction("ExitPage", new
                {
                    code = 400,
                    mensaje = "Para acceder a esta página debe ingresar sus datos personales en la página de inicio "
                });
            }
            return View();
        }

        /// <summary>
        /// Will return a hearing in case of success or failure. 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="mensaje"></param>
        /// <returns></returns>
        public ActionResult Exitpage(int code = 0, string mensaje = "No se pasó ningún mensaje")
        {
            ViewBag.Message = mensaje;
            ViewBag.Code = code;
            return View();
        }

        #endregion

        #region METODOS AUXILIARES

        /// <summary>
        /// Validate if the code is valid
        /// </summary>
        /// <param name="code"></param>
        /// <returns>1 if it is valid and 0 if it is not valid</returns>
        public JsonResult CodeIsValid(int code)
        {
            User user = (User)Session["DataUser"];
            UserLocked userLocked = new UserLocked()
            {
                cedula = user.nDocument,
                email = user.email
            };
            string messageReply = string.Empty;
            int response = 0;
            int missingAttempts = 0;

            if (Session["missingAttempts"] != null)
            {
                Session["missingAttempts"] = Convert.ToInt32(Session["missingAttempts"].ToString()) + 1;
                missingAttempts = Convert.ToInt32(Session["missingAttempts"].ToString());
            }

            //we consult the user code
            CodeByUser codebyuser = new CodeByUser();
            int codVerificacion = 0;
            try
            {
                //We consult the user code in bd
                codebyuser = codebyuserB.GetCodeByUser(userLocked.cedula, userLocked.email);
                codVerificacion = int.Parse(codebyuser.codVerificacion);

                //validate if the code has expired
                if (codebyuser.fCaducidad < DateTime.Now)
                {
                    //return -1;
                    response = -1;
                    messageReply = "Este código ha caducado por favor solicite un código nuevo";
                    return Json(new { messageReply, response, missingAttempts }, JsonRequestBehavior.AllowGet);
                }

                //We verify that the user does not have the code blocked because he/she exceeded the number of attempts allowed.
                if (codebyuser.codBloqueado)
                {
                    // return -2;
                    response = -2;
                    messageReply = "Este código ya fue utilizado por favor solicite un código nuevo";
                    return Json(new { messageReply, response, missingAttempts }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {

                new ExceptionHelper(e, "CodeIsValid");
            }



            //we validate that the user code is equal to the one entered in the form
            if (code == codVerificacion)
            {
                // return 1;
                response = 1;
                messageReply = "Éxito código validado";
                return Json(new { messageReply, response, missingAttempts }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //if an invalid code is entered the n allowed times the user is temporarily blocked. 
                if (/*missingAttempts*/ Convert.ToInt32(Session["missingAttempts"].ToString()) >= int.Parse(attemps))
                {
                    try
                    {
                        userLocked.fDesbloqueo = DateTime.Now.AddMinutes(int.Parse(retry_change_pass_minutes));
                        userlockedB.InsertBlockedUser(userLocked);
                        //if you try more than the allowed number of times, we update the blocked code flag to true.
                        codebyuserB.UpdateFlagCodBloqueado(userLocked.cedula, userLocked.email, true);
                    }
                    catch (Exception e)
                    {
                        new ExceptionHelper(e, "CodeIsValid");
                    }

                }

                // return 0;
                response = 2;
                messageReply = "Código erróneo inténtelo nuevamente tiene ";
                return Json(new { messageReply, response, missingAttempts }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Method for sending code either by phone or mail
        /// </summary>
        /// <param name="metodo">if 1 is 1 it is sent by phone if 2 it is 2 it is sent by email</param>
        /// <returns>returns true if the message could be sent and false if it could not.</returns>        
        public JsonResult SendCode(string metodo)
        {
            User user = (User)Session["DataUser"];
            string messageReply = "";
            //flag indicating whether the code has been sent or not
            bool isSend = false;
            contador++;
            if (contador <=2)
            {               

                if (metodo.Equals("tengoCodigo"))
                {
                    isSend = true;
                    messageReply = "tengoCodigo";
                    return Json(new { messageReply, isSend }, JsonRequestBehavior.AllowGet);
                }

                //It is called an auxiliary class that generates random codes.
                GenerateRandomNumbers Generatecode = new GenerateRandomNumbers();
                string codeGenerate = "";
                try
                {
                    codeGenerate = Generatecode.GenerateNumber();
                }
                catch (Exception e)
                {

                    new ExceptionHelper(e, "SendCode");
                }
                //
                CodeByUser cod = new CodeByUser();

                //add a default code in case the generated code comes empty or null
                if (string.IsNullOrEmpty(codeGenerate))
                {
                    // codeGenerate = "581126";
                    Random generator = new Random();
                    codeGenerate = generator.Next(0, 1000000).ToString("D6");
                }

                try
                {

                    //we set the values to the codeByUser object with the User's data 
                    cod.cedula = user.nDocument;
                    cod.email = user.email;
                    cod.codVerificacion = codeGenerate;
                    //fCaducidad = DateTime.Now.AddMinutes(int.Parse(expiration_time_minutes))
                    cod.fCaducidad = DateTime.Now.AddMinutes(Convert.ToInt32(expiration_time_minutes));

                }
                catch (Exception e)
                {
                    messageReply = "Error en la conversion de FCaducidad";
                    isSend = false;
                    new ExceptionHelper(e, "SendCode convert FCaducidad ****");
                    return Json(new { messageReply, isSend }, JsonRequestBehavior.AllowGet);
                }


                if (metodo.Equals("mobile")/*==1*/)
                {
                    //validate that if the message is sent by mobile the number is not empty
                    user.mobile = "573508045496";
                    if (!String.IsNullOrEmpty(user.mobile))
                    {
                        // SendSmsWhitTwilio SendSMS = new SendSmsWhitTwilio();

                        //captures what the method returns SendMsmMasivApi
                        int respuestaMasivApi = (int)responseSendMasivApi.default_value;
                        try
                        {
                            // SendSMS.SendMessage(user.mobile, codeGenerate)

                            string mobileDecript1 = user.mobile;

                            string mobileDecript = user.mobile.Replace(user.mobile.Substring(user.mobile.Length - 4), "****");

                            respuestaMasivApi = SendMsmMasivApi(mobileDecript1, codeGenerate);

                            respuestaMasivApi = 0;

                            //we validate that the method returns and display the messages
                            if (respuestaMasivApi == (int)responseSendMasivApi.sms_send_success)
                            {
                                messageReply = "Fue enviado con éxito el Código al celular =>" + mobileDecript;//user.mobile;
                                isSend = true;
                                codebyuserB.InsertCodeByUser(cod);
                            }
                            else if (respuestaMasivApi == (int)responseSendMasivApi.error_destination_number)
                            {
                                messageReply = "No se envio el mensaje de texto error de numero de destino => " + mobileDecript;//user.mobile;
                            }
                            else if (respuestaMasivApi == (int)responseSendMasivApi.credential_error)
                            {
                                messageReply = "No se envio el mensaje de texto error de credenciales en api Masiv";
                            }
                            else if (respuestaMasivApi == (int)responseSendMasivApi.encript_decrypt_error)
                            {
                                messageReply = "No se envio el mensaje de texto error en Masiv desencriptando informacion";
                            }
                            else
                            {
                                messageReply = "No se envio el mensaje de texto error en Metodo Masiv";
                            }

                        }
                        catch (Exception e)
                        {
                            //messageReply = "Excepcion al enviar el Codigo al celular Detalle ==> " + e.Message;
                            messageReply = "Ocurrió un fallo al enviar código al celular ";
                            new ExceptionHelper(e, "SendCode");
                        }
                    }
                    else
                    {
                        messageReply = "No se puede enviar código al teléfono porque este campo está vacío Elija la opción Email";
                    }

                }
                else if (metodo.Equals("email"))
                {

                    //validate that if the message is sent by email the email is not empty
                    if (!String.IsNullOrEmpty(user.alternativeEmail))
                    {
                        try
                        {
                            //Devolver despues de pruebas

                            string messageEmail = "Estimado(a)  <b>" + user.nombre + "</b> TU CLAVE UNIMINUTO te informa que el código de verificación es <b>" + codeGenerate +
                                "</b>. Recuerda que el código expirará en <b>" + expiration_time_minutes + "minutos</b>. Si tienes alguna duda puedes ingresar a nuestra Mesa de Servicio de Tecnología en http://soporte.uniminuto.edu/ o marca a la línea telefónica de tu sede o la línea nacional 01 8000 119 390 y digita la EXT. 6622, y un agente de servicio tecnológico te atenderá." +
                                "<br> *Esta es una notificación automática, por favor no respondas este mensaje.";

                            //CAMBIO 
                            string decodeb64AlternativeEmail = user.alternativeEmail;

                            string[] splitemail = user.alternativeEmail.Split('@');
                            string email1 = splitemail[0].Replace(splitemail[0].Substring(splitemail[0].Length - 4), "****");
                            string decodeEmail = email1 + "@" + splitemail[1];

                            SendEmail(/*"smtp-mail.outlook.com"*/"smtp.uniminuto.edu", Convert.ToInt32("25"), /*"hbt_send@hotmail.com"*/ /*"no-reply@uniminuto.edu"*/"tuclave@uniminuto.edu", ""/*"HBTAsdf1234$"*/, messageEmail, "Code Autenticacion", "UNIMINUTO", decodeb64AlternativeEmail/*user.alternativeEmail*//*"hbtpruebas13@gmail.com"*/);
                            messageReply = "El código fue enviado con éxito al email " + decodeEmail; //user.alternativeEmail;

                            isSend = true;
                            codebyuserB.InsertCodeByUser(cod);
                        }
                        catch (Exception e)
                        {

                            messageReply = "No se pudo enviar el código al email";
                            new ExceptionHelper(e, "SendCode");
                        }
                    }
                    else
                    {
                        messageReply = "No se puede enviar código al email porque este campo esta vacío elija opción teléfono";
                    }
                }

            }
            return Json(new { messageReply, isSend }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Method in charge of sending mails 
        /// </summary>
        /// <param name="Host">Host Email</param>
        /// <param name="Port">Port Email</param>
        /// <param name="Username">Username Email</param>
        /// <param name="Password">Password Email</param>
        /// <param name="emailbody">message body</param>
        /// <param name="emailsubject"></param>
        /// <param name="sendername">Sender name Email</param>
        /// <param name="recipients">Receiver Email</param>
        private void SendEmail(string Host, int Port, string Username, string Password, string emailbody, string emailsubject, string sendername, string recipients)
        {

            // The Mailing Process is blocked to be done 1 by 1
            lock (LockEmail)
            {
                EmailHelper email = null;
                email = new EmailHelper(MailServiceType.Smtp, Host, Convert.ToInt32(Port), Username, Password);
                email.SendEmail(sendername, Username, recipients, emailsubject, emailbody, null);

            };
        }
             

        /// <summary>
        /// Method that sends text messages to phones using MasivApi appi
        /// </summary>
        /// <param name="mobile">mobile number</param>
        /// <returns>int status response</returns>
        public int SendMsmMasivApi(string mobile, string codGenerate)
        {
            string strRetorno = string.Empty;
            //string message = "El Codigo de verificacion para cambio de credenciales es => " + codGenerate;
            string message = "El codigo de TU CLAVE UNIMINUTO es " + codGenerate + ". El codigo expirara en " + expiration_time_minutes + " minutos.";

            //read webconfig 
            string masiv_user_encript = ConfigurationManager.AppSettings["Masiv:user"];
            string masiv_pass_encript = ConfigurationManager.AppSettings["Masiv:pass"];
            string key_encript_decript = ConfigurationManager.AppSettings["key:encript_decript"];

            //will store the decrypted information
            string masiv_user = string.Empty;
            string masiv_pass = string.Empty;

            //capture the answer
            int respuesta = (int)responseSendMasivApi.default_value;

            try
            {
                EncryptService obj = new EncryptService();
                //Decrypt data
                masiv_user = obj.DecryptString(key_encript_decript, masiv_user_encript);
                masiv_pass = obj.DecryptString(key_encript_decript, masiv_pass_encript);
            }
            catch (Exception e)
            {

                new ExceptionHelper(e, "SendMsmMasivApi intentado Desencriptar");
                respuesta = (int)responseSendMasivApi.encript_decrypt_error;

            }


            try
            {
                //string strURLCompleta = "https://api-sms.masivapp.com/SmsHandlers/sendhandler.ashx?action=sendmessage&username=UNIMINUTO_PRUEBAS_5UB0Z&password=6qPvZe$ly.&recipient=573217115109&messagedata=HelloWorld&longMessage=false&flash=false&premium=false ";
                // string strURLCompleta = "https://api-sms.masivapp.com/SmsHandlers/sendhandler.ashx?action=sendmessage&username=" + masiv_user + "&password=" + masiv_pass + "&recipient=" + mobile + "&messagedata=" + message + "&longMessage=false&flash=false&premium=false ";
                string strURLCompleta = "https://api-sms.masivapp.com/SmsHandlers/sendhandler.ashx?Action=sendmessage&username=" + masiv_user + "&password=" + masiv_pass + "&recipient=" + mobile + "&messagedata=" + message + "&longMessage=false&url=";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(strURLCompleta);
                httpWebRequest.ContentType = "application/json";

                httpWebRequest.Method = "GET";
                httpWebRequest.ContentLength = 0;

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    strRetorno = streamReader.ReadToEnd();
                }

                //we validate if the message was sent or if any failure occurred
                if (strRetorno.Contains("<statuscode>0</statuscode>") &&
                    strRetorno.Contains("<statusmessage>Message accepted for delivery</statusmessage>"))
                {

                    //indicates message sent successfully
                    respuesta = (int)responseSendMasivApi.sms_send_success;
                }
                else if (strRetorno.Contains("<errorcode>1157</errorcode>") ||
                  strRetorno.Contains("<errormessage>Invalid username or password.</errormessage>"))
                {
                    //indicates that an error occurred because the credentials are not correct.
                    respuesta = (int)responseSendMasivApi.credential_error;
                }
                else if (strRetorno.Contains("<errorcode>1</errorcode>") ||
                    strRetorno.Contains("<errormessage>Invalid recipient.</errormessage>"))
                {
                    //indicates that an error occurred because the destination number is invalid.
                    respuesta = (int)responseSendMasivApi.error_destination_number;
                }

                return respuesta;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Consumes a Rest Api that returns information about a student based on his or her ID.
        /// </summary>
        /// <param name="UiEstudiante">Nº Id estudiante</param>
        /// <returns></returns>
        public string ConsultarApiBanner(string UiEstudiante)
        {
            string strRetorno = string.Empty;
            //Estudiante student = new Estudiante();
            try
            {
                string strURLCompleta = "https://webapi.uniminuto.edu/API/BannerEstudiante/DatosPersonales/" + UiEstudiante;
                //string strURLCompleta = "http://10.0.36.175:81/API/BannerEstudiante/DatosPersonales/" + UiEstudiante;
                //string strURLCompleta = "http://10.0.36.178:81/API/BannerEstudiante/DatosPersonales/" + UiEstudiante;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(strURLCompleta);
                httpWebRequest.ContentType = "application/json";

                httpWebRequest.Method = "GET";
                httpWebRequest.ContentLength = 0;

                String username = "DARA";
                String password = "MTIzNDU2";
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                httpWebRequest.Headers.Add("Authorization", "Basic " + encoded);

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    strRetorno = streamReader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                new ExceptionHelper(e, "ConsultarApiBanner");
            }

            return strRetorno;
        }

        /// <summary>
        /// Consumes a Soap Api that returns information about a Docente or Admistrative based on his or her ID.
        /// </summary>
        /// <param name="cedula">Nº Id Docente</param>
        /// <returns></returns>
        public Docente ConsultarApiBoomi(string cedula, string strRetorno = "")
        {
            Docente docente = new Docente();
            try
            {
                boomi.WebServiceProviderPortTypeClient ws = new boomi.WebServiceProviderPortTypeClient();
                boomi.ObtenerDatos ob = new boomi.ObtenerDatos();
                ob.idColaborador = cedula;
                ws.Open();
                var respuesta = ws.ObtenerDatos(ob);
                //if the answer is successful I fill the object otherwise I set it to null
                if (!respuesta.ResultadoTransaccion.Codigo.Equals("Z1002"))
                {
                    docente.Uid_Usuario = respuesta.Docente.Uid_Usuario;
                    docente.Nombre = respuesta.Docente.Nombre;
                    docente.Apellidos = respuesta.Docente.Apellidos;
                    docente.Cedula = respuesta.Docente.Cedula;
                    docente.Celular = respuesta.Docente.Celular;
                    docente.Telefono = respuesta.Docente.Telefono;
                    docente.Email = respuesta.Docente.Mail;
                }
            }
            catch (Exception e)
            {
                try
                {
                    //I unrealize the user's information to know what role he/she has  
                    UserJson data = new JavaScriptSerializer().Deserialize<UserJson>(strRetorno);

                    if (!data.Pager.Equals(cedula))
                    {
                        new ExceptionHelper(e, "ConsultarApiBoomi =>  detalle " + "No se encontró este número de documento en el sistema");
                    }

                    if (string.IsNullOrEmpty(data.HomePhone) && string.IsNullOrEmpty(data.msds_cloudextensionattribute2))
                    {
                        new ExceptionHelper(e, "ConsultarApiBoomi");
                        docente.Uid_Usuario = null;
                        docente.Nombre = null;
                        docente.Apellidos = null;
                        docente.Cedula = null;
                        docente.Celular = null;
                        docente.Telefono = null;
                    }
                    else
                    {
                        docente.Uid_Usuario = data.Pager;
                        docente.Nombre = data.FirstName;
                        docente.Apellidos = data.LastName;
                        docente.Cedula = data.Pager;
                        docente.Celular = data.HomePhone;
                        docente.Telefono = data.HomePhone;
                        docente.Email = data.msds_cloudextensionattribute2;
                    }
                }
                catch (Exception ex)
                {

                    new ExceptionHelper(ex, "ConsultarApiBoomi segunda Excepcion");
                }
            }

            return docente;

        }

        /// <summary>
        /// Consumes a Soap Api that returns information about a Docente or Admistrative based on his or her ID.
        /// </summary>
        /// <param name="cedula">Nº Id Docente</param>
        /// <returns></returns>
        public Docente ConsultarApiBoomiTest(string cedula)
        {
            Docente docente = new Docente();
            try
            {
                boomiTest.WebServiceProviderPortTypeClient ws = new boomiTest.WebServiceProviderPortTypeClient();
                boomiTest.ObtenerDatos ob = new boomiTest.ObtenerDatos();
                ob.idColaborador = cedula;
                ws.Open();
                var respuesta = ws.ObtenerDatos(ob);
                //if the answer is successful I fill the object otherwise I set it to null
                if (!respuesta.ResultadoTransaccion.Codigo.Equals("Z1002"))
                {
                    docente.Uid_Usuario = respuesta.Docente.Uid_Usuario;
                    docente.Nombre = respuesta.Docente.Nombre;
                    docente.Apellidos = respuesta.Docente.Apellidos;
                    docente.Cedula = respuesta.Docente.Cedula;
                    docente.Celular = respuesta.Docente.Celular;
                    docente.Telefono = respuesta.Docente.Telefono;
                    //Campo Nuevo agregado al email del endpoint
                    docente.Email = respuesta.Docente.Mail;
                }
            }
            catch (Exception e)
            {

                new ExceptionHelper(e, "ConsultarApiBoomi");
                docente.Uid_Usuario = null;
                docente.Nombre = null;
                docente.Apellidos = null;
                docente.Cedula = null;
                docente.Celular = null;
                docente.Telefono = null;
            }

            return docente;

        }

        /// <summary>
        /// validates if the user entered exists
        /// </summary>
        /// <param name="cedula"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public JsonResult ValidateUser(string cedula, string email)
        {

            int existe = 0;
            string messageReply = "";
            string strRetorno = string.Empty;

            User filtro = new User();
            Session["DataUser"] = filtro;
            Session["IsValidUser"] = true;
            Session["typeAccount"] = "ESTUDIANTE";


            try
            {
                //string strURLCompleta = "http://10.0.36.175:81/API/LDAP/AutenticarCambioContrasena?email=" + email;
                //string strURLCompleta = "http://10.0.36.178:81/API/LDAP/AutenticarCambioContrasena?email=" + email;
                string strURLCompleta = "http://10.0.36.178:8078/API/LDAP/AutenticarCambioContrasena?email=" + email;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(strURLCompleta);
                httpWebRequest.ContentType = "application/json";

                httpWebRequest.Method = "GET";
                httpWebRequest.ContentLength = 0;

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    strRetorno = streamReader.ReadToEnd();
                }

                //Validate if the User was not found.
                if (strRetorno.Contains("999") && (strRetorno.Contains("Usuario no encontrado en Ldap")
                                                   || strRetorno.Contains("Usuario no encontrado en DA")))
                {
                    messageReply = "Usuario no encontrado en el sistema valide su información";
                    return Json(new { existe, messageReply, filtro }, JsonRequestBehavior.AllowGet);
                }

                if (strRetorno.Contains("999") && (strRetorno.Contains("Usuario Inactivo en Ldap")
                                                   || strRetorno.Contains("Usuario Inactivo en DA")))
                {
                    messageReply = "Usuario inactivo en el sistema";
                    return Json(new { existe, messageReply, filtro }, JsonRequestBehavior.AllowGet);
                }

                //I unrealize the user's information to know what role he/she has  
                UserJson data = new JavaScriptSerializer().Deserialize<UserJson>(strRetorno);

           
                if (!data.Pager.Equals(cedula))
                {
                    messageReply = "No se encontró este número de documento en el sistema";
                    return Json(new { existe, messageReply, filtro }, JsonRequestBehavior.AllowGet);
                }


                if (data.Descripcion.Equals("ADMINISTRATIVO") || data.Descripcion.Equals("DOCENTE") || data.Descripcion.Equals("BANACADEMICO"))
                {
                    Docente UserDocente = null;
                    //we add validation to see if we are using the api pager or the form's cedula
                    if (!string.IsNullOrEmpty(data.Pager))
                    {
                        UserDocente = ConsultarApiBoomi(data.Pager, strRetorno);
                        //UserDocente = ConsultarApiBoomiTest(data.Pager);
                    }
                    else
                    {
                        UserDocente = ConsultarApiBoomi(cedula, strRetorno);
                        //UserDocente = ConsultarApiBoomiTest(cedula);
                    }

                    //I bring the data of boomi if it is not nulla full the object
                    if (UserDocente.Nombre != null && UserDocente.Apellidos != null && UserDocente.Cedula != null)
                    {
                        filtro.apellido = UserDocente.Apellidos;
                        filtro.nombre = UserDocente.Nombre;
                        filtro.nDocument = UserDocente.Cedula;
                        filtro.mobile = UserDocente.Celular;
                        filtro.email = email;
                        filtro.alternativeEmail = UserDocente.Email;
                        filtro.id = int.Parse(UserDocente.Uid_Usuario);
                        filtro.descripcion = data.Descripcion;
                        existe = 1;
                        Session["IsValidUser"] = true;
                        //I save the account type in a session variable 
                        Session["typeAccount"] = Convert.ToString(data.Descripcion);
                    }
                    else
                    {
                        messageReply = "No existe este usuario, por favor verifique su información";

                    }

                }
                else if (data.Descripcion.Equals("ESTUDIANTE") || data.Descripcion.Equals("EGRESADO") || data.Descripcion.Equals("GRADUADO"))
                {
                    //I get banner data as a string  
                    string stringObject = ConsultarApiBanner(data.Cn);

                    //I derealize the data to know the answer and the user's info
                    RespuestaEstudianteJson studentDeserealizado = new JavaScriptSerializer().Deserialize<RespuestaEstudianteJson>(stringObject);

                    //if the answer is successful, I fill the object
                    if (studentDeserealizado.resultadoTransaccion.Codigo.Equals("1") && studentDeserealizado.resultadoTransaccion.Mensaje.Equals("OK"))
                    {
                        filtro.apellido = studentDeserealizado.estudiante.Apellido;
                        filtro.nombre = studentDeserealizado.estudiante.Nombre;
                        filtro.email = studentDeserealizado.estudiante.EmailInstitucional;
                        if (!string.IsNullOrEmpty(data.Pager))
                        {
                            filtro.nDocument = data.Pager;//cedula;
                        }
                        else
                        {
                            filtro.nDocument = cedula;
                        }
                        filtro.mobile = studentDeserealizado.estudiante.TelefonoMovil;
                        filtro.alternativeEmail = studentDeserealizado.estudiante.Email;
                        filtro.id = int.Parse(studentDeserealizado.estudiante.UidEstudiante);
                        filtro.descripcion = data.Descripcion;
                        existe = 1;
                        Session["IsValidUser"] = true;
                        //I save the account type in a session variable 
                        Session["typeAccount"] = Convert.ToString(data.Descripcion);
                    }
                    else
                    {
                        messageReply = "No existe este usuario estudiante verifique su información";
                    }


                }
                else if (data.Descripcion.Equals("TERCEROS"))
                {

                    filtro.nombre = data.FirstName;
                    filtro.apellido = data.LastName;
                    filtro.nDocument = data.Pager;
                    filtro.email = data.msds_cloudextensionattribute2;
                    filtro.alternativeEmail = data.Mail;
                    filtro.descripcion = data.Descripcion;
                    filtro.mobile = data.HomePhone;
                    existe = 1;
                    Session["IsValidUser"] = true;

                    //I save the account type in a session variable 
                    Session["typeAccount"] = Convert.ToString(data.Descripcion);
                }
                else
                {
                    messageReply = "Hay un fallo con el tipo de rol de este usuario";
                }

            }
            catch (Exception e)
            {
                new ExceptionHelper(e, "ValidateUser");
                messageReply = "Usuario no encontrado por favor valide su email y numero de identificación";
                return Json(new { existe, messageReply, filtro }, JsonRequestBehavior.AllowGet);
            }


            return Json(new { existe, messageReply, filtro }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// validates that the password update has been performed 
        /// </summary>
        /// <param name="email">Email of user</param>
        /// <param name="password">new Password</param>
        /// <returns></returns>
        public JsonResult IsUpdatePassword(string password)
        {
            User user = (User)Session["DataUser"];
            int isUpdate = 0;
            bool isUpdateDA = false;
            string messageReply = "";
            //Encrypted Password
            byte[] myByte = System.Text.Encoding.UTF8.GetBytes(password);
            string passworB64 = Convert.ToBase64String(myByte);

            //Send Post Request

            if (Session["typeAccount"] != null && !String.IsNullOrEmpty(Session["typeAccount"].ToString()))
            {

                ChangeOfPasswordsDA serviceDA = new ChangeOfPasswordsDA();
                try
                {
                    //will store true if the password was updated
                    isUpdateDA = serviceDA.updatePassword(Session["typeAccount"].ToString(), user.email, password);

                    if (isUpdateDA)
                    {
                        //actualizo bien
                        messageReply = "La contraseña fue cambiada con éxito, en un lapso de uno a dos minutos la podrás utilizar para acceder a tu cuenta.";
                        //we block the code 
                        //Devolver despues de pruebas
                        codebyuserB.UpdateFlagCodBloqueado(user.nDocument, user.email, true);

                        //we obtain the ip and write a log with that name
                        string userIP = Request.UserHostAddress;
                        new WriteInfoUserInLog(user.nDocument, user.email, userIP);
                        isUpdate = 1;
                    }
                    else
                    {
                        messageReply = "Fallo al cambiar contraseña no se encontro informacion del usuario";
                    }

                }
                catch (Exception e)
                {

                    new ExceptionHelper(e, "IsUpdatePassword");
                    messageReply = "Fallo Al Cambiar Password Ocurrio Una Excepcion";

                }


            }

            return Json(new { isUpdate, messageReply }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Decodifica una cadena que este en base 64
        /// </summary>
        /// <param name="textb64">texto en base64</param>
        /// <returns>retorna un string decodificado en base64</returns>
        private string decryptBase64(string textb64)
        {
            //CAMBIO 
            byte[] b64 = null;
            string decodeb64 = string.Empty;
            try
            {
                b64 = Convert.FromBase64String(textb64);
                decodeb64 = Encoding.UTF8.GetString(b64);
            }
            catch (Exception e)
            {
                return string.Empty;
            }

            return decodeb64;
        }

        #endregion
    }
}