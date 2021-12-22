using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Collections.ObjectModel;
using System.Drawing;


namespace Client1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string Filename { get; set; }
        public BitmapImage bitmap { get; set; } = new BitmapImage();
        public OpenFileDialog Open { get; set; } = new OpenFileDialog();
        public MainWindow()
        {
            InitializeComponent();            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var socket = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
            var ipAddress = IPAddress.Parse("10.1.18.72");
            var port = 27001;

            var ep = new IPEndPoint(ipAddress, port);

            try
            {
                socket.Connect(ep);
                if (socket.Connected)
                {
                    MessageBox.Show("Connected to the server . . . ");

                    var bytes = GetBytesOfImage(Filename);
                    socket.Send(bytes);                   

                }
                else
                {
                    MessageBox.Show("Can not connect to the server . . .");
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Can not connect to the server . . .");
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Open.Filter = "Image files (*.png)|*.png|All Files (*.*)|*.*";
            Open.ShowDialog();
            Filename = Open.FileName;

            bitmap.BeginInit();
            bitmap.UriSource = new Uri(Filename);
            bitmap.EndInit();
            image.Source = bitmap;
        }


        public byte[] GetBytesOfImage(string path)
        {
            var image = new Bitmap(path);
            ImageConverter imageconverter = new ImageConverter();
            var imagebytes = ((byte[])imageconverter.ConvertTo(image, typeof(byte[])));
            return imagebytes;
        }

    }
}
