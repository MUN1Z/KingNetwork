using KingNetwork.Server;
using KingNetwork.Shared;
using System;

namespace KingNetwork.TestServer {
	class Program
    {
		private KingServer _server;

		public void Run()
		{
			try 
			{
				_server = new KingServer(7171);

				_server.PutHandler<MyPacketHandler>(MyPackets.Default);

				_server.Start();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
			}
		}

		static void Main(string[] args) {

			var program = new Program();
			program.Run();

			Console.ReadKey();
		}
	}
}
