using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PasswordChange.Models
{
    public class User
    {
        public int id { get; set; }
        public string email { get; set; }
        public string nDocument { get; set; }
        public string mobile { get; set; }
        public string alternativeEmail { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string descripcion { get; set; }
    }

    //shipping methods for the code
    public enum MethodSendCode
    {
        mobile = 1,
        email =2,
        tengoCodigo=3
    }

    //store response codes in the SendMsmMasivApi method.
    public enum responseSendMasivApi
    {
        sms_send_success = 0, //indicates message sent successfully
        encript_decrypt_error = -1157,//indicates decrypt or encrypt error
        credential_error = 1157,//indicates credential error
        error_destination_number = 1, //indicates error in the destination number
        default_value = -1 // inidcates default value 
    }
}