using KingNetwork.Client.PacketHandlers;
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
        private Dictionary<byte, ClientPacketHandler> _clientPacketHandlers;

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
		/// Creates a new instance of a <see cref="KingClient"/>.
		/// </summary>
        public KingClient()
        {
            _clientPacketHandlers = new Dictionary<byte, ClientPacketHandler>();
        }

        #endregion

        #region public methods implementation

        /// <summary>
        /// Method responsible for put packet handler in the list of packet handlers.
        /// </summary>
        /// <param name="packet">The value of packet handler.</param>
        public void PutHandler<TPacketHandler, TPacket>(TPacket packet) where TPacketHandler : PacketHandler, new() where TPacket : IConvertible
        {
            try
            {
                if (Enum.IsDefined(typeof(TPacket), packet))
                {
                    if (_clientPacketHandlers.ContainsKey((byte)(IConvertible)packet))
                        _clientPacketHandlers.Remove((byte)(IConvertible)packet);

                    var handler = new TPacketHandler();

                    if (handler != null)
                        _clientPacketHandlers.Add((byte)(IConvertible)packet, handler.HandleMessageData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Method responsible to connect client in server.
        /// </summary>
        /// <param name="ip">The ip address from server.</param>
        /// <param name="port">The port number from server, the default value us 7171</param>
        /// <param name="maxMessageBuffer">The max length of message buffer, the default value is 4096.</param>
        public void Connect(string ip, ushort port = 7171, ushort maxMessageBuffer = 4096)
        {
            try
            {
                _clientThread = new Thread(() =>
                {
                    _networkListener = new NetworkTcpListener(OnMessageReceived, OnClientDisconnected);
                    _networkListener.StartClient(ip, port, maxMessageBuffer);
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
                
                if (_clientPacketHandlers.TryGetValue(data[0], out clientPacketHandler))
                    clientPacketHandler(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Method responsible for execute the callback of client disconnected from server.
        /// </summary>
        private void OnClientDisconnected()
        {
            try
            {
                //TODO
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion
    }
}
