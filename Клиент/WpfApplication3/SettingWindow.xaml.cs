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
using System.IO;
namespace WpfApplication3
{
    /// <summary>
    /// Логика взаимодействия для SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        IPAddress ip;
        int port;
        MainWindow MainWindow;
        string path;
        public SettingWindow(IPAddress i, int p)
        {
            InitializeComponent();
            ip = i;
            port = p;
            ipTextBox.Text = ip.ToString();
            portTextBox.Text = port.ToString();
            bSave.IsEnabled = false;
            Icon = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\icon.jpg")) ;
        }
        public SettingWindow(MainWindow mw)
        {
            InitializeComponent();
            path = Environment.CurrentDirectory + @"\config.txt";
            MainWindow = mw;
            if (ReadConfig())
            {
                ipTextBox.Text = ip.ToString();
                portTextBox.Text = port.ToString();
            }
            else if (MainWindow.IPOfServ != null)
            {
                ipTextBox.Text = MainWindow.IPOfServ.ToString();
                portTextBox.Text = MainWindow.PortOfServ.ToString();
            }
            bSave.IsEnabled = true;
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
            WriteConfig();
        }
        public void Save()
        {
            try
            {
                port = int.Parse(portTextBox.Text);
                ip = IPAddress.Parse(ipTextBox.Text);
                if (port < 0 || port > 65535)
                    Int32.Parse(" ");
            }
            catch { return; }
            MainWindow.PortOfServ = port;
            MainWindow.IPOfServ = ip;
            this.Close();
        }
        bool ReadConfig()
        {
            StreamReader sr;
            bool q = false;
            try
            {
                sr = new StreamReader(path);
                sr.Close();
            }
            catch
            {
                StreamWriter sw = new StreamWriter(path, false);
                sw.Close();
                return q;
            }
            try
            {
                sr = new StreamReader(path);
                ip = IPAddress.Parse(sr.ReadLine());
                port = int.Parse(sr.ReadLine());
                sr.Close();
                q = true;
            }
            catch
            {
                sr.Close();
                StreamWriter sw = new StreamWriter(path, false);
                sw.Close();
            }
            return q;
        }
        bool WriteConfig()
        {
            StreamWriter sw = new StreamWriter(path);
            bool q = false;
            try
            {
                sw.WriteLine(ip.ToString());
                sw.WriteLine(port.ToString());
                sw.Close();
                q = true;
            }
            catch
            {
                sw.Close();
                File.CreateText(path);
            }
            return q;
        }
    }
}
