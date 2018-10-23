using System;
using System.Net;
using System.Threading;

namespace KingNetwork.Client
{
    public class KingClient
    {
	    private NetworkClientConnection _connection;

        private Thread _thread;

	    public bool Connected => _connection != null ? _connection.IsConnected : false;
	    public IPEndPoint RemoteEndPoint => _connection != null ? _connection.RemoteEndPoint : null;

	    public KingClient() 
	    {
		    
		}

	    public void Connect(string ip, ushort port)
	    {
		    try
		    {
                _thread = new Thread(() =>
                {
                    _connection = new NetworkClientConnection();
                    _connection.StartClient(ip, port);
                });

                _thread.IsBackground = true;
                _thread.Start();
            }
		    catch (Exception ex) {

		    }
	    }
    }
}
