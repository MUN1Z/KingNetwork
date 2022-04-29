using System;
using System.Collections.Generic;
using KingNetwork.Shared.Enums;
using KingNetwork.Client.Interfaces;
using KingNetwork.Client.Listeners;
using KingNetwork.Shared;
using KingNetwork.Shared.Interfaces;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly Dictionary<byte, ClientPacketHandler> _clientPacketHandlers;

        /// <summary> 	
        /// The network listener instance. 	
        /// </summary> 	
        private INetworkListener _networkListener;

        /// <summary> 	
        /// The Server network listener type. 	
        /// </summary> 	
        private readonly NetworkListenerType _listenerType;

        #endregion

        #region properties

        /// <summary>
        /// The flag of client connection.
        /// </summary>
        public bool HasConnected => _networkListener.HasConnected;

        /// <summary>
        /// The callback of message received handler implementation.
        /// </summary>
        public NetworkListener.MessageReceivedHandler OnMessageReceivedHandler { get; set; }

        /// <summary>
        /// The callback of client disconnected handler implementation.
        /// </summary>
        public NetworkListener.DisconnectedHandler OnDisconnectedHandler { get; set; }

        /// <summary>
        /// The callback of client connection success handler implementation.
        /// </summary>
        public NetworkListener.ConnectionSuccessHandler OnConnectionSuccessHandler { get; set; }

        /// <summary>
        /// The callback of client connection fail handler implementation.
        /// </summary>
        public NetworkListener.ConnectionFailHandler OnConnectionFailHandler { get; set; }

        #endregion

        #region delegates 	

        /// <summary> 	
        /// The client packet handler delegate. 	
        /// </summary> 	
        /// <param name="reader">The king buffer reader of received message.</param>
        public delegate void ClientPacketHandler(IKingBufferReader reader);

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="KingClient"/>.
        /// <param name="listenerType">The listener type to creation of listener, the default value is NetworkListenerType.TCP.</param>
        /// </summary>
        public KingClient(NetworkListenerType listenerType = NetworkListenerType.TCP)
        {
            _listenerType = listenerType;
            _clientPacketHandlers = new Dictionary<byte, ClientPacketHandler>();

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);
        }

        #endregion

        #region public methods implementation

        /// <summary>
        /// Provisory method to log all exceptions of client.
        /// </summary>
        /// <param name="sender">The sender of exception.</param>
        /// <param name="e">The exception event args.</param>
        private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var message = $"Exception: {(e.ExceptionObject as Exception).Message}.";
            Debug.WriteLine(message);
            Console.WriteLine(message);
        }

        /// <summary>
        /// Method responsible for put packet handler in the list of packet handlers.
        /// </summary>
        /// <param name="packet">The value of packet handler.</param>
        public void PutHandler<TPacketHandler, TPacket>(TPacket packet) where TPacketHandler : PacketHandler, new() where TPacket : IConvertible
        {
            if (Enum.IsDefined(typeof(TPacket), packet))
            {
                if (_clientPacketHandlers.ContainsKey((byte)(IConvertible)packet))
                    _clientPacketHandlers.Remove((byte)(IConvertible)packet);

                var handler = new TPacketHandler();

                _clientPacketHandlers.Add((byte)(IConvertible)packet, handler.HandleMessageData);
            }
        }

        /// <summary>
        /// Method responsible for put packet handler in the list of packet handlers.
        /// </summary>
        /// <param name="packet">The value of packet handler.</param>
        /// <param name="clientPacketHandler">The client packet handler callback implementation.</param>
        public void PutHandler<TPacket>(TPacket packet, ClientPacketHandler clientPacketHandler) where TPacket : IConvertible
        {
            if (Enum.IsDefined(typeof(TPacket), packet))
            {
                if (_clientPacketHandlers.ContainsKey((byte)(IConvertible)packet))
                    _clientPacketHandlers.Remove((byte)(IConvertible)packet);

                _clientPacketHandlers.Add((byte)(IConvertible)packet, clientPacketHandler);
            }
        }

        /// <summary>
        /// Method responsible to connect client in server.
        /// </summary>
        /// <param name="ip">The ip address from server.</param>
        /// <param name="port">The port number from server, the default value us 7171</param>
        /// <param name="connectionTimeout">The timeout of client connection, the default value is 5000 milisecounds.</param>
        /// <param name="maxMessageBuffer">The max length of message buffer, the default value is 4096 bytes.</param>
        /// <returns>The boolean value of client connection.</returns>
        public void Connect(string ip, ushort port = 7171, ushort connectionTimeout = 5000, ushort maxMessageBuffer = 4096)
        {
            try
            {
                if (_listenerType == NetworkListenerType.TCP)
                    _networkListener = new TcpNetworkListener(OnMessageReceived, OnDisconnected);
                else if (_listenerType == NetworkListenerType.UDP)
                    _networkListener = new UdpNetworkListener(OnMessageReceived, OnDisconnected);
                else if (_listenerType == NetworkListenerType.RUDP)
                    _networkListener = new RudpNetworkListener(OnMessageReceived, OnDisconnected);
                else if (_listenerType == NetworkListenerType.WSBinary || _listenerType == NetworkListenerType.WSText)
                    _networkListener = new WebSocketNetworkListener(_listenerType, OnMessageReceived, OnDisconnected);

                _networkListener.StartClient(ip, port, maxMessageBuffer);
            }
            catch (Exception ex)
            {
                OnConnectionFailHandler?.Invoke();
                return;
            }

            var tokenSource = new CancellationTokenSource();

            _ = Task.Run(async () =>
            {
                _ = Task.Run(async () => 
                { 
                    await Task.Delay(connectionTimeout);

                    if (HasConnected)
                        return;

                    OnConnectionFailHandler?.Invoke(); 
                    tokenSource.Cancel(); 
                }, tokenSource.Token);

                while (!HasConnected)
                    await Task.Delay(15);

                OnConnectionSuccessHandler?.Invoke();
                tokenSource.Cancel();

            }, tokenSource.Token);
        }

        /// <summary>
        /// Method responsible for disconnect client from server.
        /// </summary>
        public void Disconnect()
        {
            _networkListener.Stop();
        }

        /// <summary>
        /// Method responsible for send message to connected server.
        /// </summary>
        /// <param name="writer">The king buffer writer to send message.</param>
        public void SendMessage(KingBufferWriter writer)
        {
            _networkListener.SendMessage(writer);
        }

        /// <summary>
        /// Method responsible for send reliable and unreliable message to connected server.
        /// </summary>
        /// <param name="writer">The king buffer writer to send message.</param>
        /// <param name="messageType">The message type to send message listener.</param>
        public void SendMessage(KingBufferWriter writer, RudpMessageType messageType)
        {
            if (_networkListener is RudpNetworkListener rudpNetworkListener)
                rudpNetworkListener.SendMessage(writer, messageType);
            else
                _networkListener.SendMessage(writer);
        }

        #endregion

        #region private methods implementation 

        /// <summary>
        /// Method responsible for execute the callback of message received from client in server.
        /// </summary>
        /// <param name="reader">The king buffer reader of received message.</param>
        private void OnMessageReceived(KingBufferReader reader)
        {
            if (reader.Length > 0 && _clientPacketHandlers.Count > 0 && _clientPacketHandlers.TryGetValue(reader.ReadByte(), out var clientPacketHandler))
                clientPacketHandler(reader);
            else
                OnMessageReceivedHandler.Invoke(reader);
        }

        /// <summary>
        /// Method responsible for execute the callback of client disconnected from server.
        /// </summary>
        private void OnDisconnected()
        {
            _networkListener.Stop();
            OnDisconnectedHandler?.Invoke();
        }

        #endregion
    }
}
