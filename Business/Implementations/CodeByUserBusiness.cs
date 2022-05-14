using Data.Implementations;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementations
{
    public class CodeByUserBusiness
    {
        #region PROPIEDADES
        CodeByUserData data = new CodeByUserData();
        #endregion

        #region METODOS
        public CodeByUser GetCodeByUser(string cedula, string email)
        {
            return data.GetCodeByUser(cedula, email);
        }

        public int InsertCodeByUser(CodeByUser codebyuser)
        {
            return data.InsertCodeByUser(codebyuser);
        }

        public int UpdateFlagCodBloqueado(string cedula, string email, bool bloqueado)
        {
            return data.UpdateFlagCodBloqueado(cedula, email, bloqueado);
        }
        #endregion
    }
}
