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

        string[] sangre = new string[] {"A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };


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

            foreach (string san in sangre)
                sangreCB.Items.Add(san);

            for (int y = 1920; y <= 2013; y++)
                añoCB.Items.Add(y);
        }

        private void numerosOnly(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true;
                    break;
                }
            }
        }

        private void cambioMes(object sender, SelectionChangedEventArgs e)
        {
            diaCB.Items.Clear();
            if ((mesCB.SelectedIndex == 0) || (mesCB.SelectedIndex == 2) || (mesCB.SelectedIndex == 4)
                || (mesCB.SelectedIndex == 6) || (mesCB.SelectedIndex == 7) || (mesCB.SelectedIndex == 9) || (mesCB.SelectedIndex == 11))
            {
                for (int m = 1; m <= 31; m++)
                    diaCB.Items.Add(m);
            }
            else if ((mesCB.SelectedIndex == 3) || (mesCB.SelectedIndex == 5) || (mesCB.SelectedIndex == 10) || (mesCB.SelectedIndex == 8))
            {
                for (int m = 1; m <= 30; m++)
                    diaCB.Items.Add(m);
            }
            else
            {
                for (int m = 1; m <= 29; m++)
                    diaCB.Items.Add(m);
            }
            diaCB.SelectedIndex = 0;
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

        private void agregarB_Click(object sender, RoutedEventArgs e)
        {
            string xxx = "";
            foreach (object alergiaItem in listaAler.Items)
            {
                xxx += alergiaItem.ToString() + ",";
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
                    digitalizarB.IsEnabled = true;
                }

            }
            conexionArduino.Open();
            if (!estado)
            {
                idC.Text = conexionArduino.ReadLine();
                idC.Text = (int.Parse(idC.Text) + 1).ToString();

                digitalizarB.IsEnabled = false;
                agregarB.IsEnabled = true;
                edadC.IsEnabled = true;
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
                agregAler.IsEnabled = true;
                agregarEnfermedad.IsEnabled = true;
                agregarOperacion.IsEnabled = true;

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

        private void alergiasB_Click(object sender, RoutedEventArgs e)
        {
            string ninguna = listaAler.Items.GetItemAt(0).ToString();

            if (ninguna == "System.Windows.Controls.ListBoxItem: Ninguna")
            {
                listaAler.Items.Clear();
                listaAler.IsEnabled = true;
            }
            
            listaAler.Items.Add(agregAler.Text);
            agregAler.Clear();
        }

        private void enfermedadesB_Click(object sender, RoutedEventArgs e)
        {
            string ninguna = listaEnfer.Items.GetItemAt(0).ToString();

            if (ninguna == "System.Windows.Controls.ListBoxItem: Ninguna")
            {
                listaEnfer.Items.Clear();
                listaEnfer.IsEnabled = true;
            }

            listaEnfer.Items.Add(agregarEnfermedad.Text);
            agregarEnfermedad.Clear();
        }

        private void operacionesB_Click(object sender, RoutedEventArgs e)
        {
            string ninguna = listaOper.Items.GetItemAt(0).ToString();

            if (ninguna == "System.Windows.Controls.ListBoxItem: Ninguna")
            {
                listaOper.Items.Clear();
                listaOper.IsEnabled = true;
            }

            listaOper.Items.Add(agregarOperacion.Text);
            agregarOperacion.Clear();
        }
    }
}
