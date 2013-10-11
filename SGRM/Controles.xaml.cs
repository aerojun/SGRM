using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO.Ports;

namespace SGRM
{
    /// <summary>
    /// Lógica de interacción para Controles.xaml
    /// </summary>
    public partial class Controles
    {

        SerialPort conexionArduino = new SerialPort();
        string ruta = ""; //Ruta de la imagen
        string[] estados = new string[] {"Aguascalientes",
                                        "Baja California",
                                        "Baja California Sur",
                                        "Campeche",
                                        "Chiapas",
                                        "Chihuahua",
                                        "Coahuila",
                                        "Colima",
                                        "Distrito Federal",
                                        "Durango",
                                        "Estado de México",
                                        "Guanajuato",
                                        "Guerrero",
                                        "Hidalgo",
                                        "Jalisco",
                                        "Michoacán",
                                        "Morelos",
                                        "Nayarit",
                                        "Nuevo León",
                                        "Oaxaca",
                                        "Puebla",
                                        "Querétaro",
                                        "Quintana Roo",
                                        "San Luis Potosí",
                                        "Sinaloa",
                                        "Sonora",
                                        "Tabasco",
                                        "Tamaulipas",
                                        "Tlaxcala",
                                        "Veracruz",
                                        "Yucatán",
                                        "Zacatecas"};
        string[] meses = new string[] { "Enero", "Febrero", "Marzo", "Abril",
                                        "Mayo", "Junio", "Julio", "Agosto",
                                        "Septiembre", "Octubre", "Noviembre", "Diciembre"};

        public Controles()
        {
            InitializeComponent();
            conexionArduino.BaudRate = 9600;
            conexionArduino.PortName = "COM3";

            foreach (string s in estados)
            {
                estadoCB.Items.Add(s);
                estado2CB.Items.Add(s);
            }
            foreach (string mes in meses)
                mesCB.Items.Add(mes);
        }   


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                conexionArduino.Open();
                Registro();
                //registroDB();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: "+ex.Message);
            }
        }

        private void Registro()
        {
            digitalizarB.IsEnabled = false;
            bool estado = false;
            conexionArduino.Write("1");

            while (conexionArduino.IsOpen)
            {
                string mensaje = conexionArduino.ReadLine();
                if ((mensaje == "Ponga su huella digital\r") ||
                    (mensaje == "Remueva el dedo\r") ||
                    (mensaje == "Volver a poner su huella\r") ||
                    (mensaje == "Huella grabada satiscatoriamente\r") ||
                    (mensaje == "Fallo la digitalizacion\r") ||
                    (mensaje == "Imposible capturar por tercera vez\r") ||
                    (mensaje == "Imposible capturar por segunda vez\r") ||
                    (mensaje == "Imposible capturar\r"))
                {
                    MessageBox.Show(mensaje);
                }

                /**
                 * ESTADOS DE LECTURA
                 * 0 = FALLO
                 * 1 = LECTURA CORRECTA
                 **/

                if (mensaje == "1\r")
                {
                    //MessageBox.Show("Huella Registrada!");
                    conexionArduino.Close();
                }
                else if (mensaje == "0\r")
                {
                    //MessageBox.Show("Falló la Digitalizacion!");
                    conexionArduino.Close();
                    estado = true;
                }

            }
            conexionArduino.Open();
            if (!estado)
            {
                idC.Text = conexionArduino.ReadLine();
                idC.Text = (int.Parse(idC.Text) + 1).ToString();

                digitalizarB.IsEnabled = false;
                agregarB.IsEnabled = true;
                fotoB.IsEnabled = true;
                nombreC.IsEnabled = true;
                paternoC.IsEnabled = true;
                maternoC.IsEnabled = true;
                generoCB.IsEnabled = true;
                ciudadC.IsEnabled = true;
                estadoCB.IsEnabled = true;
                diaCB.IsEnabled = true;
                mesCB.IsEnabled = true;
                añoCB.IsEnabled = true;
                sangreCB.IsEnabled = true;
                calleC.IsEnabled = true;
                numC.IsEnabled = true;
                colC.IsEnabled = true;
                ciudad2C.IsEnabled = true;
                estado2CB.IsEnabled = true;
                telCasaC.IsEnabled = true;
                telCelC.IsEnabled = true;
                refCasaC.IsEnabled = true;
                refCelC.IsEnabled = true;
                alergiasB.IsEnabled = true;
                enfermedadesB.IsEnabled = true;
                operacionesB.IsEnabled = true;

            }
        }

        private void registroDB()
        {
            ConexionDB nuevoPaciente = new ConexionDB();
            //nuevoPaciente.agregarPaciente();
        }

        private void fotoB_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog nuevaImagen = new Microsoft.Win32.OpenFileDialog();

            nuevaImagen.DefaultExt = "JPEG (*.jpg)|*.jpg";
            nuevaImagen.Filter = "JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|Bitmap (*.bmp)|*.bmp";

            Nullable<bool> result = nuevaImagen.ShowDialog();

            if (result == true)
            {
                BitmapImage myBitmapImage = new BitmapImage();
                ruta = nuevaImagen.FileName;
                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = new Uri(ruta);
                myBitmapImage.DecodePixelWidth = 126;
                myBitmapImage.EndInit();
                fotoCuadro.Source = myBitmapImage;
            }
            fotoB.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
