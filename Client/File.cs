using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Xml.Serialization;

namespace Client
{
    public class File
    {

        [Serializable]
        public class FileDetails
        {
            public string FILETYPE = "";
            public long FILESIZE = 0;
        }

        private static FileDetails fileDet;

        private static int localPort = 5002;
        private static UdpClient receivingUdpClient = new UdpClient(localPort);
        private static IPEndPoint RemoteIpEndPoint = null;

        private static FileStream fs;
        private static Byte[] receiveBytes = new Byte[0];
        [STAThread]
        public static void Main(MainWindow mainWindow)
        {
            // Получаем информацию о файле
            GetFileDetails(mainWindow);

            // Получаем файл
            ReceiveFile();
        }
        private static void GetFileDetails(MainWindow mainWindow)
        {
            try
            {
                receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);

                XmlSerializer fileSerializer = new XmlSerializer(typeof(FileDetails));
                MemoryStream stream1 = new MemoryStream();

                stream1.Write(receiveBytes, 0, receiveBytes.Length);
                stream1.Position = 0;

                fileDet = (FileDetails)fileSerializer.Deserialize(stream1);
                mainWindow.ShowInfo("Получен файл типа ." + fileDet.FILETYPE +
                    " имеющий размер " + fileDet.FILESIZE.ToString() + " байт");

            }
            catch (Exception eR)
            {
            }
        }
        public static void ReceiveFile()
        {
            try
            {
                receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);
                fs = new FileStream("temp." + fileDet.FILETYPE, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs.Write(receiveBytes, 0, receiveBytes.Length);
            }
            catch (Exception eR)
            {
            }
            finally
            {
                fs.Close();
            }
        }
    }
}
