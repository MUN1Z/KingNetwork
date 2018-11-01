using KingNetwork.Client;
using UnityEngine;

namespace Assets.Scripts
{
    public class NetworkManager : MonoBehaviour
    {
        private static KingClient client;

        public string remoteIp;
        public string localIP;

        public string ip;

        public static KingClient GetClient() => client;

        // Use this for initialization
        void Start()
        {
            client = new KingClient();
            client.Connect(ip);

            if (client.HasConnected)
                Debug.Log("Client  started!");
            else
                Debug.LogError("Could not start client!");
        }

        private void FixedUpdate()
        {

        }
        
        private void OnApplicationQuit()
        {
            if (client != null)
                if (client.HasConnected)
                    client.Disconnect();
        }
    }
}