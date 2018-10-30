using CubesMultiplayerDemoShared;
using KingNetwork.Client;
using KingNetwork.Shared;
using System.Collections.Generic;
using UnityEngine;

public class Network : MonoBehaviour
{

    public Player player;
    private Vector3 lastNetworkedPosition = Vector3.zero;

    private float lastDistance = 0.0f;
    const float MIN_DISTANCE_TO_SEND_POSITION = 0.01f;

    private KingClient client;

    public GameObject netPlayerPrefab;
    private Dictionary<int, NetPlayer> netPlayersDictionary;

    public string remoteIp;
    public string localIP;

    public string ip;

    // Use this for initialization
    void Start()
    {
        netPlayersDictionary = new Dictionary<int, NetPlayer>();

        client = new KingClient();
        client.MessageReceivedHandler = OnMessageReceived;
        client.Connect(ip);

        if (client.HasConnected)
            Debug.Log("Client  started!");
        else
            Debug.LogError("Could not start client!");
    }

    private void FixedUpdate()
    {
        if (client != null && client.HasConnected)
        {
            lastDistance = Vector3.Distance(lastNetworkedPosition, player.transform.position);
            if (lastDistance >= MIN_DISTANCE_TO_SEND_POSITION)
            {
                using (var kingBuffer = new KingBuffer())
                {
                    kingBuffer.WriteMessagePacket(MyPackets.PlayerPosition);
                    kingBuffer.WriteFloat(player.transform.position.x);
                    kingBuffer.WriteFloat(player.transform.position.y);
                    kingBuffer.WriteFloat(player.transform.position.z);

                    client.SendMessage(kingBuffer);

                    lastNetworkedPosition = player.transform.position;
                }
            }
        }

        if (netPlayersDictionary != null)
        {
            foreach (var player in netPlayersDictionary)
            {
                if (!player.Value.GameObjectAdded)
                {
                    player.Value.GameObjectAdded = true;
                    player.Value.GameObject = Instantiate(netPlayerPrefab, player.Value.Position, Quaternion.identity);
                }
                else
                    player.Value.GameObject.transform.position = player.Value.Position;
            }
        }
    }

    public void OnMessageReceived(KingBuffer kingBuffer)
    {
        switch (kingBuffer.ReadMessagePacket<MyPackets>())
        {
            case MyPackets.PlayerPositionsArray:

                var lengthArr = kingBuffer.ReadInteger();

                Debug.Log($"Got positions array data num : {lengthArr}");

                for (int i = 0; i < lengthArr; i++)
                {
                    var playerid = kingBuffer.ReadInteger();

                    if (!netPlayersDictionary.ContainsKey(playerid))
                        netPlayersDictionary.Add(playerid, new NetPlayer());

                    netPlayersDictionary[playerid].X = kingBuffer.ReadFloat();
                    netPlayersDictionary[playerid].Y = kingBuffer.ReadFloat();
                    netPlayersDictionary[playerid].Z = kingBuffer.ReadFloat();
                }

                break;
        }
    }

    private void OnApplicationQuit()
    {
        if (client != null)
            if (client.HasConnected)
                client.Disconnect();
    }
}