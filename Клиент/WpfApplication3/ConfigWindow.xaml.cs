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

namespace WpfApplication3
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        MainWindow MainWindow;
        public ConfigWindow(MainWindow mw)
        {
            MainWindow = mw;
            InitializeComponent();
            t1.Text = mw.TranslatedRequests[0];
            t2.Text = mw.TranslatedRequests[1];
            t3.Text = mw.TranslatedRequests[2];
            t4.Text = mw.TranslatedRequests[3];
            t5.Text = mw.TranslatedRequests[4];
            t6.Text = mw.TranslatedRequests[5];
            t7.Text = mw.TranslatedRequests[6];
            Icon = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\icon.jpg"));
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.TranslatedRequests[0] = t1.Text;
            MainWindow.TranslatedRequests[1] = t2.Text;
            MainWindow.TranslatedRequests[2] = t3.Text;
            MainWindow.TranslatedRequests[3] = t4.Text;
            MainWindow.TranslatedRequests[4] = t5.Text;
            MainWindow.TranslatedRequests[5] = t6.Text;
            MainWindow.TranslatedRequests[6] = t7.Text;
            MainWindow.SendConfig();
            this.Close();
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
