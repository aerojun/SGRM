using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace SGRM2
{
    class Conexion
    {
        //Datos del servidor
        static string datosConexion = "SERVER=localhost; DATABASE=SGRM; UID=development; PASSWORD=none;";
        
        public bool iniciarSesion(string usuario, string password)
        {
            MySqlConnection conexion = new MySqlConnection(datosConexion);

            try
            {
                conexion.Open();

                try
                {
                    string query = "SELECT * FROM usuarios WHERE password = @password AND usuario = @usuario";
                    
                    MySqlCommand enviarQuery = new MySqlCommand(query, conexion);

                    enviarQuery.Parameters.AddWithValue("@usuario", usuario);

                    enviarQuery.Parameters.AddWithValue("@password", password);
                    
                    MySqlDataReader resultadoQuery = enviarQuery.ExecuteReader();

                    if (resultadoQuery.HasRows)
                        return true;
                    else
                        return false;
                }
                catch(MySqlException error)
                {
                    System.Windows.MessageBox.Show(error.ToString());
                    return false;
                }
            }
            catch (MySqlException error)
            {
                System.Windows.MessageBox.Show(error.ToString());
                return false;
            }
            finally
            {
                conexion.Close();
            }
        }

        public bool nuevoPaciente(string[] informacionPaciente)
        {
            MySqlConnection conexion = new MySqlConnection(datosConexion);
            try
            {
                conexion.Open();

                try
                {
                    string insertarDatos = 
                    "INSERT INTO datos_adicionales (Calle, Numero, Colonia, Ciudad, Estado, CP, Telefono, Celular, Nombre_Ref, Telefono_Ref)"
                    +"VALUES ()";

                    //string query = "INSERT INTO paciente(Nombre, ApellidoP, ApellidoM, Fecha_Nacimiento, id_Genero, id_Tipo_Sangre, id_Ant_Pat, id_Datos_Adicionales, Fotografia, Huella, ciudad_nacimiento, estado_nacimiento, edad) VALUES ()";

                    MySqlCommand enviarQuery = new MySqlCommand(insertarDatos, conexion);

                    enviarQuery.ExecuteNonQuery();

                    int idDatos = Convert.ToInt32(enviarQuery.LastInsertedId);

                    return true;
                }
                catch (MySqlException error)
                {
                    System.Windows.MessageBox.Show(error.ToString());
                    return false;
                }
            }
            catch (MySqlException error)
            {
                System.Windows.MessageBox.Show(error.ToString());
                return false;
            }
            
        }

        public string buscarPaciente(int idPaciente)
        {
            MySqlConnection conexion = new MySqlConnection(datosConexion);

            try
            {
                string datosPaciente = "";

                conexion.Open();

                try
                {
                    string query = "SELECT * FROM paciente WHERE Huella = @idHuella";

                    MySqlCommand enviarQuery = new MySqlCommand(query, conexion);

                    enviarQuery.Parameters.AddWithValue("@idHuella", idPaciente);

                    MySqlDataReader resultadoQuery = enviarQuery.ExecuteReader();

                    while (resultadoQuery.Read())
                    {
                        datosPaciente += Convert.ToString(resultadoQuery.GetString(0)) + ";"; //ID
                        datosPaciente += Convert.ToString(resultadoQuery.GetString(1)) + ";"; //Nombre
                        datosPaciente += Convert.ToString(resultadoQuery.GetString(2)) + ";"; //Apellido Paterno
                        datosPaciente += Convert.ToString(resultadoQuery.GetString(3)) + ";"; //Apellido Materno
                        datosPaciente += Convert.ToString(resultadoQuery.GetString(4)) + ";"; //Fecha Nacimiento
                        datosPaciente += Convert.ToString(resultadoQuery.GetString(5)) + ";"; //Fecha Registro
                        datosPaciente += Convert.ToString(resultadoQuery.GetString(6)) + ";"; //ID Genero
                        datosPaciente += Convert.ToString(resultadoQuery.GetString(7)) + ";"; //ID Tipo Sangre
                        datosPaciente += Convert.ToString(resultadoQuery.GetString(8)) + ";"; //ID Antecedentes
                        datosPaciente += Convert.ToString(resultadoQuery.GetString(9)) + ";"; //ID Datos
                        datosPaciente += Convert.ToString(resultadoQuery.GetString(10)) + ";"; //Foto
                        
                        datosPaciente += Convert.ToString(resultadoQuery.GetString(12)) + ";"; //Ciudad Nacimiento
                        datosPaciente += Convert.ToString(resultadoQuery.GetString(13)) + ";"; //Estado Nacimiento
                        datosPaciente += Convert.ToString(resultadoQuery.GetString(14)) + ";"; //Edad
                    }

                    return datosPaciente;
                }
                catch (MySqlException error)
                {
                    System.Windows.MessageBox.Show(error.ToString());
                    return null;
                }
            }
            catch (MySqlException error)
            {
                System.Windows.MessageBox.Show(error.ToString());
                return null;
            }
            finally
            {
                conexion.Close();
            }
        }

        public string buscarAntecedentes(int idAntecedente)
        {
            MySqlConnection conexion = new MySqlConnection(datosConexion);

            try
            {
                string antecedentePaciente = "";

                conexion.Open();

                try
                {
                    string query = "SELECT * FROM antecedente_patologico WHERE id = @idAntecedente";

                    MySqlCommand enviarQuery = new MySqlCommand(query, conexion);

                    enviarQuery.Parameters.AddWithValue("@idAntecedente", idAntecedente);

                    MySqlDataReader resultadoQuery = enviarQuery.ExecuteReader();

                    while (resultadoQuery.Read())
                    {
                        if (Convert.ToString(resultadoQuery.GetString(1)) == "")
                        {
                            antecedentePaciente += "n" + ";"; 
                        }
                        else
                        {
                            antecedentePaciente += Convert.ToString(resultadoQuery.GetString(1)) + ";"; //Alergias
                        }

                        if (Convert.ToString(resultadoQuery.GetString(2)) == "")
                        {
                            antecedentePaciente += "n" + ";"; 
                        }
                        else
                        {
                            antecedentePaciente += Convert.ToString(resultadoQuery.GetString(2)) + ";"; //Enfermedades
                        }

                        if (Convert.ToString(resultadoQuery.GetString(3)) == "")
                        {
                            antecedentePaciente += "n" + ";"; 
                        }
                        else
                        {
                            antecedentePaciente += Convert.ToString(resultadoQuery.GetString(3)) + ";"; //Operaciones
                        }                    
                    }

                    return antecedentePaciente;
                }
                catch (MySqlException error)
                {
                    System.Windows.MessageBox.Show(error.ToString());
                    return null;
                }
            }
            catch (MySqlException error)
            {
                System.Windows.MessageBox.Show(error.ToString());
                return null;
            }
            finally
            {
                conexion.Close();
            }
        }

        public string buscarAdicionales(int idAdicionales)
        {
            MySqlConnection conexion = new MySqlConnection(datosConexion);

            try
            {
                string adicionalesPaciente = "";

                conexion.Open();

                try
                {
                    string query = "SELECT * FROM datos_adicionales WHERE id = @idAdicionales";

                    MySqlCommand enviarQuery = new MySqlCommand(query, conexion);

                    enviarQuery.Parameters.AddWithValue("@idAdicionales", idAdicionales);

                    MySqlDataReader resultadoQuery = enviarQuery.ExecuteReader();

                    while (resultadoQuery.Read())
                    {
                        adicionalesPaciente += Convert.ToString(resultadoQuery.GetString(1)) + ";"; //Calle
                        adicionalesPaciente += Convert.ToString(resultadoQuery.GetString(2)) + ";"; //Numero
                        adicionalesPaciente += Convert.ToString(resultadoQuery.GetString(3)) + ";"; //Colonia
                        adicionalesPaciente += Convert.ToString(resultadoQuery.GetString(4)) + ";"; //Ciudad
                        adicionalesPaciente += Convert.ToString(resultadoQuery.GetString(5)) + ";"; //Estado
                        adicionalesPaciente += Convert.ToString(resultadoQuery.GetString(6)) + ";"; //CP
                        adicionalesPaciente += Convert.ToString(resultadoQuery.GetString(7)) + ";"; //Telefono
                        adicionalesPaciente += Convert.ToString(resultadoQuery.GetString(8)) + ";"; //Celular
                        adicionalesPaciente += Convert.ToString(resultadoQuery.GetString(9)) + ";"; //Nombre Referencia
                        adicionalesPaciente += Convert.ToString(resultadoQuery.GetString(10)) + ";"; //Telefono Referencia
                    }

                    return adicionalesPaciente;
                }
                catch (MySqlException error)
                {
                    System.Windows.MessageBox.Show(error.ToString());
                    return null;
                }
            }
            catch (MySqlException error)
            {
                System.Windows.MessageBox.Show(error.ToString());
                return null;
            }
            finally
            {
                conexion.Close();
            }
        }
    }
}
