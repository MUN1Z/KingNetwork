using KingNetwork.Client;
using KingNetwork.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Timers;

namespace KingNetwork.Benchmarks.Load
{
    /// <summary>
    /// This class is responsible for start the clients.
    /// </summary>
    public class RunClients
    {
        /// <summary>
        /// The value of messages sent to server
        /// </summary>
        private static long _messagesSent = 0;

        /// <summary>
        /// The value of messages received from server
        /// </summary>
        private static long _messagesReceived = 0;

        /// <summary>
        /// The value of data received from server
        /// </summary>
        private static long _dataReceived = 0;

        /// <summary>
        /// The value of buffer length
        /// </summary>
        private static int _bufferLength = 0;

        /// <summary>
        /// The instance of stop whatch
        /// </summary>
        private static Stopwatch _stopwatch;

        /// <summary>
        /// Method responsible for start the clients.
        /// </summary>
        /// <param name="ip">The ip address from the server.</param>
        /// <param name="clientConnections">The number of client connections.</param>
        public static void StartClients(string ip, int clientConnections)
        {
            var kingBuffer = new KingBuffer();
            kingBuffer.WriteString("Sometimes we just need a good networking library");

            _stopwatch = Stopwatch.StartNew();

            var clientFrequency = 14;

            var clients = new List<KingClient>();

            for (int i = 0; i < clientConnections; ++i)
            {
                var client = new KingClient();

                client.MessageReceivedHandler = OnMessageReceived;

                client.Connect(ip);
                clients.Add(client);

                Thread.Sleep(15);
            }

            Console.WriteLine("started all clients");

            var timer = new System.Timers.Timer(1000.0 / clientFrequency);

            timer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {

                foreach (var client in clients)
                {
                    if (client.HasConnected)
                    {
                        // send 2 messages each time
                        client.SendMessage(kingBuffer);
                        client.SendMessage(kingBuffer);

                        _messagesSent += 1;
                    }
                }
                
                // report every 10 seconds
                if (_stopwatch.ElapsedMilliseconds > 1000 * 10)
                {
                    long bandwithIn = _dataReceived * 1000 / (_stopwatch.ElapsedMilliseconds * 1024);
                    long bandwithOut = _messagesSent * _bufferLength * 1000 / (_stopwatch.ElapsedMilliseconds * 1024);

                    Console.WriteLine(string.Format("Client in={0} ({1} KB/s)  out={2} ({3} KB/s) bufferL={4}",
                                             _messagesReceived,
                                             bandwithIn,
                                             _messagesSent,
                                             bandwithOut,
                                             _bufferLength));
                    _stopwatch.Stop();
                    _stopwatch = Stopwatch.StartNew();
                    _messagesSent = 0;
                    _dataReceived = 0;
                    _messagesReceived = 0;
                }
            };

            timer.AutoReset = true;
            timer.Enabled = true;

            Console.ReadLine();
            timer.Stop();
            timer.Dispose();
        }

        /// <summary>
        /// Method responsible for execute the callback of message received from server in client.
        /// </summary>
        /// <param name="kingBuffer">The king buffer from received message.</param>
        private static void OnMessageReceived(KingBuffer kingBuffer)
        {
            try
            {
                _messagesReceived++;
                _dataReceived += kingBuffer.Length();
                _bufferLength = kingBuffer.Length();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }
    }
}
