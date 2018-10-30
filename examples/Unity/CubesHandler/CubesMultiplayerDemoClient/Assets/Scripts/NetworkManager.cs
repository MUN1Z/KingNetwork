using KingNetwork.Client;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private static KingClient _clientInstance;
    
    public string remoteIp;
    public string localIP;

    public string ip;

    public static KingClient ClientInstance()
    {
        if(_clientInstance == null)
            _clientInstance = new KingClient();

        return _clientInstance;
    }

    // Use this for initialization
    void Start()
    {
        ClientInstance().Connect(ip);

        if (ClientInstance().HasConnected)
            Debug.Log("Client  started!");
        else
            Debug.LogError("Could not start client!");
    }

    private void FixedUpdate()
    {
        
    }
    
    private void OnApplicationQuit()
    {
        if (ClientInstance() != null)
            if (ClientInstance().HasConnected)
                ClientInstance().Disconnect();
    }
}
