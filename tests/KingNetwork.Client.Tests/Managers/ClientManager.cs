namespace KingNetwork.Client.Tests.Managers
{
	public class ClientManager
	{
		private static ClientManager _instance;

		public ClientManager()
		{
			KingClient = new KingClient();
		}

		public static ClientManager GetInstance()
		{
			if (_instance == null)
				_instance = new ClientManager();

			return _instance;
		}

		public KingClient KingClient { get; set; }
	}
}
