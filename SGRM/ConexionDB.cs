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

        public bool agregarPaciente(int id, string nombre, string ap, string am, string nac, string reg, int gen, int sang, string foto, int huella, string calle, int numero, string colonia, string ciudad, string estado, int cp, int tel, int celular, string nRef, int tRef, string alergia, string oper, string enfer, string cNacimiento, string eNacimiento, string edad  )
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

                using (var query = conexionn.CreateCommand())
                {
                    query.CommandText = "INSERT INTO paciente (id, Nombre, ApellidoP, ApellidoM, Fecha_Nacimiento, Fecha_Registro, id_Genero, id_Tipo_Sangre, id_Ant_Pat, id_Datos_Adicionales, Fotografia, Huella, ciudad_nacimiento, estado_nacimiento, edad) VALUES ('"
                + id + "','" + nombre + "','" + ap + "','" + am + "','" + nac + "','" + reg + "','" + gen + "','" + sang + "','" + antPat + "','" + dat + "', ?Fotografia ,'" + huella +"', '"+ cNacimiento+"','"+eNacimiento+"','"+edad+"')";

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

        public Tuple<Tuple<string, string, string, string, string, int, int, Tuple<string, string, string, string>>, Tuple<string, string, string, string, string, string, string, Tuple<string>>, Tuple<string, string>, Tuple<string, string, string>> busquedaPaciente(int id)
        {
            string query = "SELECT * FROM paciente WHERE Huella = '"+id+"'";
            int antecedentes = 0;
            int adicionales = 0;
            //----------------
            string id2 = ""; //Id del paciente
            string nombre = ""; //Nombre
            string ap = ""; //Apellido Paterno
            string am = ""; //Apellido Materno
            string nac = ""; //Fecha de Nacimiento
            int gen = 0; //Genero
            int sang = 0; //Tipo de Sangre
            string foto = ""; //Localización de la foto de perfil
            string cNacimiento = "";
            string eNacimiento = "";
            string edad = "";
            //---------------
            string calle = ""; //Calle
            string numero = ""; //Numero
            string colonia = ""; //Colonia
            string ciudad = ""; //Ciudad
            string estado = ""; //Estado
            string cp = ""; //CP
            string tel = ""; //Telefono
            string celular = ""; //Celular
            string nRef = ""; // Nombre de la Referencia
            string tRef = ""; //Telefono de la Referencia
            //----------------
            string alergia = "";
            string oper = "";
            string enfer = "";

            Tuple<string, string, string, string, string, int, int, Tuple<string, string, string, string>> datosPaciente;
            Tuple<string, string, string, string, string, string, string, Tuple<string>> datosAdicionalesPaciente;
            Tuple<string, string> datosAdicionalesPaciente2;
            Tuple<string, string, string> antecedentesPaciente;
            Tuple<Tuple<string, string, string, string, string, int, int, Tuple<string, string, string, string>>, Tuple<string, string, string, string, string, string, string, Tuple<string>>, Tuple<string, string>, Tuple<string, string, string>> data;

            MySqlCommand enviarQuery = new MySqlCommand(query, conexionn);

            try
            {
                //Abrir conexion
                conexionn.Open();

                //Enviar query
                MySqlDataReader reader = enviarQuery.ExecuteReader();

                while (reader.Read())
                {
                     id2 = reader.GetString("id");
                     nombre = reader.GetString("Nombre");
                     ap = reader.GetString("ApellidoP");
                     am = reader.GetString("ApellidoM");
                     nac = reader.GetString("Fecha_Nacimiento");
                     gen = Convert.ToInt32(reader.GetString("id_Genero"));
                     sang = Convert.ToInt32(reader.GetString("id_Tipo_Sangre"));
                     foto = reader.GetString("Fotografia");
                     antecedentes = Convert.ToInt32(reader.GetString("id_Ant_Pat"));
                     adicionales = Convert.ToInt32(reader.GetString("id_Datos_Adicionales"));
                     cNacimiento = reader.GetString("ciudad_nacimiento");
                     eNacimiento = reader.GetString("estado_nacimiento");
                     edad = reader.GetString("edad");
                }
                reader.Dispose();

                datosPaciente = new Tuple<string, string, string, string, string, int, int, Tuple<string, string, string, string>>(id2, nombre, ap, am, nac, gen, sang, Tuple.Create(foto, cNacimiento, eNacimiento, edad));
                //----------------------------------------------------------------------------
                query = "SELECT * FROM datos_adicionales WHERE id = '" + adicionales + "'";
                enviarQuery = new MySqlCommand(query, conexionn);
                reader = enviarQuery.ExecuteReader();

                while (reader.Read())
                {
                    calle = reader.GetString("Calle");
                    numero = reader.GetString("Numero");
                    colonia = reader.GetString("Colonia");
                    ciudad = reader.GetString("Ciudad");
                    estado = reader.GetString("Estado");
                    cp =reader.GetString("CP");
                    tel = reader.GetString("Telefono");
                    celular = reader.GetString("Celular");
                    nRef = reader.GetString("Nombre_Ref");
                    tRef = reader.GetString("Telefono_Ref");
                }
                datosAdicionalesPaciente = new Tuple<string, string, string, string, string, string, string, Tuple<string>>(calle, numero, colonia, ciudad, estado, cp, tel, Tuple.Create(celular));
                datosAdicionalesPaciente2 = new Tuple<string, string>(nRef, tRef);
                reader.Dispose();
                //----------------------------------------------------------------------------
                query = "SELECT * FROM antecedente_patologico WHERE id = '" + antecedentes + "'";
                enviarQuery = new MySqlCommand(query, conexionn);
                reader = enviarQuery.ExecuteReader();

                while (reader.Read())
                {
                    alergia = reader.GetString("Alergias");
                    oper = reader.GetString("Enfermedades");
                    enfer = reader.GetString("Operaciones");
                    
                }
                reader.Dispose();
                antecedentesPaciente = new Tuple<string, string, string>(alergia, oper, enfer);

                data = new Tuple<Tuple<string, string, string, string, string, int, int, Tuple<string, string, string, string>>, Tuple<string, string, string, string, string, string, string, Tuple<string>>, Tuple<string, string>, Tuple<string, string, string>>(datosPaciente, datosAdicionalesPaciente, datosAdicionalesPaciente2, antecedentesPaciente);
                return data;
            }
            catch (MySqlException error)
            {
                Tuple<string, string, string, string, string, int, int, Tuple<string, string, string, string>> datosPacientee = new Tuple<string, string, string, string, string, int, int, Tuple<string, string, string, string>>("", "", "", "", "", 0, 0, Tuple.Create("", "", "", ""));
                Tuple<string, string, string, string, string, string, string, Tuple<string>> datosAdicionalesPacientee = new Tuple<string, string, string, string, string, string, string, Tuple<string>>("", "", "", "", "", "", "", Tuple.Create(""));
                Tuple<string, string> datosAdicionalesPaciente22 = new Tuple<string, string>("","");
                Tuple<string, string, string> antecedentesPacientee = new Tuple<string, string, string>("","","");
                Tuple<Tuple<string, string, string, string, string, int, int, Tuple<string, string, string, string>>, Tuple<string, string, string, string, string, string, string, Tuple<string>>, Tuple<string, string>, Tuple<string, string, string>> dato;
                dato = new Tuple<Tuple<string, string, string, string, string, int, int, Tuple<string, string, string, string>>, Tuple<string, string, string, string, string, string, string, Tuple<string>>, Tuple<string, string>, Tuple<string, string, string>>(datosPacientee, datosAdicionalesPacientee, datosAdicionalesPaciente22, antecedentesPacientee);
                System.Windows.MessageBox.Show("Error: " + error);
                return dato;
            }
        }
    }
}
