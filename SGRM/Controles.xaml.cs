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
using System.Threading;
using System.ComponentModel;

namespace SGRM
{
    /// <summary>
    /// Lógica de interacción para Controles.xaml
    /// </summary>
    public partial class Controles
    {
        //Conexión serial a Arduino
        SerialPort conexionArduino = new SerialPort();
        
        //Ruta de la copia de la foto de perfil del paciente
        string rutaFinal = "";

        //Lista de los estados para el combobox
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
        
        //Lista de los meses
        string[] meses = new string[] { "Enero", "Febrero", "Marzo", "Abril",
                                        "Mayo", "Junio", "Julio", "Agosto",
                                        "Septiembre", "Octubre", "Noviembre", "Diciembre"};
        
        //Lista de los tipos de sangre
        string[] sangre = new string[] {"A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };

        BackgroundWorker backgroundWorker1 = new BackgroundWorker();

        //Constructor
        public Controles()
        {
            Uri NewTheme1 = new Uri(@"/MahApps.Metro;component/Styles/Colours.xaml", UriKind.Relative);
            Uri NewTheme = new Uri(@"/MahApps.Metro;component/Styles/Accents/Blue.xaml", UriKind.Relative);
            ResourceDictionary dictionary = (ResourceDictionary)Application.LoadComponent(NewTheme);
            Application.Current.Resources.MergedDictionaries.Add(dictionary);
            ResourceDictionary dictionary1 = (ResourceDictionary)Application.LoadComponent(NewTheme1);
            Application.Current.Resources.MergedDictionaries.Add(dictionary1);

            InitializeComponent();
         
            //Baudrate del puerto serial
            conexionArduino.BaudRate = 9600;

            //Puerto Serial a usar
            conexionArduino.PortName = "COM3";

            //Llenar el combobox de Estados
            foreach (string s in estados)
            {
                estadoCB.Items.Add(s);
                estado2CB.Items.Add(s);
            }

            //Llenar el combobox de meses
            foreach (string mes in meses)
                mesCB.Items.Add(mes);

            //Llenar el combobox de los tipos de sangre
            foreach (string san in sangre)
                sangreCB.Items.Add(san);

            //Llenar el combobox de años hasta el año actual
            DateTime dateTime1 = DateTime.UtcNow.Date; //Fecha actual
            for (int y = 1920; y <= Convert.ToInt32(dateTime1.ToString("yyyy")); y++)
                añoCB.Items.Add(y);
        }

        //Funcion que verifica que ciertos textbox sean solo numericos
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

        //Funcion que cambia los dias con respecto al mes
        private void cambioMes(object sender, SelectionChangedEventArgs e)
        {
            diaCB.Items.Clear(); //Limpia el combobox de dias
            //Si el mes es...
            if ((mesCB.SelectedIndex == 0) || (mesCB.SelectedIndex == 2) || (mesCB.SelectedIndex == 4)
                || (mesCB.SelectedIndex == 6) || (mesCB.SelectedIndex == 7) || (mesCB.SelectedIndex == 9) || (mesCB.SelectedIndex == 11))
            {
                for (int m = 1; m <= 31; m++) //31 dias
                    diaCB.Items.Add(m);
            }
            else if ((mesCB.SelectedIndex == 3) || (mesCB.SelectedIndex == 5) || (mesCB.SelectedIndex == 10) || (mesCB.SelectedIndex == 8))
            {
                for (int m = 1; m <= 30; m++) //30 dias
                    diaCB.Items.Add(m);
            }
            else
            {
                for (int m = 1; m <= 29; m++) //Febrero con limite de 29 dias
                    diaCB.Items.Add(m);
            }
            diaCB.SelectedIndex = 0; //Selecciona el primer item por default. Dia 1
        }

        //Funcion al dar click al boton de Digitalizar Huella
        private void digiatlizarB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                conexionArduino.Open(); //Abre la conexion al arduino
                Registro(); //Funcion para agregar la huella
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: "+ex.Message); //Mensaje de error si es que ocurre uno
            }
        }

        //Funcion al dar click al boton Agregar. Registra al paciente en la DB.
        private void agregarB_Click(object sender, RoutedEventArgs e)
        {
            registroDB(); //Funcion que registra en la DB
        }

