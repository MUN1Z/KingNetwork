using ChatMultiplayerDemoShared;
using KingNetwork.Server;
using KingNetwork.Server.Interfaces;
using KingNetwork.Shared;
using System;

namespace ChatMultiplayerDemoServer
{
    /// <summary>
    /// This class represents the program instance.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The king server instance.
        /// </summary>
        private KingServer _server;

        /// <summary>
        /// This method is responsible for run the server.
        /// </summary>
        public void Run()
        {
            try
            {
                _server = new KingServer();
                _server.OnMessageReceivedHandler = OnMessageReceived;
                _server.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// This method is responsible for main execution of console application.
        /// </summary>
        /// <param name="args">The string args received by parameters.</param>
        static void Main(string[] args)
        {
            try
            {
                Program program = new Program();
                program.Run();

                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Method responsible for execute the callback of on message received handler.
        /// </summary>
        /// <param name="client">The client instance.</param>
        /// <param name="kingBuffer">The king buffer from received message.</param>
        private void OnMessageReceived(IClient client, IKingBuffer kingBuffer)
        {
            try
            {
                switch (kingBuffer.ReadMessagePacket<MyPackets>())
                {
                    case MyPackets.Message:
                        Console.WriteLine($"Received message: {kingBuffer.ReadString()}.");
                        _server.SendMessageToAllMinus(client, kingBuffer);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }
    }
}
