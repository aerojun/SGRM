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


namespace SGRM
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private string usuario = "";
        private string password = "";
        BackgroundWorker backgroundWorker1 = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += delegate
            {
                Dispatcher.BeginInvoke(new Action(delegate { progreso.Visibility = System.Windows.Visibility.Visible;  }));
                Thread.Sleep(1000);
                Dispatcher.BeginInvoke(new Action(delegate {   Verificar(); }));
            };
            worker.RunWorkerAsync();
        }
      

        public void Verificar()
        {
            /*
             * Obtiene los datos de los textbox
             */
            usuario = user.Text;
            password = pass.Password;
            /*
             * Limpia los textbox
             */
            user.Clear();
            pass.Clear();

            ConexionDB conectar = new ConexionDB();
            if (conectar.verificarUsuario(usuario, password))
            {
                var control = new Controles();
                mostrarControl();
                progreso.Visibility = System.Windows.Visibility.Hidden;
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
            var segundaFase = new Controles();
            segundaFase.Show();
            segundaFase.Closing += Controles_Closing;
        }

        private void Controles_Closing(object sender, EventArgs e)
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
