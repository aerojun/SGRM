using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace SGRM
{
    class ConexionDB
    {
        /*
         * CONEXION A LA BASE DE DATOS
         */
        static string conexion = "SERVER=localhost; DATABASE=hospital;"
            + "UID=development; PASSWORD=x7uj2jsp;"; //Datos del servidor

        MySqlConnection conexionn = new MySqlConnection(conexion); //Objeto de conexion

        public ConexionDB()
        {

        }

        public bool verificarUsuario(string usuario, string contrasena)
        {
            string query = "SELECT id FROM usuarios WHERE password = '" + contrasena + "' AND usuario = '" + usuario + "'";
            MySqlCommand enviarQuery = new MySqlCommand(query, conexionn);

            try
            {
                //Abrir conexion
                conexionn.Open();

                //Enviar query
                MySqlDataReader resultadoQuery = enviarQuery.ExecuteReader();

                if (resultadoQuery.HasRows)
                    return true;
                else
                    return false;

                //Finalizo correctamente
            }
            catch (MySqlException MySqlError)
            {
                return false;
            }
            finally
            {
                //Cerrar conexion
                conexionn.Close();
            }
        }

        public bool agregarAlumno(int id, string Nombre, string Apellido)
        {
            string query = "INSERT INTO Alumnos (id, Nombre, Apellidos) VALUES ('" + id + "','" + Nombre + "','" + Apellido + "')";
            MySqlCommand enviarQuery = new MySqlCommand(query, conexionn);

            try
            {
                //Abrir conexion
                conexionn.Open();

                //Enviar query
                enviarQuery.ExecuteNonQuery();

                //Finalizo correctamente
                return true;
            }
            catch (MySqlException MySqlError)
            {
                return false;
            }
            finally
            {
                //Cerrar conexion
                conexionn.Close();
            }
        }

        public bool eliminarAlumno()
        {
            return false;
        }
    }
}