        //Funcion que agrega la huella y libera los campos
        private void Registro()
        {
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
                    MessageBox.Show(mensaje); //Muestra el mensaje si no cae en ninguno de los anteriores
                }

                /**
                 * ESTADOS DE LECTURA
                 * 0 = FALLO
                 * 1 = LECTURA CORRECTA
                 **/

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

            if (!estado) //Checa el estado actual, si no hay cambios...
            {
                idC.Text = conexionArduino.ReadLine(); //Lee el id del arduino
                idC.Text = (int.Parse(idC.Text) + 1).ToString(); //Le suma 1 para cuestiones de la base de datos

                //Habilita todo
                //---------------------------------
                digitalizarB.IsEnabled = false;
                agregarB.IsEnabled = true;
                edadC.IsEnabled = true;
                fotoB.IsEnabled = true;
                nombreC.IsEnabled = true;
                paternoC.IsEnabled = true;
                maternoC.IsEnabled = true;
                generoCB.IsEnabled = true;
                ciudadC.IsEnabled = true;
                cpC.IsEnabled = true;
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
                //----------------------------------------
            }
        }

        //Funcion que registra en la Base de datos
        private void registroDB()
        {
            string fecha = ""; //Cadena que llevará la fecha de nacimiento del paciente
            ConexionDB nuevoPaciente = new ConexionDB(); //Objeto a la conexion de la DB

            DateTime dateTime = DateTime.UtcNow.Date; //Objeto con la fecha actual

            //Fecha de nacimiento del paciente
            fecha = añoCB.SelectedItem.ToString() + "-" + Convert.ToString(mesCB.SelectedIndex + 1) + "-" + diaCB.SelectedItem.ToString();

            string alergias = ""; //Cadena con las alergias del paciente
            string operaciones = ""; //Cadena con las operaciones
            string enfermedades = ""; //Cadena con las enfermedades

            //Recorren los combobox para sacar sus elementos
            if (listaAler.Items.GetItemAt(0).ToString() != "System.Windows.Controls.ListBoxItem: Ninguna")
                foreach (object alergiaItem in listaAler.Items)
                    alergias += alergiaItem.ToString() + "/";

            //Recorren los combobox para sacar sus elementos
            if (listaOper.Items.GetItemAt(0).ToString() != "System.Windows.Controls.ListBoxItem: Ninguna")
                foreach (object operacionItem in listaOper.Items)
                    operaciones += operacionItem.ToString() + "/";

            //Recorren los combobox para sacar sus elementos
            if (listaEnfer.Items.GetItemAt(0).ToString() != "System.Windows.Controls.ListBoxItem: Ninguna")
                foreach (object enfermedadItem in listaEnfer.Items)
                    enfermedades += enfermedadItem.ToString() + "/";

            //Envio de los datos de los inputs al metodo de agregar paciente
            if (nuevoPaciente.agregarPaciente(Convert.ToInt32(idC.Text), nombreC.Text, paternoC.Text, maternoC.Text, fecha, dateTime.ToString("yyyy-MM-dd"), generoCB.SelectedIndex + 1,
                sangreCB.SelectedIndex + 1, rutaFinal, (Convert.ToInt32(idC.Text)) - 1, calleC.Text, Convert.ToInt32(numC.Text), colC.Text, ciudad2C.Text, estado2CB.SelectedItem.ToString(),
                Convert.ToInt32(cpC.Text), Convert.ToInt32(telCasaC.Text), Convert.ToInt32(telCelC.Text), refCasaC.Text, Convert.ToInt32(refCelC.Text), alergias, operaciones, enfermedades, ciudadC.Text, estadoCB.SelectedItem.ToString(), edadC.Text))
            {
                limpiar();
            }
            else
            {
                MessageBox.Show("Imposible guardar!");
            }
        }

        //Funcion que maneja la foto del paciente
        private void fotoB_Click(object sender, RoutedEventArgs e)
        {
            string copiaDir = "D:\\Proyectos\\Projects\\SGRM\\Fotos\\"; //Directorio a copiar las fotos
            Microsoft.Win32.OpenFileDialog nuevaImagen = new Microsoft.Win32.OpenFileDialog(); //Objeto manejador

            nuevaImagen.DefaultExt = "JPEG (*.jpg)|*.jpg"; //Extension default de imagen
            nuevaImagen.Filter = "JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|Bitmap (*.bmp)|*.bmp"; //Posibles extensiones

            Nullable<bool> result = nuevaImagen.ShowDialog(); //Resultado

            if (result == true) //Si fue elegida una imagen ...
            {
                BitmapImage myBitmapImage = new BitmapImage(); //Crear bitmap
                string ruta = nuevaImagen.FileName; //Ruta original
                myBitmapImage.BeginInit(); //Inicia
                myBitmapImage.UriSource = new Uri(ruta); //Origen de la imagen
                myBitmapImage.DecodePixelWidth = 126;
                myBitmapImage.EndInit(); //Fin
                fotoCuadro.Source = myBitmapImage; //Cuadro de la aplicacion muestra la imagen
                System.IO.File.Copy(ruta, (copiaDir + nuevaImagen.SafeFileName)); //Copia la imagen a otra carpeta
            }
            fotoB.Visibility = System.Windows.Visibility.Hidden; //Esconde el boton de la foto
            rutaFinal = (copiaDir + nuevaImagen.SafeFileName); //Guarda en la variable global la ruta de la foto copiada
            
        }
        //Agregar Alergia
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
        //Agregar Enfermedad
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
        //Agregar Operaciones
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

        private void limpiar()
        {
            digitalizarB.IsEnabled = true;
            agregarB.IsEnabled = false;
            edadC.IsEnabled = false;
            fotoB.IsEnabled = false;
            nombreC.IsEnabled = false;
            paternoC.IsEnabled = false;
            maternoC.IsEnabled = false;
            generoCB.IsEnabled = false;
            ciudadC.IsEnabled = false;
            cpC.IsEnabled = false;
            estadoCB.IsEnabled = false;
            diaCB.IsEnabled = false;
            mesCB.IsEnabled = false;
            añoCB.IsEnabled = false;
            sangreCB.IsEnabled = false;
            calleC.IsEnabled = false;
            numC.IsEnabled = false;
            colC.IsEnabled = false;
            ciudad2C.IsEnabled = false;
            estado2CB.IsEnabled = false;
            telCasaC.IsEnabled = false;
            telCelC.IsEnabled = false;
            refCasaC.IsEnabled = false;
            refCelC.IsEnabled = false;
            alergiasB.IsEnabled = false;
            enfermedadesB.IsEnabled = false;
            operacionesB.IsEnabled = false;
            agregAler.IsEnabled = false;
            agregarEnfermedad.IsEnabled = false;
            agregarOperacion.IsEnabled = false;
            edadC.IsEnabled = false;

            nombreC.Clear();
            paternoC.Clear();
            maternoC.Clear();
            ciudadC.Clear();
            cpC.Clear();
            generoCB.SelectedIndex = 0;
            estadoCB.SelectedIndex = 0;
            diaCB.SelectedIndex = 0;
            mesCB.SelectedIndex = 0;
            añoCB.SelectedIndex = 0;
            sangreCB.SelectedIndex = 0;
            calleC.Clear();
            numC.Clear();
            colC.Clear();
            ciudad2C.Clear();
            estado2CB.SelectedIndex = 0;
            telCasaC.Clear();
            telCelC.Clear();
            refCasaC.Clear();
            refCelC.Clear();

            fotoB.Visibility = System.Windows.Visibility.Visible;
        }

        private void buscarWait(object sender, RoutedEventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += delegate
            {
                Dispatcher.BeginInvoke(new Action(delegate { progresoBusqueda.Visibility = System.Windows.Visibility.Visible; }));
                Thread.Sleep(1000);
                Dispatcher.BeginInvoke(new Action(delegate { buscarPaciente(sender, e); }));
            };
            worker.RunWorkerAsync();
        }

        private void buscarPaciente(object sender, RoutedEventArgs e)
        {
            Tuple<Tuple<string, string, string, string, string, int, int, Tuple<string, string, string, string>>, Tuple<string, string, string, string, string, string, string, Tuple<string>>, Tuple<string, string>, Tuple<string, string, string>> data;
            MessageBox.Show("Ponga la huella del paciente");
            int id = 200;
            string idd = "";
            try
            {
                conexionArduino.Open(); //Abre la conexion al arduino

                conexionArduino.Write("0");

                while (conexionArduino.IsOpen)
                {
                    idd = conexionArduino.ReadLine();
                    id = Convert.ToInt32(idd);
                    if (id != 200)
                    {
                        conexionArduino.Close();
                    }
                    else
                    {
                        conexionArduino.Close();
                        MessageBox.Show("Paciente no encontrado!");
                        nPaciente.Focus();
                        return;
                    }
                }

                ConexionDB busqueda = new ConexionDB();
                data = busqueda.busquedaPaciente(id);
                //----------------------------------------------------------------------------
                idC.Text = data.Item1.Item1;
                nombreC.Text = data.Item1.Item2;
                paternoC.Text = data.Item1.Item3;
                maternoC.Text = data.Item1.Item4;
                ciudadC.Text = data.Item1.Rest.Item2;

                int w = 0;
                foreach (object esta in estadoCB.Items)
                {
                    if (esta.ToString().Contains(data.Item1.Rest.Item3))
                    {
                        estadoCB.SelectedIndex = w;
                    }
                    w++;
                }

                edadC.Text = data.Item1.Rest.Item4;

                string temp = data.Item1.Item5;
                                
                mesCB.SelectedIndex = Convert.ToInt32(temp.Substring(4, 1)) - 1;

                int y = 0;
                foreach (object anio in añoCB.Items)
                {
                    if (anio.ToString().Contains(temp.Substring(6, 4)))
                        añoCB.SelectedIndex = y;
                    y++;
                }
                generoCB.SelectedIndex = data.Item1.Item6 - 1;
                sangreCB.SelectedIndex = data.Item1.Item7 - 1;
                rutaFinal = data.Item1.Rest.Item1.Replace(@"\", @"\\");
                fotoB.Visibility = System.Windows.Visibility.Hidden;
                //----------------------------------------------------------------------------
                BitmapImage myBitmapImage = new BitmapImage(); //Crear bitmap
                myBitmapImage.BeginInit(); //Inicia
                myBitmapImage.UriSource = new Uri(rutaFinal); //Origen de la imagen
                myBitmapImage.DecodePixelWidth = 126;
                myBitmapImage.EndInit(); //Fin
                fotoCuadro.Source = myBitmapImage; //Cuadro de la aplicacion muestra la imagen
                //---------------------------------------------------------------------------
                calleC.Text = data.Item2.Item1;
                numC.Text = data.Item2.Item2;
                colC.Text = data.Item2.Item3;
                ciudad2C.Text = data.Item2.Item4;

                int x = 0;
                foreach (object esta in estado2CB.Items)
                {
                    if (esta.ToString().Contains(data.Item2.Item5))
                    {
                        estado2CB.SelectedIndex = x;
                    }
                    x++;
                }

                cpC.Text = data.Item2.Item6;
                telCasaC.Text = data.Item2.Item7;
                telCelC.Text = data.Item2.Rest.Item1;
                refCasaC.Text = data.Item3.Item1;
                refCelC.Text = data.Item3.Item2;
                //---------------------------------------------------------------------
                string temp2 = data.Item4.Item1;
                string temp3 = data.Item4.Item2;
                string temp4 = data.Item4.Item3;

                char[] delimiters = new char[] { '/', ' ' };
                string[] al = temp2.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                string[] en = temp3.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                string[] op = temp4.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                listaEnfer.Items.Clear();
                listaAler.Items.Clear();
                listaOper.Items.Clear();

                foreach (string tipo in al)
                    listaAler.Items.Add(tipo);
                foreach (string tipo in en)
                    listaEnfer.Items.Add(tipo);
                foreach (string tipo in op)
                    listaOper.Items.Add(tipo);

                if (listaAler.Items.Count > 0)
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

                string dia = data.Item1.Item5.Substring(0, 2);

                int z = 0;
                foreach (object diaa in diaCB.Items)
                {
                    if (diaa.ToString().Contains(dia))
                        diaCB.SelectedIndex = z;
                    z++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message); //Mensaje de error si es que ocurre uno
            }
            conexionArduino.Close();
            nPaciente.Focus();
        }
    }
}
