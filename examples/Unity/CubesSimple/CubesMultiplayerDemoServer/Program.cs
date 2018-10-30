using CubesMultiplayerDemoShared;
using KingNetwork.Server;
using KingNetwork.Server.Interfaces;
using KingNetwork.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CubesMultiplayerDemoServer
{
    /// <summary>
    /// This class represents the program instance.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The network players dictionary.
        /// </summary>
        private Dictionary<IClient, NetworkPlayer> _networkPlayersDictionary;

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
                _networkPlayersDictionary = new Dictionary<IClient, NetworkPlayer>();

                _server = new KingServer();

                _server.OnMessageReceivedHandler = OnMessageReceived;
                _server.OnClientConnectedHandler = OnClientConnectedHandler;
                _server.OnClientDisconnectedHandler = OnClientDisconnectedHandler;
                _server.OnServerStartedHandler = OnServerStartedHandler;

                _server.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// This method is responsible for sychronize the players positions.
        /// </summary>
        public void SynchronizePlayersPositions()
        {
            try
            {
                var sendPosDict = new Dictionary<IClient, NetworkPlayer>(_networkPlayersDictionary);

                foreach (var sendToPlayer in sendPosDict)
                {
                    if (sendToPlayer.Value == null)
                        continue;

                    using (var kingBuffer = new KingBuffer())
                    {
                        kingBuffer.WriteMessagePacket(MyPackets.PlayerPositionsArray);
                        kingBuffer.WriteInteger(sendPosDict.Count(c => c.Key.Id != sendToPlayer.Key.Id && c.Value.Moved));

                        int amountPlayersMoved = 0;

                        foreach (var posPlayers in sendPosDict)
                        {
                            if (sendToPlayer.Key.Id == posPlayers.Key.Id)
                                continue;

                            if (!posPlayers.Value.Moved)
                                continue;

                            kingBuffer.WriteInteger(posPlayers.Key.Id);

                            kingBuffer.WriteFloat(posPlayers.Value.X);
                            kingBuffer.WriteFloat(posPlayers.Value.Y);
                            kingBuffer.WriteFloat(posPlayers.Value.Z);

                            amountPlayersMoved++;
                        }

                        if (amountPlayersMoved > 0)
                            _server.SendMessage(sendToPlayer.Key, kingBuffer);
                    }
                }

                foreach (var player in _networkPlayersDictionary)
                    player.Value.Moved = false;
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

                        Console.WriteLine($"client {client.Id} sent position packet : {x} | {y} | {z}");

                        _networkPlayersDictionary[client].X = x;
                        _networkPlayersDictionary[client].Y = y;
                        _networkPlayersDictionary[client].Z = z;

                        using (var buffer = new KingBuffer())
                        {
                            buffer.WriteMessagePacket(MyPackets.PlayerPosition);

                            buffer.WriteInteger(client.Id);

                            buffer.WriteFloat(x);
                            buffer.WriteFloat(y);
                            buffer.WriteFloat(z);

                            _server.SendMessageToAllMinus(client, buffer);
                        }

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

                var newPlayer = new NetworkPlayer(client);
                _networkPlayersDictionary.Add(client, newPlayer);

                using (var kingBuffer = new KingBuffer())
                {
                    kingBuffer.WriteMessagePacket(MyPackets.PlayerPositionsArray);
                    kingBuffer.WriteInteger(_networkPlayersDictionary.Count);
                    
                    foreach (var player in _networkPlayersDictionary)
                    {
                        kingBuffer.WriteInteger(player.Key.Id);

                        kingBuffer.WriteFloat(player.Value.X);
                        kingBuffer.WriteFloat(player.Value.Y);
                        kingBuffer.WriteFloat(player.Value.Z);
                    }

                    _server.SendMessage(client, kingBuffer);
                }

                using (var kingBuffer = new KingBuffer())
                {
                    kingBuffer.WriteMessagePacket(MyPackets.PlayerPosition);

                    kingBuffer.WriteInteger(newPlayer.IClient.Id);

                    kingBuffer.WriteFloat(newPlayer.X);
                    kingBuffer.WriteFloat(newPlayer.Y);
                    kingBuffer.WriteFloat(newPlayer.Z);
                    
                    _server.SendMessageToAllMinus(client, kingBuffer);
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

        /// <summary>
        /// Method responsible for execute the callback of on server started handler.
        /// </summary>
        private void OnServerStartedHandler()
        {
            try
            {
                //Console.WriteLine("OnServerStartedHandler");

                //new Thread(() => 
                //{
                //    while (true)
                //    {
                //        SynchronizePlayersPositions();
                //    }
                //}).Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }
    }
}
