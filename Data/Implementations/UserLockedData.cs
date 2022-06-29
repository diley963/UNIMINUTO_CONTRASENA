using Helpers.Implementations;
using Model;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace Data.Implementations
{
    public class UserLockedData
    {
        #region PROPIEDADES
        private string connection = Convert.ToString(ConfigurationManager.AppSettings["CON:ConnectionString"]);
        #endregion

        #region METODOS
        public UserLocked GetUserBlocked(string cedula, string email)
        {
            UserLocked userlocked = new UserLocked();
            using (SqlConnection conn = new SqlConnection(connection))
            {

                using (SqlCommand command = new SqlCommand())
                {
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception e)
                    {
                        new ExceptionHelper(e, "GetUserBlocked (Problema de Conexion a la bd) ");
                    }

                    try
                    {
                        command.Connection = conn;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "[dbo].[GetUserBlocked]";
                        command.Parameters.AddWithValue("@cedula", cedula);
                        command.Parameters.AddWithValue("@email", email);

                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            userlocked.id = (int)reader["id"];
                            userlocked.cedula = (string)reader["cedula"];
                            userlocked.email = (string)reader["email"];
                            userlocked.fDesbloqueo = (DateTime)reader["fDesbloqueo"];
                        }
                    }
                    catch (Exception e)
                    {

                        new ExceptionHelper(e, "GetUserBlocked (Problema Con el Sp) ");
                    }
                    finally
                    {
                        conn.Close();
                    }

                    return userlocked;
                }
            }
        }

        /// <summary>
        /// insertion method
        /// </summary>
        /// <param name="userlocked">UserLocked</param>
        /// <returns>int</returns>
        public int InsertBlockedUser(UserLocked userlocked)
        {
            using (SqlConnection conn = new SqlConnection(this.connection))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    int recordsAffected = 0;
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception e)
                    {
                        new ExceptionHelper(e, "InsertBlockedUser (Problema de Conexion a la bd) ");
                    }

                    try
                    {
                        command.Connection = conn;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "[dbo].[InsertBlockedUser]";
                        command.Parameters.AddWithValue("@cedula", userlocked.cedula);
                        command.Parameters.AddWithValue("@email", userlocked.email);
                        command.Parameters.AddWithValue("@fDesbloqueo", userlocked.fDesbloqueo);

                        recordsAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {

                        new ExceptionHelper(e, "InsertBlockedUser (Problema con el SP) ");
                    }
                    finally
                    {
                        conn.Close();
                    }


                    return recordsAffected;
                }
            }
        }
        #endregion

     
    }
}
