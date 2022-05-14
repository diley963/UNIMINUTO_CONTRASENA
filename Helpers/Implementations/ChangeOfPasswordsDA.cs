using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Implementations
{
    public class ChangeOfPasswordsDA
    {
        private string daDomain = ConfigurationManager.AppSettings["DA:Domain"];
        private string daUser = ConfigurationManager.AppSettings["DA:User"];
        private string daPassword = ConfigurationManager.AppSettings["DA:Password"];
        private string daRamaGeneral = ConfigurationManager.AppSettings["DA:RamaGeneral"];
        private string key_encript_decript = ConfigurationManager.AppSettings["key:encript_decript"];
        public bool updatePassword(string tipoCuenta,string email,string password)
        {

            bool isUpdate = false;

           // string tipoCuenta = "ADMINISTRATIVO";
            string rama = "";
            switch (tipoCuenta)
            {
                case "BANACADEMICO":
                    rama = "OU=BANACADEMICO,OU=ACADEMICO,OU=USUARIOS UNIMINUTO,DC=UMDQA,DC=LOCAL";
                    break;
                case "DOCENTE":
                    rama = "OU=DOCENTES,OU=ACADEMICO,OU=USUARIOS UNIMINUTO,DC=UMDQA,DC=LOCAL";
                    break;
                case "EGRESADO":
                    rama = "OU=EGRESADO,OU=ACADEMICO,OU=USUARIOS UNIMINUTO,DC=UMDQA,DC=LOCAL";
                    break;
                case "ESTUDIANTE":
                    rama = "OU=ESTUDIANTE,OU=ACADEMICO,OU=USUARIOS UNIMINUTO,DC=UMDQA,DC=LOCAL";
                    break;
                case "GRADUADO":
                    rama = "OU=EGRESADO,OU=ACADEMICO,OU=USUARIOS UNIMINUTO,DC=UMDQA,DC=LOCAL";
                    break;
                case "ADMINISTRATIVO":
                    rama = "OU=ADMINISTRATIVO,OU=USUARIOS UNIMINUTO,DC=UMDQA,DC=LOCAL";
                    break;
                case "TERCEROS":
                    rama = "OU=EXTERNOS,OU=USUARIOS UNIMINUTO,DC=UMDQA,DC=LOCAL";
                    break;
                default:
                    break;
            }

            try
            {
                rama = daRamaGeneral;//"OU=USUARIOS UNIMINUTO,DC=UMDQA,DC=LOCAL";
                string DomainPath = @"LDAP://" + daDomain + "/" + daRamaGeneral; //"LDAP://192.168.101.200:389/" + rama;
                DirectoryEntry adSearchRoot = new DirectoryEntry(DomainPath);

                adSearchRoot.AuthenticationType = AuthenticationTypes.Secure;
                //adSearchRoot.Username = "webestudiantes";
                //adSearchRoot.Password = "C0nsulT$42585*";

                //Desencripto los datos de daUser y daPassword             
                try
                {
                    EncryptService obj = new EncryptService();
                    //Decrypt data
                    daUser = obj.DecryptString(key_encript_decript, daUser);
                    daPassword = obj.DecryptString(key_encript_decript, daPassword);
                }
                catch (Exception e)
                {
                    new ExceptionHelper(e, "updatePassword() intentado Desencriptar");
                }

                adSearchRoot.Username = daUser;
                adSearchRoot.Password = daPassword;

                //obtengo el userName sin el @
                string name = email.Substring(0, email.IndexOf("@"));

                //al
                string typeFilter = string.Empty;
                if(tipoCuenta.Equals("TERCEROS") || tipoCuenta.Equals("TERCERO"))
                {
                    typeFilter = "(name=" + name + ")";
                }
                else
                {
                  typeFilter = "(mail=" + email + ")";
                  //typeFilter = "(mail=xavier.pruebas@uniminuto.edu.co)";
                }


                DirectorySearcher adSearcher = new DirectorySearcher(adSearchRoot);
                // adSearcher.Filter = "(mail=xavier.pruebas@uniminuto.edu.co)";
                // adSearcher.Filter = "(mail=fabio.castellanos@uniminuto.edu)";
                // adSearcher.Filter = "(name=proveedor.prueba)";
                adSearcher.Filter = typeFilter;
                SearchResult result = adSearcher.FindOne();

                if (result != null)
                {
                    DirectoryEntry userEntry = result.GetDirectoryEntry();
                    if (userEntry != null)
                    {

                        string distinguishedname = (string)userEntry.Properties["distinguishedname"][0];
                        // ChangePassworduserAccount("fabio.castellanos", "Password1234$#2021", distinguishedname);
                       isUpdate = ChangePassworduserAccount(name, password, distinguishedname);

                    }
                }
            }
            catch (Exception e)
            {

                throw e;
            }
            return isUpdate;
        }

        private bool ChangePassworduserAccount(string user, string newPassword, string ramaCnUser = "")
        {
            bool isChanged = false;

            try
            {

                //NetworkCredential credentials = new NetworkCredential("webestudiantes", "C0nsulT$42585*");
                NetworkCredential credentials = new NetworkCredential(daUser, daPassword);

                using (LdapConnection con = new LdapConnection(daDomain/*"192.168.101.200:389"*/))
                {
                    con.Credential = credentials;
                    con.AuthType = AuthType.Negotiate; // esto tiene varios tipos, si falla, lo primero que hay que hacer es probar cambiando el tipo
                    con.SessionOptions.SecureSocketLayer = false;
                    con.SessionOptions.Sealing = true;
                    con.Bind();

                    DirectoryAttributeModification[] damList = null;
                    DirectoryAttributeModification modifyUserPassword = new DirectoryAttributeModification();
                    modifyUserPassword.Operation = DirectoryAttributeOperation.Replace;
                    modifyUserPassword.Name = "unicodePwd"; // este es el atributo a modificar.
                    modifyUserPassword.Add(BuildBytePWD(newPassword));
                    damList = new DirectoryAttributeModification[] { modifyUserPassword };
                    //ModifyRequest modifyRequest = new ModifyRequest("CN=fabio.castellanos,OU=SERVICIOS INTEGRADOS,OU=ADMINISTRATIVO,OU=USUARIOS UNIMINUTO,DC=UMDQA,DC=LOCAL", damList);
                    ModifyRequest modifyRequest = new ModifyRequest(ramaCnUser, damList);
                    // ModifyRequest modifyRequest = new ModifyRequest("CN=xavier.pruebas,OU=EGRESADO,OU=ACADEMICO,OU=USUARIOS UNIMINUTO,DC=UMDQA,DC=LOCAL", damList);
                    DirectoryResponse response = con.SendRequest(modifyRequest);
                }

                using (LdapConnection con2 = new LdapConnection(daDomain/*"192.168.101.200:389"*/))
                {
                    credentials.UserName = user;
                    credentials.Password = newPassword;
                    con2.Credential = credentials;
                    con2.AuthType = AuthType.Negotiate; // esto tiene varios tipos, si falla, lo primero que hay que hacer es probar cambiando el tipo
                    con2.SessionOptions.Sealing = true;
                    con2.Bind();
                    //flag que indica que se cambio la contraseña
                    isChanged = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isChanged;
        }

        /// <summary>
        /// encrypts a string
        /// </summary>
        /// <param name="pwd">password</param>
        /// <returns>string</returns>
        private byte[] BuildBytePWD(string pwd)
        {
            return (Encoding.Unicode.GetBytes(String.Format("\"{0}\"", pwd)));
        }

        public bool estaAutenticado(string dominio, string usuario, string pwd, string path)
        {
            //Armamos la cadena completa de dominio y usuario
            string domainAndUsername = dominio + @"\" + usuario;
            //Creamos un objeto DirectoryEntry al cual le pasamos el URL, dominio/usuario y la contraseña
            DirectoryEntry entry = new DirectoryEntry(path, domainAndUsername, pwd);
            try
            {
                DirectorySearcher search = new DirectorySearcher(entry);
                //Verificamos que los datos de logeo proporcionados son correctos
                SearchResult result = search.FindOne();
                if (result == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
