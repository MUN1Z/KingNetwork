using KingNetwork.Shared;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for managing the udp network udp listener.
    /// </summary>
    public class UdpNetworkListener : NetworkListener
    {
        #region private members
        
        /// <summary>
		/// The endpoint value to received data.
		/// </summary>
        private EndPoint _endPointFrom;
        
        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="TcpNetworkListener"/>.
        /// </summary>
        /// <param name="port">The port of server.</param>
        /// <param name="clientConnectedHandler">The client connected handler callback implementation.</param>
        public UdpNetworkListener(ushort port, ClientConnectedHandler clientConnectedHandler) : base(port, clientConnectedHandler)
        {
            try
            {
                var ipVersion = IPVersion.IPv4;
                AddressFamily family = (AddressFamily)((ipVersion == IPVersion.IPv6) ? 23 : 2);

                _clientConnectedHandler = clientConnectedHandler;
                _listener = new Socket(family, SocketType.Dgram, ProtocolType.Udp);
                _listener.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port));
                //_listener.NoDelay = true;
                _endPointFrom = new IPEndPoint(IPAddress.Any, 0);

                var socketAsyncEventArgs = KingPoolManager.GetInstance().SocketAsyncEventArgs;

                socketAsyncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnAccept);
                socketAsyncEventArgs.RemoteEndPoint = new IPEndPoint((ipVersion == IPVersion.IPv6) ? IPAddress.IPv6Any : IPAddress.Any, 0);
                socketAsyncEventArgs.BufferList = null;
                socketAsyncEventArgs.SetBuffer(new byte[65535], 0, 65535);

                if (!_listener.ReceiveFromAsync(socketAsyncEventArgs))
                    OnAccept(this, socketAsyncEventArgs);

                //EndPoint endPoint = new IPEndPoint((ipVersion == IPVersion.IPv6) ? IPAddress.IPv6Any : IPAddress.Any, 0);
                //byte[] array = new byte[65535];
                //_listener.BeginReceiveFrom(array, 0, 65535, SocketFlags.None, ref endPoint, new AsyncCallback(OnAccept), array);

                Console.WriteLine($"Starting the server network listener on port: {port}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion
        
        #region private methods implementation

        /// <summary> 	
        /// The callback from accept client connection. 	
        /// </summary> 	
        /// <param name="asyncResult">The async result from socket accepted in connection.</param>
        private void OnAccept(object sender, SocketAsyncEventArgs e)
        {
            byte[] array = new byte[e.BytesTransferred];
            Buffer.BlockCopy(e.Buffer, 0, array, 0, e.BytesTransferred);
            EndPoint remoteEndPoint = e.RemoteEndPoint;
            bool flag = _listener.ReceiveFromAsync(e);

            //Dictionary<EndPoint, NetworkServerConnection> obj = this.udpConnections;
            //Monitor.Enter(obj);

            //bool flag2 = default(bool);

            //NetworkServerConnection networkServerConnection = default(NetworkServerConnection);

            //try
            //{
            //    flag2 = this.udpConnections.TryGetValue(remoteEndPoint, out networkServerConnection);
            //}
            //finally
            //{
            //    Monitor.Exit(obj);
            //}
            //if (flag2)
            //{
            //    networkServerConnection.HandleUdpMessage(array);
            //}
            //else
            //{
            if (e.SocketError != 0)
            {
                e.Completed -= new EventHandler<SocketAsyncEventArgs>(OnAccept);
                KingPoolManager.GetInstance().DisposeSocketAsyncEventArgs(e);
            }
            else
            {
                var ipVersion = IPVersion.IPv4;
                AddressFamily family = (AddressFamily)((ipVersion == IPVersion.IPv6) ? 23 : 2);
                var s = new Socket(family, SocketType.Dgram, ProtocolType.Udp);

                s.Bind(remoteEndPoint);

                _clientConnectedHandler(s);
                Console.WriteLine($"Teste: { array.Length }");
            }
            //}
            if (!flag)
            {
                OnAccept(this, e);
            }


            //if (e.SocketError != 0)
            //{
            //    e.Completed -= new EventHandler<SocketAsyncEventArgs>(OnAccept);
            //    KingPoolManager.GetInstance().DisposeSocketAsyncEventArgs(e);
            //}
            //else
            //{
            //    try
            //    {
            //        _clientConnectedHandler(e.AcceptSocket);
            //    }
            //    finally
            //    {
            //        e.AcceptSocket = null;

            //        if (!_listener.AcceptAsync(e))
            //            OnAccept(this, e);
            //    }
            //}
        }

        /// <summary> 	
        /// The callback from accept client connection. 	
        /// </summary> 	
        /// <param name="asyncResult">The async result from socket accepted in connection.</param>
        private void OnAccept(IAsyncResult asyncResult)
        {
            try
            {
                _clientConnectedHandler(_listener.EndAccept(asyncResult));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
            finally
            {
                _listener.BeginAccept(new AsyncCallback(OnAccept), null);
            }
        }


        #endregion
    }
}
