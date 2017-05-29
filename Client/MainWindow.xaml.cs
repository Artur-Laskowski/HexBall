using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int playerIndex;
        private ConnectionController cc;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Connect_Server(object sender, RoutedEventArgs e)
        {
            try
            {
                string ip = this.IPBox.Text;
                int port = int.Parse(this.PortBox.Text);
                cc = new ConnectionController(ip, port);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd");
            }
        }
    }
}
