using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
   public class UserLocked
    {
        public int id { get; set; }
        public string cedula { get; set; }
        public string email { get; set; }

        public DateTime fDesbloqueo { get; set; }
    }
}
