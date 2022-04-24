﻿using KingNetwork.Server;
using KingNetwork.Server.Interfaces;
using KingNetwork.Shared;
using KingNetwork.Shared.Interfaces;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace KingNetwork.Benchmarks.Load
{
    /// <summary>
    /// This class is responsible for start the server.
    /// </summary>
    public class RunServer
    {
        private static NetworkListenerType _networkListenerType;

        /// <summary>
        /// The value of server frequency
        /// </summary>
        private static int _serverFrequency = 60;

        /// <summary>
        /// The value of messages received from client
        /// </summary>
        private static long _messagesReceived = 0;

        /// <summary>
        /// The value of data received from client
        /// </summary>
        private static long _dataReceived = 0;

        /// <summary>
        /// The instance of king server
        /// </summary>
        private static KingServer _server;

        /// <summary>
        /// The instance of stop whatch
        /// </summary>
        private static Stopwatch _stopwatch;

        /// <summary>
        /// Method responsible for start the server.
        /// </summary>
        public static void StartServer()
        {
            _networkListenerType = NetworkListenerType.WSText;
            _server = new KingServer(_networkListenerType);
            _server.OnMessageReceivedHandler = OnMessageReceived;
            
            new Thread(() =>
            {
                _stopwatch = Stopwatch.StartNew();

                while (true)
                {
                    Thread.Sleep(1000 / _serverFrequency);
                    
                    if (_stopwatch.ElapsedMilliseconds > 1000 * 10)
                    {
                        Console.WriteLine(string.Format("Server in={0} ({1} KB/s)  out={0} ({1} KB/s)", _messagesReceived, (_dataReceived * 1000 / (_stopwatch.ElapsedMilliseconds * 1024))));
                        _stopwatch.Stop();
                        _stopwatch = Stopwatch.StartNew();
                        _messagesReceived = 0;
                        _dataReceived = 0;
                    }
                }
               
            }).Start();

            _server.StartAsync(out var canceletionToken);
        }

        /// <summary>
        /// Method responsible for execute the callback of message received from client in server.
        /// </summary>
        /// <param name="client">The client instance.</param>
        /// <param name="kingBuffer">The king buffer from received message.</param>
        private static void OnMessageReceived(IClientConnection client, IKingBufferReader reader)
        {
            try
            {
                if (_networkListenerType == NetworkListenerType.WSText)
                {
                    Console.WriteLine($"OnMessageReceived: {reader.ReadString()}");
                }
                else
                {
                    var buffer = KingBufferWriter.Create();
                    buffer.Write(reader.ReadString());

                    _server.SendMessage(client, buffer);

                    _messagesReceived++;
                    _dataReceived += reader.Length;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }
    }
}
