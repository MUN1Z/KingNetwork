using KingNetwork.Server.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for manipulation of server.
    /// </summary>
    public class KingServer
    {
        #region private members 	
        
        /// <summary> 	
        /// The Server port. 	
        /// </summary> 	
        public ushort Port { get; private set; }

        /// <summary> 	
        /// The client manager instance. 	
        /// </summary> 	
        public IClientManager ClientManager { get; private set; }

        #endregion

        /// <summary>
		/// Creates a new instance of a <see cref="KingServer"/>.
		/// </summary>
        /// <param name="port">The server port.</param>
        public KingServer(ushort port)
        {
            try
            {
                Port = port;
                ClientManager = new ClientManager(this);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Method responsible for start the server.
        /// </summary>
        public void Start()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();

                var listeningTask = RunAsync(cancellationTokenSource.Token);
                
                listeningTask.Wait(cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            ClientManager.Start();

            while (!cancellationToken.IsCancellationRequested)
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
    }
}
