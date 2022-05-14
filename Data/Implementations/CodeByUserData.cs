using Helpers.Implementations;
using Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Implementations
{
    public class CodeByUserData
    {
        #region PROPIEDADES
        private string connection = Convert.ToString(ConfigurationManager.AppSettings["CON:ConnectionString"]);
        #endregion

        #region METODOS
        public CodeByUser GetCodeByUser(string cedula, string email)
        {
            CodeByUser codebyuser = new CodeByUser();
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
                        //new LogHelperData(e);
                        new ExceptionHelper(e, "GetCodeByUser (Problema de Conexion a la bd) ");
                    }

                    try
                    {
                        command.Connection = conn;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "[dbo].[GetCodeByUser]";
                        command.Parameters.AddWithValue("@cedula", cedula);
                        command.Parameters.AddWithValue("@email", email);

                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            codebyuser.id = (int)reader["id"];
                            codebyuser.cedula = (string)reader["cedula"];
                            codebyuser.email = (string)reader["email"];
                            codebyuser.codVerificacion = (string)reader["codVerificacion"];
                            codebyuser.fCaducidad = (DateTime)reader["fCaducidad"];
                            codebyuser.codBloqueado = (bool)reader["codBloqueado"];
                        }

                    }
                    catch (Exception e)
                    {

                        new ExceptionHelper(e, "GetCodeByUser (Problema de el sp) ");
                    }
                    finally
                    {
                        conn.Close();
                    }

                    return codebyuser;
                }
            }
        }

        /// <summary>
        /// insertion method
        /// </summary>
        /// <param name="codebyuser">CodeByUser</param>
        /// <returns>int</returns>
        public int InsertCodeByUser(CodeByUser codebyuser)
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
                        new ExceptionHelper(e, "InsertCodeByUser (Problema de Conexion a la bd) ");
                    }

                    try
                    {
                        command.Connection = conn;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "[dbo].[InsertCodeByUser]";
                        command.Parameters.AddWithValue("@cedula", codebyuser.cedula);
                        command.Parameters.AddWithValue("@email", codebyuser.email);
                        command.Parameters.AddWithValue("@codVerificacion", codebyuser.codVerificacion);
                        command.Parameters.AddWithValue("@fCaducidad", codebyuser.fCaducidad);
                        recordsAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        new ExceptionHelper(e, "InsertCodeByUser (Problema de el sp) ");

                    }
                    finally
                    {
                        conn.Close();
                    }


                    return recordsAffected;
                }
            }
        }

        public int UpdateFlagCodBloqueado(string cedula, string email, bool bloqueado)
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
                        new ExceptionHelper(e, "UpdateFlagCodBloqueado (Problema de Conexion a la bd) ");
                    }

                    try
                    {
                        command.Connection = conn;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "[dbo].[UpdateFlagCodBloqueado]";
                        command.Parameters.AddWithValue("@cedula", cedula);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@isBloqueado", bloqueado);

                        recordsAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {

                        new ExceptionHelper(e, "UpdateFlagCodBloqueado (Problema con el Sp) ");
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
