using System;
using System.Collections.Generic;
using System.Threading;

namespace KingNetwork.Client
{
    public class KingClient
    {
        #region private members 	

        /// <summary> 	
        /// The network dictionary list of server packet handlers. 	
        /// </summary> 	
        private Dictionary<ushort, ClientPacketHandler> _clientPacketHandlers;

        private NetworkListener _networkListener;

        private Thread _clientThread;

        #endregion

        #region delegates 	

        /// <summary> 	
        /// The client packet handler delegate. 	
        /// </summary> 	
        /// <param name="data">The bytes data from message.</param>
        public delegate void ClientPacketHandler(byte[] data);

        #endregion

        public bool HasConnected => _networkListener != null ? _networkListener.IsConnected : false;

        public KingClient()
        {

        }

        public void Connect(string ip, ushort port)
        {
            try
            {
                _clientThread = new Thread(() =>
                {
                    _networkListener = new NetworkListener();
                    _networkListener.StartClient(ip, port);
                    _networkListener.StartListening();
                });

                _clientThread.IsBackground = true;
                _clientThread.Start();
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
