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
        static string conexion = "SERVER=localhost; DATABASE=SGRM;"
            + "UID=development; PASSWORD=x7uj2jsp;"; //Datos del servidor

        MySqlConnection conexionn = new MySqlConnection(conexion); //Objeto de conexion

        public ConexionDB()
        {

        }

        public bool verificarPaciente(string usuario, string contrasena)
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

        public bool agregarPaciente(int id, string nombre, string ap, string am, string nac, string reg, int gen, int sang, string foto, int huella, string calle, int numero, string colonia, string ciudad, string estado, int cp, int tel, int celular, string nRef, int tRef, string alergia, string oper, string enfer  )
        {
            int antPat = 0;
            int dat = 0;

            /*
             * Tabla de Antecedentes Patologicos
             */

            string query3 = "INSERT INTO antecedente_patologico (id, Alergias, Enfermedades, Operaciones) VALUES ('"+id+"', '"+alergia+"', '"+enfer+"', '"+oper+"')";

            MySqlCommand enviarQuery = new MySqlCommand(query3, conexionn);

            try
            {
                //Abrir conexion
                conexionn.Open();

                //Enviar query
                enviarQuery.ExecuteNonQuery();

                antPat = Convert.ToInt32(enviarQuery.LastInsertedId);

                /*
                * Tabla de Datos Adicionales
                */

                string query2 = "INSERT INTO datos_adicionales (Calle, Numero, Colonia, Ciudad, Estado, CP, Telefono, Celular, Nombre_Ref, Telefono_Ref)" +
                "VALUES ('" + calle + "' , '" + numero + "' , '" + colonia + "' , '" + ciudad + "' , '" + estado + "' , '" + cp + "' , '" + tel + "' , '" + celular + "' , '" + nRef + "' , '" + tRef + "')";
                
                enviarQuery = new MySqlCommand(query2, conexionn);

                enviarQuery.ExecuteNonQuery();

                dat = Convert.ToInt32(enviarQuery.LastInsertedId);

                /*
                * Tabla de Datos del Paciente
                */

                /*string query = "INSERT INTO paciente (id, Nombre, ApellidoP, ApellidoM, Fecha_Nacimiento, Fecha_Registro, id_Genero, id_Tipo_Sangre, id_Ant_Pat, id_Datos_Adicionales, Fotografia, Huella) VALUES ('"
                + id + "','" + nombre + "','" + ap + "','" + am + "','" + nac + "','" + reg + "','" + gen + "','" + sang + "','" + antPat + "','" + dat + "', ?Fotografia ,'" + huella + "')";

                query.Parameters.AddWithValue("?Fotografia", foto);

                enviarQuery = new MySqlCommand(query, conexionn);*/

                using (var query = conexionn.CreateCommand())
                {
                    query.CommandText = "INSERT INTO paciente (id, Nombre, ApellidoP, ApellidoM, Fecha_Nacimiento, Fecha_Registro, id_Genero, id_Tipo_Sangre, id_Ant_Pat, id_Datos_Adicionales, Fotografia, Huella) VALUES ('"
                + id + "','" + nombre + "','" + ap + "','" + am + "','" + nac + "','" + reg + "','" + gen + "','" + sang + "','" + antPat + "','" + dat + "', ?Fotografia ,'" + huella + "')";

                    query.Parameters.AddWithValue("?Fotografia", foto);

                    query.ExecuteNonQuery();
                }

                //Finalizo correctamente
                return true;
            }
            catch (MySqlException MySqlError)
            {
                System.Windows.MessageBox.Show("Error: " + MySqlError);
                return false;
            }
        }

        public bool eliminarAlumno()
        {
            return false;
        }
    }
}
