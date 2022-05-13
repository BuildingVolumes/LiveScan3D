using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using System.Net.Sockets;
using System.Net;


namespace KinectServer
{
    public class TransferServer
    {
        public List<float> lVertices = new List<float>();
        public List<byte> lColors = new List<byte>();

        bool bServerRunning = false;
        ManualResetEvent allDone = new ManualResetEvent(false);

        TcpListener oListener;
        Thread listeningThread;
        Thread receivingThread;
        List<TransferSocket> lClientSockets = new List<TransferSocket>();
        object oClientSocketLock = new object();

        ~TransferServer()
        {
            StopServer();
        }

        public void StartServer()
        {
            if (!bServerRunning)
            {
                oListener = new TcpListener(IPAddress.Any, 48002);
                oListener.Start();

                bServerRunning = true;
                listeningThread = new Thread(this.ListeningWorker);
                listeningThread.Start();
                receivingThread = new Thread(this.ReceivingWorker);
                receivingThread.Start();
            }
        }

        public void StopServer()
        {
            if (bServerRunning)
            {
                bServerRunning = false;
                allDone.Set();
                listeningThread.Join();
                receivingThread.Join();

                oListener.Stop();
                lock (oClientSocketLock)
                    lClientSockets.Clear();
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            // Get the listener that handles the client request.
            TcpListener listener = (TcpListener)ar.AsyncState;
            if (listener == null || !bServerRunning)
            {
                return;
            }

            // End the operation and display the received data on
            // the console.
            TcpClient newClient = listener.EndAcceptTcpClient(ar);

            // Signal main thread to go ahead
            allDone.Set();

            //we do not want to add new clients while a frame is being requested
            lock (oClientSocketLock)
            {
                lClientSockets.Add(new TransferSocket(newClient));
            }
        }

        private void ListeningWorker()
        {
            while (bServerRunning)
            {
                allDone.Reset();
                try
                {
                    oListener.BeginAcceptTcpClient(new AsyncCallback(AcceptCallback), oListener);
                }
                catch (SocketException e)
                {
                    Log.LogError("Error accepting TCP Client");
                }
                allDone.WaitOne();
            }
        }

        private void ReceivingWorker()
        {
            System.Timers.Timer checkConnectionTimer = new System.Timers.Timer();
            checkConnectionTimer.Interval = 1000;

            checkConnectionTimer.Elapsed += delegate (object sender, System.Timers.ElapsedEventArgs e)
            {
                lock (oClientSocketLock)
                {
                    for (int i = 0; i < lClientSockets.Count; i++)
                    {                      
                        if (!lClientSockets[i].SocketConnected())
                        {
                            lClientSockets.RemoveAt(i);
                            i--;
                        }
                    }
                }
            };

            checkConnectionTimer.Start();

            while (bServerRunning)
            {
                lock (oClientSocketLock)
                {
                    for (int i = 0; i < lClientSockets.Count; i++)
                    {
                        byte[] buffer = lClientSockets[i].Receive(1);

                        while (buffer.Length != 0)
                        {
                            if (buffer[0] == 0)
                            {
                                lock (lVertices)
                                {
                                    lClientSockets[i].SendFrame(lVertices, lColors);
                                }
                            }

                            buffer = lClientSockets[i].Receive(1);
                        }
                    }
                }

                Thread.Sleep(10);
            }

            checkConnectionTimer.Stop();
        }
    }
}
