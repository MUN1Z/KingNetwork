using KingNetwork.Client;
using System;
using System.Collections.Generic;
using System.Threading;

namespace KingNetwork.TestClient
{
    class Program
    {
		static void Main(string[] _args)
		{
			var clients = new List<KingClient>();

			for (var i = 0; i < 1; i++)
				clients.Add(new KingClient());

			foreach (var client in clients)
			{
				client.Connect("127.0.0.1", 7171);
				Thread.Sleep(15);
			}

			Console.ReadLine();
		}
	}
}
