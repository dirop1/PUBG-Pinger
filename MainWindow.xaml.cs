using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.NetworkInformation;
using System.IO;
using System.Reflection;

namespace PUBG_Pinger
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary> 
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        String serverToPing = "";
        int refresh = 10;

        public MainWindow()
        {
            InitializeComponent();

            //dataSource.Add(new Server() { Name = "blah", Value = "blah" });
            readFile();


            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);

        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (serverToPing != null && serverToPing != "")
            {
                ping();
            }
        }

        private void readFile()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string path = System.IO.Path.Combine(currentDirectory, "pubg_list_servers.txt");
            Console.WriteLine(path);
            foreach (string line in File.ReadLines(@path))
            {
                // if (line.Contains("episode") & line.Contains("2006"))
                //{
                Server sv = null;
                if (line != null && line != "") {
                    try
                    {
                        String[] data = line.Trim().Split('|');
                        comboBox1.Items.Add(new Server(data[0].Trim(), data[1].Trim()));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }


                }

                //}
            }
            comboBox1.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            toggleRunning();
        }

        private void ping()
        {
            try
            {
                Ping p = new Ping();
                PingReply r;
                string s;
                serverToPing = tbIP.Text;
                if (serverToPing == "" || serverToPing == null)
                {
                    textBlock1.Foreground = Brushes.Red;
                    textBlock1.Text = "No ip to ping";
                    stopRunning();
                    return;
                }

                textBlock1.Foreground = Brushes.Gray;
                textBlock1.Text = "Refreshing...";

                s = serverToPing;
                r = p.Send(s);
                int numPings = 4;
                int curPing = 0;
                //for(int i = 0; i > numPings; i ++)
                if (r.Status == IPStatus.Success)
                {
                    /* textBlock1.Text = "Ping to " + s.ToString() + "[" + r.Address.ToString() + "]" + " Successful"
                        + " Response delay = " + r.RoundtripTime.ToString() + " ms" + "\n";*/

                    textBlock1.Text = r.RoundtripTime.ToString() + "ms";

                    if (r.RoundtripTime < 40)
                        textBlock1.Foreground = Brushes.Green;
                    else if (r.RoundtripTime > 40 && r.RoundtripTime < 60)
                        textBlock1.Foreground = Brushes.DarkOrange;
                    else
                        textBlock1.Foreground = Brushes.Red;
                }
                else
                {
                    textBlock1.Foreground = Brushes.Red;
                    textBlock1.Text = "Cant ping that";
                    stopRunning();
                }
            } catch (Exception e)
            {
                stopRunning();
                textBlock1.Foreground = Brushes.Red;
                textBlock1.Text = "Error";
                Console.WriteLine(e.Message);
            }
        }
        public class Server
        {
            public Server(string name, string value) { this.Name = name; this.Value = value; }
            public string Name { get; set; }
            public string Value { get; set; }
            public override string ToString() { return this.Name; }
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            stopRunning();
            string ip = ((Server)comboBox1.SelectedItem).Value;
            if (ip != null)
            {
                serverToPing = ip;
                tbIP.Text = ip;
            }
            else
            {

            }
        }

        private void sliderRefresh_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            refresh = Convert.ToInt32(sliderRefresh.Value);
            if (refreshLabel != null)
                refreshLabel.Content = "Refresh " + refresh + "s";
            stopRunning();
        }

        private void toggleRunning()
        {
            if (dispatcherTimer.IsEnabled)
                stopRunning();
            else
                run();
        }
        private void stopRunning()
        {            
                btRun.Content = "Start";
            if(dispatcherTimer != null)
                dispatcherTimer.Stop();
            textBlock1.Foreground = Brushes.Gray;
            textBlock1.Text = "Stopped...";
        }

        private void run() {
            btRun.Content = "Running...\n\nPress to Stop";
            dispatcherTimer.Interval = new TimeSpan(0, 0, refresh);
            dispatcherTimer.Start();
            ping();
        }
    }
}
