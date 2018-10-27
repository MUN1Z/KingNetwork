using CubesMultiplayerDemoShared;
using KingNetwork.Server;
using KingNetwork.Server.Interfaces;
using KingNetwork.Shared;
using System;
using System.Collections.Generic;

namespace CubesMultiplayerDemoServer
{
    /// <summary>
    /// This class represents the program instance.
    /// </summary>
    class Program
    {

        private Dictionary<IClient, NetworkPlayer> _networkPlayersDictionary;
        private KingServer _server;

        public void Run()
        {
            try
            {
                _networkPlayersDictionary = new Dictionary<IClient, NetworkPlayer>();

                _server = new KingServer();

                _server.OnMessageReceivedHandler = OnMessageReceived;
                _server.OnClientConnectedHandler = OnClientConnectedHandler;
                _server.OnClientDisconnectedHandler = OnClientDisconnectedHandler;

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
        private void OnMessageReceived(IClient client, KingBuffer kingBuffer)
        {
            try
            {
                switch (kingBuffer.ReadMessagePacket<MyPackets>())
                {
                    case MyPackets.PlayerPosition:

                        float x = kingBuffer.ReadFloat();
                        float y = kingBuffer.ReadFloat();
                        float z = kingBuffer.ReadFloat();

                        Console.WriteLine($"Got position packet : {x} | {y} | {z}");

                        _networkPlayersDictionary[client].X = x;
                        _networkPlayersDictionary[client].Y = y;
                        _networkPlayersDictionary[client].Z = z;

                        _networkPlayersDictionary[client].Moved = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Method responsible for execute the callback of on client connected handler.
        /// </summary>
        /// <param name="client">The client instance.</param>
        private void OnClientConnectedHandler(IClient client)
        {
            try
            {
                Console.WriteLine($"OnClientConnected: {client.Id}");

                using (var kingBuffer = new KingBuffer())
                {
                    kingBuffer.WriteMessagePacket(MyPackets.PlayerPositionsArray);
                    kingBuffer.WriteInteger(_networkPlayersDictionary.Count);
                    
                    foreach (var player in _networkPlayersDictionary)
                    {
                        kingBuffer.WriteInteger(player.Value.IClient.Id);

                        kingBuffer.WriteFloat(player.Value.X);
                        kingBuffer.WriteFloat(player.Value.Y);
                        kingBuffer.WriteFloat(player.Value.Z);
                    }

                    _server.SendMessage(client, kingBuffer);

                    if (!_networkPlayersDictionary.ContainsKey(client))
                        _networkPlayersDictionary.Add(client, new NetworkPlayer(client));

                    _networkPlayersDictionary[client].Moved = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Method responsible for execute the callback of on client disconnected handler.
        /// </summary>
        /// <param name="client">The client instance.</param>
        private void OnClientDisconnectedHandler(IClient client)
        {
            try
            {
                Console.WriteLine($"OnClientDisconnected: {client.Id}");

                if (_networkPlayersDictionary.ContainsKey(client))
                    _networkPlayersDictionary.Remove(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }
    }
}
