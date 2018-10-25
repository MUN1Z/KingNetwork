using System;
using System.Threading;

namespace KingNetwork.Client
{
    public class KingClient
    {
        private NetworkListener _networkListener;

        private Thread _thread;

        public bool Connected => _networkListener != null ? _networkListener.IsConnected : false;

        public KingClient()
        {

        }

        public void Connect(string ip, ushort port)
        {
            try
            {
                _thread = new Thread(() =>
                {
                    _networkListener = new NetworkListener();
                    _networkListener.StartClient(ip, port);
                    _networkListener.StartListening();
                });

                _thread.IsBackground = true;
                _thread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        public void SendMessage()
        {
            _networkListener.SendMessage();
        }
    }
}
