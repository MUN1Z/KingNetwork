using KingNetwork.Shared;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using static KingNetwork.Server.ClientConnection;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for managing the udp network udp listener.
    /// </summary>
    public class RudpNetworkListener : NetworkListener
    {
        #region properties

        /// <summary>
        /// The socket connection listener;
        /// </summary>
        public Socket UdpSocket => _udpListener;

        #endregion

        #region private members

        /// <summary>
        /// The kingUdpClients list.
        /// </summary>
        private Dictionary<EndPoint, RudpClientConnection> _kingRudpClients;

        /// <summary>
        /// The kingUdpClients list.
        /// </summary>
        private Socket _tcpAcceptConnection;

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="TcpNetworkListener"/>.
        /// </summary>
        /// <param name="port">The port of server.</param>
        /// <param name="clientConnectedHandler">The client connected handler callback implementation.</param>
        /// <param name="messageReceivedHandler">The callback of message received handler implementation.</param>
        /// <param name="clientDisconnectedHandler">The callback of client disconnected handler implementation.</param>
        /// <param name="maxMessageBuffer">The number max of connected clients, the default value is 1000.</param>
        public RudpNetworkListener(
            ushort port,
            ClientConnectedHandler clientConnectedHandler,
            MessageReceivedHandler messageReceivedHandler,
            ClientDisconnectedHandler clientDisconnectedHandler,
            ushort maxMessageBuffer) : base(clientConnectedHandler, messageReceivedHandler, clientDisconnectedHandler, maxMessageBuffer)
        {
            //Tcp
            _tcpListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _tcpListener.Bind(new IPEndPoint(IPAddress.Any, port));
            _tcpListener.Listen(100);
            _tcpListener.BeginAccept(new AsyncCallback(OnAcceptTcp), null);

            //Udp
            _kingRudpClients = new Dictionary<EndPoint, RudpClientConnection>();
            _udpListener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _udpListener.Bind(new IPEndPoint(IPAddress.Any, port));

            EndPoint endPointFrom = new IPEndPoint(IPAddress.Any, 0);

            byte[] array = new byte[_maxMessageBuffer];
            _udpListener.BeginReceiveFrom(array, 0, _maxMessageBuffer, SocketFlags.None, ref endPointFrom, new AsyncCallback(ReceiveDataCallback), array);

            Console.WriteLine($"Starting the RUDP network listener on port: {port}.");
        }

        #endregion

        #region private methods implementation

        /// <summary> 	
        /// The callback from accept client connection. 	
        /// </summary> 	
        /// <param name="asyncResult">The async result from socket accepted in connection.</param>
        private void OnAcceptTcp(IAsyncResult asyncResult)
        {
            _tcpAcceptConnection = _tcpListener.EndAccept(asyncResult);
            _tcpListener.BeginAccept(new AsyncCallback(OnAcceptTcp), null);
        }

        /// <summary> 	
        /// The callback from received message from connected server. 	
        /// </summary> 	
        /// <param name="asyncResult">The async result from a received message from connected server.</param>
        private void ReceiveDataCallback(IAsyncResult asyncResult)
        {
            EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);

            int num;

            try
            {
                num = _udpListener.EndReceiveFrom(asyncResult, ref endPoint);
            }
            catch (SocketException)
            {
                _udpListener.BeginReceiveFrom((byte[])asyncResult.AsyncState, 0, _maxMessageBuffer, SocketFlags.None, ref endPoint, new AsyncCallback(this.ReceiveDataCallback), (byte[])asyncResult.AsyncState);
                return;
            }

            byte[] array = new byte[num];
            Buffer.BlockCopy((byte[])asyncResult.AsyncState, 0, array, 0, num);

            _udpListener.BeginReceiveFrom((byte[])asyncResult.AsyncState, 0, _maxMessageBuffer, SocketFlags.None, ref endPoint, new AsyncCallback(this.ReceiveDataCallback), (byte[])asyncResult.AsyncState);

            var kingUdpClientsObj = _kingRudpClients;

            Monitor.Enter(kingUdpClientsObj);

            bool hasClientConnection = false;
            RudpClientConnection kingRudpClient = null;

            try
            {
                hasClientConnection = _kingRudpClients.TryGetValue(endPoint, out kingRudpClient);
            }
            finally
            {
                Monitor.Exit(kingUdpClientsObj);
            }

            if (hasClientConnection)
            {
                kingRudpClient.ReceiveUdpDataCallback(array);
            }
            else if (array.Length == 9)
            {
                var clientId = GetNewClientIdentifier();
                var client = new RudpClientConnection(clientId, _tcpAcceptConnection, _udpListener, endPoint, _messageReceivedHandler, _clientDisconnectedHandler, _maxMessageBuffer);

                _kingRudpClients.Add(endPoint, client);

                var writter = KingBufferWriter.Create();
                writter.Write((byte)1);

                client.SendMessage(writter);

                _clientConnectedHandler(client);
            }
        }

        #endregion
    }
}
