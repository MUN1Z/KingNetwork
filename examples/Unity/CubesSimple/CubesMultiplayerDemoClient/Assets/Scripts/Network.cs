using CubesMultiplayerDemoShared;
using KingNetwork.Client;
using KingNetwork.Shared;
using KingNetwork.Shared.Interfaces;
using System.Collections.Generic;
using System.Threading;
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
        client.DisconnectedHandler = OnDisconnected;
        client.Connect(ip);

        Thread.Sleep(15); // Delay for socket connection.

        if (client.HasConnected)
            Debug.Log("Client  started!");
        else
            Debug.LogError("Could not start client!");
    }

    private void Update()
    {
        if (client != null && client.HasConnected)
        {
            lastDistance = Vector3.Distance(lastNetworkedPosition, player.transform.position);
            if (lastDistance >= MIN_DISTANCE_TO_SEND_POSITION)
            {
                using (var kingBuffer = KingBufferWriter.Create())
                {
                    kingBuffer.Write(MyPackets.PlayerPosition);
                    kingBuffer.Write(player.transform.position.x);
                    kingBuffer.Write(player.transform.position.y);
                    kingBuffer.Write(player.transform.position.z);

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

    public void OnDisconnected() =>  Debug.Log($"Client disconnected from server.");

    public void OnMessageReceived(IKingBufferReader reader)
    {
        switch (reader.ReadMessagePacket<MyPackets>())
        {
            case MyPackets.PlayerPositionsArray:

                var lengthArr = reader.ReadInt32();

                Debug.Log($"Got positions array data num : {lengthArr}");

                for (int i = 0; i < lengthArr; i++)
                {
                    var playerid = reader.ReadUInt16();

                    if (!netPlayersDictionary.ContainsKey(playerid))
                        netPlayersDictionary.Add(playerid, new NetPlayer());

                    netPlayersDictionary[playerid].X = reader.ReadSingle();
                    netPlayersDictionary[playerid].Y = reader.ReadSingle();
                    netPlayersDictionary[playerid].Z = reader.ReadSingle();
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