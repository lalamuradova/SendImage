using SendImage.Command;
using SendImage.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SendImage.ViewModels
{
    public class ServerViewModel : BaseViewModel
    {

        private ObservableCollection<AppImage> appImage;

        public ObservableCollection<AppImage> AppImage
        {
            get { return appImage; }
            set { appImage = value; OnPropertyChanged(); }
        }
        public RelayCommand RunCommand { get; set; }
        public object obj = new object();
        public string path { get; set; }

        public ServerViewModel()
        {
            AppImage = new ObservableCollection<AppImage>();

            RunCommand = new RelayCommand((sender) =>
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                System.IO.Directory.CreateDirectory(desktopPath + "\\Images");
                path = desktopPath + "\\Images";

                Task.Run(() =>
                {
                      var ipAddress = IPAddress.Parse("10.1.18.72");
                      var port = 27001;
                      using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                      {
                          var ep = new IPEndPoint(ipAddress, port);
                          socket.Bind(ep);

                          socket.Listen(10);


                          ///////////////////

                          while (true)
                          {
                              var client = socket.Accept();

                              Task.Run(() =>
                              {

                                  var length = 0;
                                  var bytes = new byte[10000];
                                  do
                                  {
                                      length = client.Receive(bytes);
                                      var path = GetImagePath(bytes, DateTime.Now.Millisecond);

                                      var appimage = new AppImage
                                      {
                                          RemoteEndPoint = client.RemoteEndPoint.ToString(),
                                          ImagePath = path
                                      };

                                      App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                      {
                                          AppImage.Add(appimage);
                                      });
                                      
                                      

                                  } while (length > 0);

                              });

                          }


                      }
                  });
              });



        }
        public string GetImagePath(byte[] buffer, int counter)
        {
            ImageConverter ic = new ImageConverter();
            Image img = (Image)ic.ConvertFrom(buffer);
            Bitmap bitmap1 = new Bitmap(img);
            bitmap1.Save($@"{path}\{counter}.png");
            var imagepath = $@"{path}\{counter}.png";
            return imagepath;
        }

    }
}
