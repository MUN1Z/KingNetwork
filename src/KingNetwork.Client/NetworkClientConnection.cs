using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace KingNetwork.Client
{
	public class NetworkClientConnection : TcpClient
	{
		public bool IsConnected { get; private set; }

		public IPEndPoint RemoteEndPoint { get; private set; }

		public NetworkClientConnection()
		{
			
		}

		public void StartClient(string ip, int port)
		{
			RemoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            Client.NoDelay = true;
            Connect(ip, port);

            Console.WriteLine("Connected to server!");
		}
    }
}
