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
    public class UdpNetworkListener : NetworkListener
    {
        #region properties

        /// <summary>
        /// The socket connection listener;
        /// </summary>
        public Socket Socket => _udpListener;

        #endregion

        #region private members

        /// <summary>
        /// The kingUdpClients list.
        /// </summary>
        private Dictionary<EndPoint, UdpClientConnection> _kingUdpClients;

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
        public UdpNetworkListener(ushort port, ClientConnectedHandler clientConnectedHandler,
            MessageReceivedHandler messageReceivedHandler,
            ClientDisconnectedHandler clientDisconnectedHandler,
            ushort maxMessageBuffer) : base(clientConnectedHandler, messageReceivedHandler, clientDisconnectedHandler, maxMessageBuffer)
        {
            try
            {
                _kingUdpClients = new Dictionary<EndPoint, UdpClientConnection>();
                _udpListener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _udpListener.Bind(new IPEndPoint(IPAddress.Any, port));

                EndPoint endPointFrom = new IPEndPoint(IPAddress.Any, 0);

                byte[] array = new byte[_maxMessageBuffer];
                _udpListener.BeginReceiveFrom(array, 0, _maxMessageBuffer, SocketFlags.None, ref endPointFrom, new AsyncCallback(ReceiveDataCallback), array);

                Console.WriteLine($"Starting the UDP network listener on port: {port}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion

        #region private methods implementation

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

            var kingUdpClientsObj = _kingUdpClients;

            Monitor.Enter(kingUdpClientsObj);

            bool hasClientConnection = false;
            UdpClientConnection kingUdpClient = null;

            try
            {
                hasClientConnection = _kingUdpClients.TryGetValue(endPoint, out kingUdpClient);
            }
            finally
            {
                Monitor.Exit(kingUdpClientsObj);
            }

            if (hasClientConnection)
            {
                kingUdpClient.ReceiveDataCallback(array);
            }
            else if (array.Length == 9)
            {
                var clientId = GetNewClientIdentifier();
                var client = new UdpClientConnection(clientId, _udpListener, endPoint, _messageReceivedHandler, _clientDisconnectedHandler, _maxMessageBuffer);

                _kingUdpClients.Add(endPoint, client);

                var writter = KingBufferWriter.Create();
                writter.Write((byte)1);

                client.SendMessage(writter);

                _clientConnectedHandler(client);
            }
        }

        #endregion
    }
}