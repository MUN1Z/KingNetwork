using CubesMultiplayerDemoShared;
using KingNetwork.Client;
using KingNetwork.Shared;
using UnityEngine;

public class RemotePlayer : MonoBehaviour
{

    public bool Active { get; set; }
    public Player CurrentPlayer { get; set; }

    private Transform _transform;
    private Vector3 _position;
    private Quaternion _rotation;
    private Vector3 _finalPosition;
    private static Vector3 _finalRotation;

    // Use this for initialization
    void Start()
    {
        NetworkManager.ClientInstance().PutHandler(MyPackets.PlayerPosition, PlayerData);

        _transform = gameObject.GetComponent<Transform>();
        _position = _transform.position;
        _rotation = _transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void PlayerData(KingBuffer kingBuffer)
    {
        // TODO get from kingBuffer finalPosition = null;
        // TODO get from kingBuffer finalRotation = null;
    }
}
