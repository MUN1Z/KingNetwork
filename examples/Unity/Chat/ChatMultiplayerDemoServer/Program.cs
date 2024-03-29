﻿using ChatMultiplayerDemoShared;
using KingNetwork.Server;
using KingNetwork.Server.Interfaces;
using KingNetwork.Shared;
using KingNetwork.Shared.Interfaces;
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
        /// <param name="reader">The king buffer reader from received message.</param>
        private void OnMessageReceived(IClientConnection client, IKingBufferReader reader)
        {
            try
            {
                switch (reader.ReadMessagePacket<MyPackets>())
                {
                    case MyPackets.Message:
                        Console.WriteLine($"Received message: {reader.ReadString()}.");
                        _server.SendMessageToAllMinus(client, KingBufferWriter.Create());
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
