using System;
using System.Collections.Generic;
using System.Threading;

namespace KingNetwork.Client
{
    /// <summary>
    /// This class is responsible for management of king client.
    /// </summary>
    public class KingClient
    {
        #region private members 	

        /// <summary> 	
        /// The network dictionary list of server packet handlers. 	
        /// </summary> 	
        private Dictionary<ushort, ClientPacketHandler> _clientPacketHandlers;

        /// <summary> 	
        /// The network tcp listener instance. 	
        /// </summary> 	
        private NetworkTcpListener _networkListener;

        /// <summary> 	
        /// The thread for start the network listener. 	
        /// </summary> 	
        private Thread _clientThread;

        #endregion

        #region properties

        /// <summary>
		/// The flag of client connection.
		/// </summary>
        public bool HasConnected => _networkListener.Connected;

        #endregion

        #region delegates 	

        /// <summary> 	
        /// The client packet handler delegate. 	
        /// </summary> 	
        /// <param name="data">The bytes data from message.</param>
        public delegate void ClientPacketHandler(byte[] data);

        #endregion

        #region constructors

        /// <summary>
		/// Creates a new instance of a <see cref="KingServer"/>.
		/// </summary>
        public KingClient() { }

        #endregion

        #region public methods implementation

        /// <summary>
        /// Method responsible to connect clint in server.
        /// </summary>
        /// <param name="ip">The ip address from server.</param>
        /// <param name="data">The port number from server.</param>
        public void Connect(string ip, ushort port)
        {
            try
            {
                _clientThread = new Thread(() =>
                {
                    _networkListener = new NetworkTcpListener(OnMessageReceived);
                    _networkListener.StartClient(ip, port);
                });

                _clientThread.IsBackground = true;
                _clientThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Method responsible for send message to connected server.
        /// </summary>
        /// <param name="data">The data bytes from message.</param>
        public void SendMessage(byte[] data)
        {
            try
            {
                _networkListener.SendMessage(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion

        #region private methods implementation 

        /// <summary>
        /// Method responsible for execute the callback of message received from client in server.
        /// </summary>
        /// <param name="data">The data bytes from message.</param>
        private void OnMessageReceived( byte[] data)
        {
            try
            {
                ClientPacketHandler clientPacketHandler;

                Console.WriteLine("OnMessageReceived");

                if (_clientPacketHandlers.TryGetValue(data[0], out clientPacketHandler))
                    clientPacketHandler(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion
    }
}
