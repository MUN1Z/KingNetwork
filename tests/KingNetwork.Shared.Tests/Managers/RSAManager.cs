namespace KingNetwork.Shared.Tests.Managers
{
	public class RSAManager
	{
		private static RSAManager _instance;

		public RSAManager()
		{
		}

		public static RSAManager GetInstance()
		{
			if (_instance == null)
				_instance = new RSAManager();

			return _instance;
		}
	}
}
