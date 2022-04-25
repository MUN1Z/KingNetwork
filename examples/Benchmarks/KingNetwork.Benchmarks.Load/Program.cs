using System;
using System.Threading;

namespace KingNetwork.Benchmarks.Load
{
    /// <summary>
    /// This class represents the program instance.
    /// </summary>
    class Program
    {
        /// <summary>
        /// This method is responsible for main execution of console application.
        /// </summary>
        /// <param name="args">The string args received by parameters.</param>
        public static void Main(string[] args)
        {
            Thread serverThread = new Thread(() =>
            {
                RunServer.StartServerAsync();
            });

            serverThread.IsBackground = false;
            serverThread.Start();
            
            RunClients.StartClients("127.0.0.1", 10000);

            Console.ReadLine();
        }
    }
}
