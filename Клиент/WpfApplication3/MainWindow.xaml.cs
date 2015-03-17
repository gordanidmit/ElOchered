using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace WpfApplication3
{
    enum requests { prop1, prop2, prop3, prop4, prop5, prop6, EOSes, GetBD, GetHD, GetCl, SetCg, GetCg, enumErr }
    public partial class MainWindow : Window
    {
        Socket Socket;
        public IPEndPoint EndPoint;
        exitedClient CLIENT;
        public ObservableCollection<client> Base
        {
            get;
            set;
        }
        public ObservableCollection<exitedClient> History
        {
            get;
            set;
        }
        public IPAddress IPOfServ
        {
            get;
            set;
        }
        public int PortOfServ
        {
            get;
            set;
        }
        public Process RunningServer
        {
            get;
            set;
        }
        bool ChangedBaseListener
        { get; set; }
        bool temp1
        { get; set; }//переменная для отслеживания потери соединения с сервером
  //      bool BlockAutoRefresh
  //      { get; set; }
        public string[] TranslatedRequests = new string[7];


        public MainWindow()
        {
            Base = new ObservableCollection<client>();
            History = new ObservableCollection<exitedClient>();
            SettingWindow SetWin = new SettingWindow(this);
            SetWin.Save();
            InitializeComponent();
        }



        private void ServerStart_Click(object sender, RoutedEventArgs e)
        {
            if (!(Socket != null && Socket.Connected) && (RunningServer == null || RunningServer.HasExited))
            {
                RunServWindow RSW = new RunServWindow(this);
                RSW.Show();
            }
            else MessageBox.Show("Сервер уже запущен либо установлено соединение");
        }
        private void ConnectToServ_Click(object sender, RoutedEventArgs e)
        {
            if (!(Socket != null && Socket.Connected))
            {
                try
                {
                    EndPoint = new IPEndPoint(IPOfServ, PortOfServ);
                    Socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                    Socket.Connect(EndPoint);
                    MessageBox.Show("Подключение установлено");
                }
                catch { MessageBox.Show("Ошибка подключения, проверьте настройки"); return; }
    //            BlockAutoRefresh = true;
                LoadConfig();
                LoadData();
                LoadClient(0);
     //           BlockAutoRefresh = false;
                temp1 = true;
            }
            else MessageBox.Show("Соединение уже установлено");
        }
        private void EndConnectionToServ_Click(object sender, RoutedEventArgs e)
        {
            if (Socket.Connected)
                try
                {
                    string req = requests.EOSes.ToString();
                    byte[] buf = Encoding.ASCII.GetBytes(req);
                    Socket.Send(buf);
                    temp1 = false;
                    MessageBox.Show("Соединение завершено");
                }
                catch { }
            else MessageBox.Show("Соединение не установлено");
        }
        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow SetWin;
            if (Socket != null && Socket.Connected)
                SetWin = new SettingWindow(IPOfServ, PortOfServ);
            else SetWin = new SettingWindow(this);
            SetWin.Show();
        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                string req = requests.EOSes.ToString();
                byte[] buf = Encoding.ASCII.GetBytes(req);
                Socket.Send(buf);
            }
            catch { }
        }



        public void StartConnection(bool CreatingServ)
        {
            if (CreatingServ)
                Thread.Sleep(500);
            EndPoint = new IPEndPoint(IPOfServ, PortOfServ);
            Socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            try
            {
                Socket.Connect(EndPoint);
            }
            catch { MessageBox.Show("Ошибка подключения"); return; }
            MessageBox.Show("Сервер запущен");
            temp1 = true;
            LoadClient(0);
            LoadData();

        }
        void LoadData()
        {
   //         BlockAutoRefresh = true;
            History.Clear();
            if (!ChangedBaseListener)
                Base.Clear();
            string req = requests.GetBD.ToString();
            byte[] buf = Encoding.ASCII.GetBytes(req);
            Socket.Send(buf);
            byte[] Receive = new byte[1024];
            int Count = 0;
            req = "";
            do
            {
                Count = Socket.Receive(Receive);
                req += Encoding.ASCII.GetString(Receive, 0, Count);
            } while ((Count) == 1024);
            string[] ClientBuf = req.Split(';');
            for (int i = 0; i < ClientBuf.Length; i++)
            {
                try
                {
                    string[] buf2 = ClientBuf[i].Split('_');
                    if (ChangedBaseListener && req != " ")
                        Base.Clear();
                    int n = int.Parse(buf2[0]);
                    string p = TranslatingReq(buf2[1]);
                    DateTime d = DateTime.Parse(buf2[2]);
                    Base.Add(new client(n, p, d));
                }
                catch { }
            }
            req = requests.GetHD.ToString();
            buf = Encoding.ASCII.GetBytes(req);
            Socket.Send(buf);
            Receive = new byte[1024];
            req = "";
            do
            {
                Count = Socket.Receive(Receive);
                req += Encoding.ASCII.GetString(Receive, 0, Count);
            } while ((Count) == 1024);
            ClientBuf = req.Split(';');
            for (int i = 0; i < ClientBuf.Length; i++)
            {
                try
                {
                    string[] buf2 = ClientBuf[i].Split('_');
                    int n = int.Parse(buf2[0]);
                    string p = TranslatingReq(buf2[1]);;
                    DateTime d = DateTime.Parse(buf2[2]);
                    DateTime d2 = DateTime.Parse(buf2[3]);
                    History.Add(new exitedClient(n, p, d, d2));
                }
                catch { }
            }
  //          BlockAutoRefresh = false;
            Counter1.Text = Base.Count.ToString();
            Counter2.Text = History.Count.ToString() ;
        }
        void LoadClient(int i)
        {
   //         BlockAutoRefresh = true;
            string req = requests.GetCl.ToString();
            byte[] buf = Encoding.ASCII.GetBytes(req);
            Socket.Send(buf);
            req = i.ToString("D5");
            buf = Encoding.ASCII.GetBytes(req);
            Socket.Send(buf);
            byte[] Receive = new byte[1024];
            int Count = 0;
            req = "";
            do
            {
                Count = Socket.Receive(Receive);
                req += Encoding.ASCII.GetString(Receive, 0, Count);
            } while ((Count) == 1024);
            if (req == "error")
            {
                MessageBox.Show("Список пуст");
                Base.Clear();
                if (!ChangedBaseListener)
                {
                    Base.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ListChanged);
                    ChangedBaseListener = true;
                }
                return;
            }
            string[] buf2 = req.Split('_');
            int n = int.Parse(buf2[0]);
            string p = TranslatingReq(buf2[1]);
            DateTime d = DateTime.Parse(buf2[2]);
            DateTime d2 = DateTime.Parse(buf2[3]);
            CLIENT = new exitedClient(n, p, d, d2);
            DisplayClient();
   //         BlockAutoRefresh = false;
        }
        void DisplayClient()
        {
            tNumber.Content = CLIENT.Number.ToString("D3");
            tPurpose.Content = CLIENT.Purpose;
            tTimeOfEnter.Content = CLIENT.TimeOfEnter.ToString();
            tTimeOfOut.Content = CLIENT.TimeOfOut.ToString();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(5000);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
   //         if(!BlockAutoRefresh)
            try
            {
                DateTime NowTime = DateTime.Now;
                string NowTimeStr = NowTime.ToString("HH:mm:ss");
                LoadData();
            }
            catch
            {
                if (temp1)
                {
                    MessageBox.Show("Соединение с сервером потеряно");
                    temp1 = false;
                }
    //            BlockAutoRefresh = false;
            }
        }
        private void ListChanged(object sender, EventArgs e)
        {
            Base.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ListChanged);
            MessageBox.Show("Появился клиент в очереди");
            ChangedBaseListener = false;
        }

        private void GetCL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadClient(0);
            }
            catch
            {
                if (temp1)
                {
                    MessageBox.Show("Соединение с сервером потеряно");
                    temp1 = false;
                }
            }
        }

        string TranslatingReq(string s)
        {
            switch (ParseReq(s))
            {
                case requests.prop1:
                    return TranslatedRequests[0];
                case requests.prop2:
                    return TranslatedRequests[1];
                case requests.prop3:
                    return TranslatedRequests[2];
                case requests.prop4:
                    return TranslatedRequests[3];
                case requests.prop5:
                    return TranslatedRequests[4];
                case requests.prop6:
                    return TranslatedRequests[5];
                default:
                    return "";
            }


        }
        requests ParseReq(string value)// Parse из string в request
        {
            try
            {
                return (requests)Enum.Parse(typeof(requests), value);
            }
            catch { return requests.enumErr; }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ConfigWindow cw = new ConfigWindow(this);
            cw.Show();
        }

        public void LoadConfig()
        {
            byte[] Buffer = Encoding.ASCII.GetBytes(requests.GetCg.ToString());
  //          BlockAutoRefresh = true;
            Socket.Send(Buffer);
            Buffer = new byte[1024];
            int Count = 0;
            string msg = "";
            do
            {
                Count = Socket.Receive(Buffer);
                msg += Encoding.Unicode.GetString(Buffer, 0, Count);
            } while ((Count) == 1024);
  //          BlockAutoRefresh = false ;
            TranslatedRequests = msg.Split(';');
        }
        public void SendConfig()
        {
            string msg = "";
            for (int i = 0; i < 7; i++)
                msg += TranslatedRequests[i] + ";";
    //        BlockAutoRefresh = true;
            byte[] Buffer = Encoding.ASCII.GetBytes(requests.SetCg.ToString());
            Socket.Send(Buffer);
            Buffer = Encoding.Unicode.GetBytes(msg);
            Socket.Send(Buffer);
    //        BlockAutoRefresh = false;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
            Environment.Exit(0);
        }
        
    }




    public class client
    {
        int num;
        string purpose;
        DateTime timeOfEnter;
        public client(int n, string p)
        {
            num = n;
            purpose = p;
            timeOfEnter = DateTime.Now;
        }
        public client(int n, string p, DateTime E)
        {
            num = n;
            purpose = p;
            timeOfEnter = E;
        }
        public int Number
        {
            get { return num; }
        }
        public string Purpose
        {
            get { return purpose; }
        }
        public DateTime TimeOfEnter
        {
            get { return timeOfEnter; }
        }
        public string ToString()
        {
            return String.Format("{0,5:d}_{1,10:s}_{2}", Number, Purpose, TimeOfEnter);
        }

    }
    public class exitedClient
    {
        int num;
        string purpose;
        DateTime timeOfEnter;
        DateTime timeOfOut;
        public exitedClient(int n, string p, DateTime E, DateTime O)
        {
            num = n;
            purpose = p;
            timeOfEnter = E;
            timeOfOut = O;
        }
        public int Number
        {
            get { return num; }
        }
        public string Purpose
        {
            get { return purpose; }
        }
        public DateTime TimeOfEnter
        {
            get { return timeOfEnter; }
        }
        public DateTime TimeOfOut
        {
            get { return timeOfOut; }
        }
        public string ToString()
        {
            return String.Format("{0,5:d}_{1,10:s}_{2}_{3}", Number, Purpose, TimeOfEnter, TimeOfOut);
        }
    }
}
