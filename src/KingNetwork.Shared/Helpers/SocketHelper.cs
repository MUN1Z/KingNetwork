using System.Net.Sockets;

namespace KingNetwork.Shared.Helpers
{
    /// <summary>
    /// This class is responsible for provider methotds to help sockets.
    /// </summary>
    public static class SocketHelper
    {
        // <summary> 	
        /// This method is responsible for verify if tcp client is connected in server. 	
        /// </summary> 	
        /// <param name="tcpClient">The instance of tcp client connection.</param>
        public static bool IsConnected(this TcpClient tcpClient)
        {
            try
            {
                return !(tcpClient.Client.Poll(1, SelectMode.SelectRead) && tcpClient.Client.Available == 0);
            }
            catch (SocketException) { return false; }
        }
    }
}
