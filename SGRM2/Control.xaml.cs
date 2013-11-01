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
using System.ComponentModel;
using System.Threading;
using System.IO.Ports;
using System.Management;

namespace SGRM2
{
    /// <summary>
    /// Lógica de interacción para Control.xaml
    /// </summary>
    public partial class Control
    {
        string ruta = "";
        string[] estados = new string[] {"Aguascalientes", "Baja California", "Baja California Sur", "Campeche",
                                        "Chiapas", "Chihuahua", "Coahuila", "Colima", "Distrito Federal",
                                        "Durango", "Estado de México", "Guanajuato", "Guerrero", "Hidalgo",
                                        "Jalisco", "Michoacán", "Morelos", "Nayarit", "Nuevo León", "Oaxaca",
                                        "Puebla", "Querétaro", "Quintana Roo", "San Luis Potosí", "Sinaloa",
                                        "Sonora", "Tabasco", "Tamaulipas", "Tlaxcala", "Veracruz", "Yucatán", "Zacatecas"};
        
        string[] meses = new string[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto",
                                        "Septiembre", "Octubre", "Noviembre", "Diciembre"};

        string[] sangre = new string[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };

            //-------ARDUINO--------
                SerialPort conexionArduino = new SerialPort();
            //---------------

        public Control()
        {
            //-----------------------------------METRO----------------------------------------------
            Uri NewTheme1 = new Uri(@"/MahApps.Metro;component/Styles/Colours.xaml", UriKind.Relative);
            Uri NewTheme = new Uri(@"/MahApps.Metro;component/Styles/Accents/Blue.xaml", UriKind.Relative);
            ResourceDictionary dictionary = (ResourceDictionary)Application.LoadComponent(NewTheme);
            Application.Current.Resources.MergedDictionaries.Add(dictionary);
            ResourceDictionary dictionary1 = (ResourceDictionary)Application.LoadComponent(NewTheme1);
            Application.Current.Resources.MergedDictionaries.Add(dictionary1);
            //-------------------------------------------------------------------------------------

            //-------ARDUINO--------
            conexionArduino.BaudRate = 9600;
            string nombrePuerto = "";
            MessageBoxButton botones = MessageBoxButton.OKCancel;

            nombrePuerto = puertoArduino();

            while (nombrePuerto == null)
            {
                MessageBox.Show("Dispositivo no Enlazado", "Error", botones);
                nombrePuerto = puertoArduino();
            }
            

            conexionArduino.PortName = nombrePuerto;
            //----------------------

            InitializeComponent();

            limpiar();
        }

        private string puertoArduino()
        {
            ManagementScope connectionScope = new ManagementScope();
            SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);

            try
            {
                foreach (ManagementObject item in searcher.Get())
                {
                    string desc = item["Description"].ToString();
                    string deviceId = item["DeviceID"].ToString();

                    if (desc.Contains("Arduino"))
                    {
                        return deviceId;
                    }
                }
            }
            catch (ManagementException e)
            {
                MessageBox.Show(e.Message.ToString());
            }

            return null;
        }

        private void cambioMes(object sender, SelectionChangedEventArgs e)
        {

        }

        private void fotoB_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog nuevaImagen = new Microsoft.Win32.OpenFileDialog(); //Objeto manejador

            nuevaImagen.DefaultExt = "JPEG (*.jpg)|*.jpg"; //Extension default de imagen
            nuevaImagen.Filter = "JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|Bitmap (*.bmp)|*.bmp"; //Posibles extensiones

            Nullable<bool> result = nuevaImagen.ShowDialog(); //Resultado

