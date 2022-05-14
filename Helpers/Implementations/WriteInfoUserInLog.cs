using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Implementations
{
   public class WriteInfoUserInLog
    {

        /// <summary>
        /// write in a log the information of a user who performs a password change
        /// </summary>
        /// <param name="cedula">Nº id of User</param>
        /// <param name="email">email of User</param>
        /// <param name="ip">ip</param>
        public WriteInfoUserInLog(string cedula,string email,string ip)
        {
            string detail = "El Usuario con numero de identificacion ==> " + cedula + " y email ==> "+email +
           " en la Fecha  ==>  " + DateTime.Now + 
           " Realizo un cambio de Contraseña en un dispositivo con ip =>: " + ip + Environment.NewLine + 
            Environment.NewLine + "*************************************************************************************************************" + Environment.NewLine + Environment.NewLine;
            string logFilePath = Convert.ToString(ConfigurationManager.AppSettings["LOG:FilePathUser"]);
            //checks if the directory exists if it doesn't create it
            if (!Directory.Exists(logFilePath))
            {
                Directory.CreateDirectory(logFilePath);
            }

                try
                {
                    File.AppendAllText(logFilePath + "/User_"+cedula+".txt", detail);
                }
                catch
                {
                    //Don't do anything in order to retry adding the log
                }
            
        }
    }
}
