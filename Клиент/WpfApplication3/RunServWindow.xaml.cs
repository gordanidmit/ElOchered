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
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace WpfApplication3
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class RunServWindow : Window
    {
        int port;
        List<IPAddress> ip;
        MainWindow MainWindow;
        public RunServWindow(MainWindow mw)
        {
            MainWindow = mw;
            InitializeComponent();
            Icon = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\icon.jpg"));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string hostName = Dns.GetHostName();
            ip = new List<IPAddress>(Dns.GetHostEntry(hostName).AddressList);
            for (int i = 0; i < ip.Count; i++)
                if (!ip[i].IsIPv6LinkLocal && !ip[i].IsIPv6Teredo && !ip[i].IsIPv6Multicast && !ip[i].IsIPv6SiteLocal && !ip[i].IsIPv4MappedToIPv6)
                    IPBox.Items.Add(ip[i]);
                else
                {
                    ip.Remove(ip[i]);
                    i--;
                }
        }

        private void bStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                port = int.Parse(portBox.Text);
                if (!(port >= 0 && port < Math.Pow(2, 16)))
                    int.Parse("q");
            }
            catch { MessageBox.Show("Ошибка ввода"); return; }
            try
            {
                TcpListener tl = new TcpListener(port);
                tl.Start();
                tl.Stop();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Ошибка: " + exc.Message); return;
            }
            Process proc = new Process();
            proc.StartInfo.FileName = "ConsoleApplication42.exe";
            proc.StartInfo.Arguments = ip[IPBox.SelectedIndex].ToString() + " " + port;
            proc.Start();
            MainWindow.RunningServer = proc;
            MainWindow.IPOfServ = ip[IPBox.SelectedIndex];
            MainWindow.PortOfServ = port;
            MainWindow.StartConnection(true);
            this.Close();
        }

        private void bExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