            if (result == true) //Si fue elegida una imagen ...
            {
                BitmapImage myBitmapImage = new BitmapImage(); //Crear bitmap
                ruta = nuevaImagen.FileName; //Ruta original
                myBitmapImage.BeginInit(); //Inicia
                myBitmapImage.UriSource = new Uri(ruta); //Origen de la imagen
                myBitmapImage.DecodePixelWidth = 126;
                myBitmapImage.EndInit(); //Fin
                fotoCuadro.Source = myBitmapImage; //Cuadro de la aplicacion muestra la imagen
                
            }
            fotoB.Visibility = System.Windows.Visibility.Hidden; //Esconde el boton de la foto
        }

        private void alergiasB_Click(object sender, RoutedEventArgs e)
        {
            if (listaAler.Items.GetItemAt(0).ToString().Contains("Ninguna"))
            {
                listaAler.Items.Clear();
            }
            listaAler.Items.Add("Nigga");
        }

        private void enfermedadesB_Click(object sender, RoutedEventArgs e)
        {

        }

        private void operacionesB_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buscarWait(object sender, RoutedEventArgs e)
        {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += delegate
                {
                    Dispatcher.BeginInvoke(new Action(delegate { progresoBusqueda.Visibility = System.Windows.Visibility.Visible; }));
                    Thread.Sleep(1000);
                    //Dispatcher.BeginInvoke(new Action(delegate { buscarPaciente(sender, e); }));
                    Dispatcher.BeginInvoke(new Action(delegate { huellaArduino(sender, e); }));
                };
                worker.RunWorkerAsync();
        }

        private void huellaArduino(object sender, RoutedEventArgs e)
        {
            try
            {
                conexionArduino.Open();

                conexionArduino.Write("0");
                MessageBox.Show("Ponga la huella del paciente");
                while (conexionArduino.IsOpen)
                {
                    string idd = conexionArduino.ReadLine();
                    int id = Convert.ToInt32(idd);
                    if (id != 200)
                    {
                        conexionArduino.Close();
                        buscarPaciente(sender, e, id);
                        nPaciente.Focus();
                    }
                    else
                    {
                        conexionArduino.Close();
                        MessageBox.Show("Paciente no encontrado!");
                        nPaciente.Focus();
                        return;
                    }
                }
            }
            catch (System.IO.IOException error)
            {
                MessageBox.Show(error.ToString());
            }

        }

        private void buscarPaciente(object sender, RoutedEventArgs e, int id)
        {
            limpiar();
            
            Conexion busqueda = new Conexion();
            string datosPaciente = busqueda.buscarPaciente(id);

            if (datosPaciente != null)
            {
                char[] delimitador = new char[] { ';' };
                char[] delimitadorFecha = new char[] { '/', ' ' };

                string[] datosPacienteLimitados = datosPaciente.Split(delimitador, StringSplitOptions.RemoveEmptyEntries);

                string[] fechaNacimiento = datosPacienteLimitados[4].Split(delimitadorFecha, StringSplitOptions.RemoveEmptyEntries);

                idC.Text = datosPacienteLimitados[0];
                nombreC.Text = datosPacienteLimitados[1];
                paternoC.Text = datosPacienteLimitados[2];
                maternoC.Text = datosPacienteLimitados[3];
                ciudadC.Text = datosPacienteLimitados[11];
                edadC.Text = datosPacienteLimitados[13];

                generoCB.SelectedIndex = Convert.ToInt32(datosPacienteLimitados[6]) - 1;
                sangreCB.SelectedIndex = Convert.ToInt32(datosPacienteLimitados[7]) - 1;
                
                estadoCB.SelectedIndex = estadoCB.Items.IndexOf(datosPacienteLimitados[12]);

                diaCB.SelectedIndex = diaCB.Items.IndexOf(Convert.ToInt32(fechaNacimiento[0]));
                mesCB.SelectedIndex = (Convert.ToInt32(fechaNacimiento[1]))-1;
                añoCB.SelectedIndex = añoCB.Items.IndexOf(Convert.ToInt32(fechaNacimiento[2]));

                string antecedentesPaciente = busqueda.buscarAntecedentes(Convert.ToInt32(datosPacienteLimitados[8]));
                string adicionalesPaciente = busqueda.buscarAdicionales(Convert.ToInt32(datosPacienteLimitados[9]));

                string[] antecedentesPacienteLimitados = antecedentesPaciente.Split(delimitador, StringSplitOptions.RemoveEmptyEntries);
                string[] adicionalesPacienteLimitados = adicionalesPaciente.Split(delimitador, StringSplitOptions.RemoveEmptyEntries);

                fotoB.Visibility = System.Windows.Visibility.Hidden;

                //----------------------------IMAGEN --------------------------------------------
                BitmapImage myBitmapImage = new BitmapImage(); //Crear bitmap
                myBitmapImage.BeginInit(); //Inicia
                myBitmapImage.UriSource = new Uri(datosPacienteLimitados[10]); //Origen de la imagen
                myBitmapImage.DecodePixelWidth = 126;
                myBitmapImage.EndInit(); //Fin
                fotoCuadro.Source = myBitmapImage; //Cuadro de la aplicacion muestra la imagen
                //---------------------------------------------------------------------------

                calleC.Text = adicionalesPacienteLimitados[0];
                numC.Text = adicionalesPacienteLimitados[1];
                colC.Text = adicionalesPacienteLimitados[2];
                ciudad2C.Text = adicionalesPacienteLimitados[3];

                estado2CB.SelectedIndex = estado2CB.Items.IndexOf(adicionalesPacienteLimitados[4]);

                cpC.Text = adicionalesPacienteLimitados[5];
                telCasaC.Text = adicionalesPacienteLimitados[6];
                telCelC.Text = adicionalesPacienteLimitados[7];
                refCasaC.Text = adicionalesPacienteLimitados[8];
                refCelC.Text = adicionalesPacienteLimitados[9];

                string[] alergias = antecedentesPacienteLimitados[0].Split(delimitadorFecha, StringSplitOptions.RemoveEmptyEntries);
                string[] enfermedades = antecedentesPacienteLimitados[1].Split(delimitadorFecha, StringSplitOptions.RemoveEmptyEntries);
                string[] operaciones = antecedentesPacienteLimitados[2].Split(delimitadorFecha, StringSplitOptions.RemoveEmptyEntries);

                listaAler.Items.Clear();
                listaOper.Items.Clear();
                listaEnfer.Items.Clear();

                foreach (string alergia in alergias)
                {
                    if (alergia != "n")
                        listaAler.Items.Add(alergia);
                    else
                        listaAler.Items.Add("Ninguna");
                }
                foreach (string enfermedad in enfermedades)
                {
                    if (enfermedad != "n")
                        listaEnfer.Items.Add(enfermedad);
                    else
                        listaEnfer.Items.Add("Ninguna");
                }
                foreach (string operacion in operaciones)
                {
                    if (operacion != "n")
                        listaOper.Items.Add(operacion);
                    else
                        listaOper.Items.Add("Ninguna");
                }

                if ((listaAler.Items.Count > 0) && (listaAler.Items.GetItemAt(0).ToString() != "Ninguna"))
                {
                    Uri NewTheme = new Uri(@"/MahApps.Metro;component/Styles/Accents/Red.xaml", UriKind.Relative);
                    ResourceDictionary dictionary = (ResourceDictionary)Application.LoadComponent(NewTheme);
                    Application.Current.Resources.MergedDictionaries.Add(dictionary);
                }
                else
                {
                    Uri NewTheme = new Uri(@"/MahApps.Metro;component/Styles/Accents/Blue.xaml", UriKind.Relative);
                    ResourceDictionary dictionary = (ResourceDictionary)Application.LoadComponent(NewTheme);
                    Application.Current.Resources.MergedDictionaries.Add(dictionary);
                }
            }
            else
                MessageBox.Show("Paciente No Encontrado!");
            
        }

        private void numerosOnly(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text) //Verifica cada caracter
            {
                if (!char.IsDigit(c)) //Si es diferente de numerico
                {
                    e.Handled = true; //Evita el ingreso del caracter
                    break;
                }
            }
        }

        private void habilitar(object sender, RoutedEventArgs e)
        {
            var textboxes = grid1.Children.OfType<TextBox>();
            var button = grid1.Children.OfType<Button>();
            var comboboxes = grid1.Children.OfType<ComboBox>();

            foreach (var textBox in textboxes)
                textBox.IsEnabled = true;
            foreach (var comboBox in comboboxes)
                comboBox.IsEnabled = true;
            foreach (var boton in button)
                boton.IsEnabled = true;

            digitalizarB.IsEnabled = false;
        }
        
        private void limpiar()
        {
            var textboxes = grid1.Children.OfType<TextBox>();
            var listboxes = grid1.Children.OfType<ListBox>();
            var comboboxes = grid1.Children.OfType<ComboBox>();

            foreach (var textBox in textboxes)
                textBox.Clear();
            foreach (var listBox in listboxes)
                listBox.Items.Clear();
            foreach (var comboBox in comboboxes)
                comboBox.Items.Clear();

            foreach (string estado in estados)
            {
                estadoCB.Items.Add(estado);
                estado2CB.Items.Add(estado);
            }

            foreach (string mes in meses)
                mesCB.Items.Add(mes);

            foreach (string tipo in sangre)
                sangreCB.Items.Add(tipo);

            for (int m = 1; m <= 31; m++) //31 dias
                diaCB.Items.Add(m);

            DateTime dateTime1 = DateTime.UtcNow.Date; //Fecha actual
            for (int y = 1920; y <= Convert.ToInt32(dateTime1.ToString("yyyy")); y++)
                añoCB.Items.Add(y);

            listaAler.Items.Add("Ninguna");
            listaEnfer.Items.Add("Ninguna");
            listaOper.Items.Add("Ninguna");
        }

        private void digitalizarB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                conexionArduino.Open();

                conexionArduino.Write("1");

                //Dehabilita el boton que de registro
                digitalizarB.IsEnabled = false;
                //Estado actual
                bool estado = false;
                //Envia un 1 por el serial
                conexionArduino.Write("1");

                //Lee los mensajes del arduino
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
                        MessageBox.Show(mensaje); //Muestra el mensaje si es uno de los anteriores
                    }

                    if (mensaje == "1\r") //Terminó todo correctamente
                    {
                        conexionArduino.Close(); //Cierra la conexion
                    }
                    else if (mensaje == "0\r") //Error
                    {
                        conexionArduino.Close(); //Cierra la conexion
                        estado = true; //Cambia el estado actual
                        digitalizarB.IsEnabled = true; //Rehabilita el boton
                    }
                }
                conexionArduino.Open(); //Vuelve a abrir la conexion

                if (!estado)
                {
                    idC.Text = conexionArduino.ReadLine(); //Lee el id del arduino
                    idC.Text = (int.Parse(idC.Text) + 1).ToString(); //Le suma 1 para cuestiones de la base de datos

                    habilitar(sender, e);                
                }
            }
            catch (System.IO.IOException error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        private void agregarB_Click(object sender, RoutedEventArgs e)
        {
            string copiaDir = "D:\\Proyectos\\Projects\\SGRM\\Fotos\\"; //Directorio a copiar las fotos
            System.IO.File.Copy(ruta, (copiaDir + idC.Text)); //Copia la imagen a otra carpeta
            string rutaFinal = (copiaDir + idC.Text); //Guarda en la variable global la ruta de la foto copiada

            string fecha = ""; //Cadena que llevará la fecha de nacimiento del paciente
            Conexion newPaciente = new Conexion(); //Objeto a la conexion de la DB

            DateTime dateTime = DateTime.UtcNow.Date; //Objeto con la fecha actual

            //Fecha de nacimiento del paciente
            fecha = añoCB.SelectedItem.ToString() + "-" + Convert.ToString(mesCB.SelectedIndex + 1) + "-" + diaCB.SelectedItem.ToString();

            string alergias = ""; //Cadena con las alergias del paciente
            string operaciones = ""; //Cadena con las operaciones
            string enfermedades = ""; //Cadena con las enfermedades

            //Recorren los combobox para sacar sus elementos
            if (listaAler.Items.GetItemAt(0).ToString() != "Ninguna")
                foreach (object alergiaItem in listaAler.Items)
                    alergias += alergiaItem.ToString() + "/";

            //Recorren los combobox para sacar sus elementos
            if (listaOper.Items.GetItemAt(0).ToString() != "Ninguna")
                foreach (object operacionItem in listaOper.Items)
                    operaciones += operacionItem.ToString() + "/";

            //Recorren los combobox para sacar sus elementos
            if (listaEnfer.Items.GetItemAt(0).ToString() != "Ninguna")
                foreach (object enfermedadItem in listaEnfer.Items)
                    enfermedades += enfermedadItem.ToString() + "/";

            string[] datosPaciente = {idC.Text, nombreC.Text, paternoC.Text, maternoC.Text, fecha, dateTime.ToString("yyyy-MM-dd"), Convert.ToString(generoCB.SelectedIndex + 1),
                        Convert.ToString(sangreCB.SelectedIndex + 1), rutaFinal, Convert.ToString((Convert.ToInt32(idC.Text)) - 1), calleC.Text, numC.Text, colC.Text, ciudad2C.Text, estado2CB.SelectedItem.ToString(),
                        cpC.Text, telCasaC.Text, telCelC.Text, refCasaC.Text, refCelC.Text, alergias, operaciones, enfermedades, ciudadC.Text, estadoCB.SelectedItem.ToString(), edadC.Text  };

            //Envio de los datos de los inputs al metodo de agregar paciente
            if (newPaciente.nuevoPaciente(datosPaciente))
            {
                limpiar();
            }
            else
            {
                MessageBox.Show("Imposible guardar!");
            }
        }        
    }
}
