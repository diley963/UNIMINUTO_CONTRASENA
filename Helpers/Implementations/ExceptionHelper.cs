using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Implementations
{
    public class ExceptionHelper
    {
        public ExceptionHelper(Exception e,string Metodo)
        {

            string detail = "Generado en el Metodo ==> "+Metodo +" en la Fecha  ==>  " + DateTime.Now + " Descripcion de Excepcion: " + e.Message + Environment.NewLine + "  El StackTrace es: " + e.StackTrace
                + Environment.NewLine + Environment.NewLine + "*************************************************************************************************************" + Environment.NewLine + Environment.NewLine;
            string logFilePath = Convert.ToString(ConfigurationManager.AppSettings["LOG:FilePath"]);
            //checks if the directory exists if it doesn't create it
            if (!Directory.Exists(logFilePath))
            {
                Directory.CreateDirectory(logFilePath);
            }
            while (e != null)
            {
                try
                {
                    File.AppendAllText(logFilePath + "/Log.txt", detail);
                    e = e.InnerException;
                }
                catch
                {
                    //Don't do anything in order to retry adding the log
                }
            }

        }
    }
}
