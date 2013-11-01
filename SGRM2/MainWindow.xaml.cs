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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.ComponentModel;

namespace SGRM2
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ingreso_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += delegate
            {
                Dispatcher.BeginInvoke(new Action(delegate { progreso.Visibility = System.Windows.Visibility.Visible; }));
                Thread.Sleep(1000);
                Dispatcher.BeginInvoke(new Action(delegate { verificarUsuario(user.Text, pass.Password); }));
            };
            worker.RunWorkerAsync();
        }

        private void verificarUsuario(string usuario, string password)
        {
            Conexion conexionDB = new Conexion();

            user.Clear();
            pass.Clear();

            if (conexionDB.iniciarSesion(usuario, password))
            {
                progreso.Visibility = System.Windows.Visibility.Hidden;
                mostrarControl();
            }
            else
            {
                MessageBox.Show("Acceso Denegado");
                progreso.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        public void mostrarControl()
        {
            this.Hide();
            var segundaFase = new Control();
            segundaFase.Show();
            segundaFase.Closing += Control_Closing;
        }

        private void Control_Closing(object sender, EventArgs e)
        {
            this.Show();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }

    }
}
