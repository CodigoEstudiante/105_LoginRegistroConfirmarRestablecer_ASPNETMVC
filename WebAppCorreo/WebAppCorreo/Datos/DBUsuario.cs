using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using WebAppCorreo.Models;
using System.Data;
using System.Data.SqlClient;

namespace WebAppCorreo.Datos
{
    public class DBUsuario
    {
        private static string CadenaSQL = "Server=(local); DataBase=DBPrueba; Trusted_Connection=True; TrustServerCertificate=True;";

        public static bool Registrar(UsuarioDTO usuario)
        {
            bool respuesta = false;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(CadenaSQL))
                {
                    string query = "insert into Usuario(Nombre,Correo,Clave,Restablecer,Confirmado,Token)";
                    query += " values(@nombre,@correo,@clave,@restablecer,@confirmado,@token)";

                    SqlCommand cmd = new SqlCommand(query,oconexion);
                    cmd.Parameters.AddWithValue("@nombre",usuario.Nombre);
                    cmd.Parameters.AddWithValue("@correo", usuario.Correo);
                    cmd.Parameters.AddWithValue("@clave", usuario.Clave);
                    cmd.Parameters.AddWithValue("@restablecer", usuario.Restablecer);
                    cmd.Parameters.AddWithValue("@confirmado", usuario.Confirmado);
                    cmd.Parameters.AddWithValue("@token", usuario.Token);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    int filasAfectadas = cmd.ExecuteNonQuery();
                    if(filasAfectadas >0)  respuesta = true;
                }

                return respuesta;
            }catch (Exception ex)
            {

                throw ex;
            }
        }

        public static UsuarioDTO Validar(string correo, string clave)
        {
            UsuarioDTO usuario = null;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(CadenaSQL))
                {
                    string query = "select Nombre,Restablecer,Confirmado from Usuario";
                    query += " where Correo=@correo and Clave = @clave";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.Parameters.AddWithValue("@clave", clave);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            usuario = new UsuarioDTO()
                            {
                                Nombre = dr["Nombre"].ToString(),
                                Restablecer = (bool)dr["Restablecer"],
                                Confirmado = (bool)dr["Confirmado"]

                            };
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return usuario;
        }


        public static UsuarioDTO Obtener(string correo)
        {
            UsuarioDTO usuario = null;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(CadenaSQL))
                {
                    string query = "select Nombre,Clave,Restablecer,Confirmado,Token from Usuario";
                    query += " where Correo=@correo";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            usuario = new UsuarioDTO()
                            {
                                Nombre = dr["Nombre"].ToString(),
                                Clave = dr["Clave"].ToString(),
                                Restablecer = (bool)dr["Restablecer"],
                                Confirmado = (bool)dr["Confirmado"],
                                Token = dr["Token"].ToString()

                            };
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return usuario;
        }


        public static bool RestablecerActualizar(int restablecer,string clave,string token)
        {
            bool respuesta = false;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(CadenaSQL))
                {
                    string query = @"update Usuario set " +
                        "Restablecer= @restablecer, " +
                        "Clave=@clave " +
                        "where Token=@token";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@restablecer", restablecer);
                    cmd.Parameters.AddWithValue("@clave", clave);
                    cmd.Parameters.AddWithValue("@token",token);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    int filasAfectadas = cmd.ExecuteNonQuery();
                    if (filasAfectadas > 0) respuesta = true;
                }

                return respuesta;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static bool Confirmar(string token)
        {
            bool respuesta = false;
            try
            {
                using (SqlConnection oconexion = new SqlConnection(CadenaSQL))
                {
                    string query = @"update Usuario set " +
                        "Confirmado= 1 " +
                        "where Token=@token";

                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@token", token);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    int filasAfectadas = cmd.ExecuteNonQuery();
                    if (filasAfectadas > 0) respuesta = true;
                }

                return respuesta;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}