using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public class GenerateRandomNumbers
    {
        /// <summary>
        /// method that generates random numbers 
        /// </summary>
        /// <returns>a random code</returns>
        public string GenerateNumber() {
            try
            {
                System.Random randomGenerate = new System.Random();
                System.String sPassword = "";
                sPassword = System.Convert.ToString(randomGenerate.Next(000001, 999999).ToString("D6"));
                //return sPassword.Substring(sPassword.Length - 6, 6);
                return sPassword;
            }
            catch (Exception)
            {

                System.Random randomGenerate = new System.Random();
                System.String sPassword = "";
                sPassword = System.Convert.ToString(randomGenerate.Next(000001, 999999).ToString("D6"));
                //return sPassword.Substring(sPassword.Length - 6, 6);
                return sPassword;
            }
           
        }
        
    }
}
