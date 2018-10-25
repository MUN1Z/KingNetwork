using KingNetwork.Client;
using System;

namespace KingNetwork.Example.TestClient
{
    /// <summary>
    /// This class represents the program instance.
    /// </summary>
    class Program
    {
        /// <summary>
        /// This method is responsible for main execution of console application.
        /// </summary>
        /// <param name="args">The string args receiveds by parameters.</param>
        static void Main(string[] args)
        {
            try
            {
                var client = new KingClient();
                client.Connect("127.0.0.1", 7171);

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
