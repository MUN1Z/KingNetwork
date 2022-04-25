using KingNetwork.Client;
using KingNetwork.Shared;
using KingNetwork.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Timers;

namespace KingNetwork.Benchmarks.Load
{
    /// <summary>
    /// This class is responsible for start the clients.
    /// </summary>
    public class RunClients
    {
        private static NetworkListenerType _networkListenerType;

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
            _networkListenerType = NetworkListenerType.TCP;

            var kingBuffer = KingBufferWriter.Create();
            //kingBuffer.Write((byte)0);
            kingBuffer.Write("Sometimes we just need a good networking library");

            _stopwatch = Stopwatch.StartNew();

            var clientFrequency = 14;

            var clients = new List<KingClient>();

            for (int i = 0; i < clientConnections; ++i)
            {
                var client = new KingClient(_networkListenerType);

                client.OnMessageReceivedHandler = OnMessageReceived;

                client.Connect(ip, 7171);
                clients.Add(client);

                Thread.Sleep(15);
            }

            Console.WriteLine("started all clients");

            new Thread(() =>
            {
                Thread.Sleep(10000);

                foreach (var client in clients)
                {
                    //if (client.HasConnected)
                    //{
                        // send 2 messages each time

                        var bytes = Encoding.GetEncoding("UTF-8").GetBytes("Testinho");

                        kingBuffer.Reset();

                        kingBuffer.Write(bytes);

                        client.SendMessage(kingBuffer);
                        //client.SendMessage(kingBuffer);

                        _messagesSent += 1;
                    //}
                }
            }).Start();

            //foreach (var client in clients)
            //{
            //    if (client.HasConnected)
            //    {
            //        // send 2 messages each time
            //        client.SendMessage(kingBuffer);
            //        //client.SendMessage(kingBuffer);

            //        _messagesSent += 1;
            //    }
            //}

            var timer = new System.Timers.Timer(1000.0 / clientFrequency);

            timer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {

                //foreach (var client in clients)
                //{
                //    if (client.HasConnected)
                //    {
                //        // send 2 messages each time
                //        client.SendMessage(kingBuffer);
                //        //client.SendMessage(kingBuffer);

                //        _messagesSent += 1;
                //    }
                //}
                
                //// report every 10 seconds
                //if (_stopwatch.ElapsedMilliseconds > 1000 * 10)
                //{
                //    long bandwithIn = _dataReceived * 1000 / (_stopwatch.ElapsedMilliseconds * 1024);
                //    long bandwithOut = _messagesSent * _bufferLength * 1000 / (_stopwatch.ElapsedMilliseconds * 1024);

                //    Console.WriteLine(string.Format("Client in={0} ({1} KB/s)  out={2} ({3} KB/s) bufferL={4}",
                //                             _messagesReceived,
                //                             bandwithIn,
                //                             _messagesSent,
                //                             bandwithOut,
                //                             _bufferLength));
                //    _stopwatch.Stop();
                //    _stopwatch = Stopwatch.StartNew();
                //    _messagesSent = 0;
                //    _dataReceived = 0;
                //    _messagesReceived = 0;
                //}
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
        /// <param name="reader">The king buffer reader from received message.</param>
        private static void OnMessageReceived(IKingBufferReader reader)
        {
            try
            {
                _messagesReceived++;
                _dataReceived += reader.Length;
                _bufferLength = reader.Length;

                //if (_networkListenerType == NetworkListenerType.WS)
                //{
                //    Console.WriteLine($"OnMessageReceived: ");

                //    var bytes = kingBuffer.ReadAllBytes();

                //    string text = Encoding.UTF8.GetString(bytes);
                //    Console.WriteLine(text);
                //}
                //else
                //{
                //    _messagesReceived++;
                //    _dataReceived += kingBuffer.Length;
                //    _bufferLength = kingBuffer.Length;
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }
    }
}
