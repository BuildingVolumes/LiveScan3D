using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AudioRecorderClient
{
    public class NetworkClient
    {
        //Do the things!
        public Action StartRecording;
        public Action StopRecording;
        public Action SendData;
        Thread listenerThread;
        //Server stuff.
        TcpClient client;
        const int port = 48002;
        public void Connect(string server)
        {
            RecorderWindow.SetStatusBarStatus("Trying to Connect");

            //sanitize input
            server = server.Trim();
            server = server == "" ? "localhost" : server;//if the server is null, set it to localhost. otherwise ignore (set it to itself).
;
            try
            {
                client = new TcpClient(server, port);
                listenerThread = new Thread(ListenerHandler);
                listenerThread.Start();
                RecorderWindow.SetStatusBarStatus("Connected to "+server);
            }
            catch (ArgumentNullException e)
            {
                RecorderWindow.SetStatusBarStatus(string.Format("ArgumentNullException: {0}", e));
            }
            catch (SocketException e)
            {
                RecorderWindow.SetStatusBarStatus(string.Format("SocketException: {0}", e));
            }
        }       

        public void ListenerHandler()
        {
            NetworkStream stream = client.GetStream();
            while (client.Connected)
            {
                if(client.Available > 0)
                {
                    int offset = 0;
                    int size = client.Available;
                    byte[] buffer = new byte[size];
                    stream.Read(buffer, offset, size);
                    HandleReceive(buffer);
                }
            }

            //Do we need to manually abort this thread or will it just end when the function ends?
            RecorderWindow.SetStatusBarStatus("Disconnected");
        }

        private void HandleReceive(byte[] buffer)
        {
            RecorderWindow.SetStatusBarStatus(buffer.ToString());
        }

        public void Close()
        {
            client.Close();
        }
    }
}
