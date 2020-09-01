using KingNetwork.Server;
using KingNetwork.Shared;
using System.Threading;

namespace KingNetwork.Client.Tests.Managers
{
	public class ServerManager
	{
		private static ServerManager _instance;
		private static Thread _serverTread;

		public ServerManager(NetworkListenerType type)
		{
			_serverTread = new Thread(() =>
			{
				KingServer = new KingServer();
				KingServer.Start(type);
			});

			_serverTread.Start();
		}

		public static ServerManager GetInstance(NetworkListenerType type)
		{
			if (_instance == null)
				_instance = new ServerManager(type);

			return _instance;
		}

		public KingServer KingServer { get; set; }
	}
}
