using CubesMultiplayerDemoShared;
using KingNetwork.Client;
using KingNetwork.Shared;
using System.Collections.Generic;
using UnityEngine;

public class Network : MonoBehaviour {

    public enum ServerUpdateSpeed : int
    {
        SuperFast,
        Fast,
        Normal,
        Slow,
        SuperSlow
    }

    [Header("Remote Options")]
    public ServerUpdateSpeed serverUpdateSpeed;

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

    private float updateFrequency;

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
        if (updateFrequency <= 0f && client != null && client.HasConnected)
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
        
        if (updateFrequency > 0f)
            updateFrequency -= Time.deltaTime;
        else
            updateFrequency = (float)serverUpdateSpeed * 0.1f;
    }

    public void OnMessageReceived(KingBuffer kingBuffer)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => 
        {
            switch (kingBuffer.ReadMessagePacket<MyPackets>())
            {
                case MyPackets.PlayerPositionsArray:

                    var lengthArr = kingBuffer.ReadInteger();

                    Debug.Log($"Got positions array data num : {lengthArr}");

                    for (int i = 0; i < lengthArr; i++)
                    {
                        var playerid = kingBuffer.ReadInteger();

                        Debug.Log($"PlayerId : {playerid}");

                        if (!netPlayersDictionary.ContainsKey(playerid))
                            netPlayersDictionary.Add(playerid, new NetPlayer());

                        netPlayersDictionary[playerid].X = kingBuffer.ReadFloat();
                        netPlayersDictionary[playerid].Y = kingBuffer.ReadFloat();
                        netPlayersDictionary[playerid].Z = kingBuffer.ReadFloat();

                        if (netPlayersDictionary[playerid].X != 0)
                            netPlayersDictionary[playerid].GameObject = Instantiate(netPlayerPrefab, netPlayerPrefab.transform.position, Quaternion.identity);
                        else
                            netPlayersDictionary[playerid].GameObject = Instantiate(netPlayerPrefab, netPlayersDictionary[playerid].NewPosition, Quaternion.identity);
                    }

                    break;

                case MyPackets.PlayerPosition:

                    Debug.Log($"Got player position");

                    var id = kingBuffer.ReadInteger();

                    if (!netPlayersDictionary.ContainsKey(id))
                    {
                        netPlayersDictionary.Add(id, new NetPlayer());
                        
                        netPlayersDictionary[id].GameObject = Instantiate(netPlayerPrefab, netPlayerPrefab.transform.position, Quaternion.identity);
                    }

                    netPlayersDictionary[id].X = kingBuffer.ReadFloat();
                    netPlayersDictionary[id].Y = kingBuffer.ReadFloat();
                    netPlayersDictionary[id].Z = kingBuffer.ReadFloat();

                    netPlayersDictionary[id].GameObject.transform.position = netPlayersDictionary[id].NewPosition;

                    //netPlayersDictionary[id].GameObject.transform.position = Vector3.Lerp(
                    //                                                                        netPlayersDictionary[id].OldPosition,
                    //                                                                        netPlayersDictionary[id].NewPosition,
                    //                                                                        5f * Time.deltaTime);
                    break;
            }
        });
    }

    private void OnApplicationQuit()
    {
        if (client != null)
            if (client.HasConnected)
                client.Disconnect();
    }
}
