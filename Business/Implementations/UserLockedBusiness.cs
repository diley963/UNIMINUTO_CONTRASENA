using Data.Implementations;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementations
{
    public class UserLockedBusiness
    {
        #region PROPIEDADES
        UserLockedData data = new UserLockedData();
        #endregion

        #region METODOS

        /// <summary>
        /// method that returns a temporarily blocked user 
        /// </summary>
        /// <param name="cedula">Nº of cedula</param>
        /// <param name="email">email</param>
        /// <returns></returns>
        public UserLocked GetUserBlocked(string cedula, string email)
        {
            return data.GetUserBlocked(cedula,email);
        }

        /// <summary>
        /// method calling insertion sp 
        /// </summary>
        /// <param name="userlocked">UserLocked</param>
        /// <returns>int </returns>
        public int InsertBlockedUser(UserLocked userlocked){
            return data.InsertBlockedUser(userlocked);
        }
        #endregion
    }
}
