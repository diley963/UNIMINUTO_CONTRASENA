using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CodeByUser
    {
        public int id { get; set; }

        public string cedula { get; set; }
        public string email { get; set; }
        public string codVerificacion { get; set; }
        public DateTime fCaducidad { get; set; }
        public bool codBloqueado { get; set; }
    }
}
