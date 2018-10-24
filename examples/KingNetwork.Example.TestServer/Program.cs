using KingNetwork.Server;
using System;
using KingNetwork.Example.Shared.PacketHandlers;

namespace KingNetwork.Example.TestServer {
	class Program {
		private KingServer _server;

		public void Run() {
			try {
				_server = new KingServer(7171);

				_server.PutHandler<MyPacketHandlerOne>(MyPackets.Default);

				_server.Start();
			}
			catch (Exception ex) {
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
