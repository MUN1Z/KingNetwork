using CubesMultiplayerDemoShared;
using KingNetwork.Shared;
using UnityEngine;

public class RemotePlayer : MonoBehaviour
{
    public Player CurrentPlayer { get; set; }

    private Transform _transform;
    private Vector3 _position;
    private Vector3 _finalPosition;

    // Use this for initialization
    void Start()
    {
        NetworkManager.ClientInstance().PutHandler(MyPackets.PlayerPosition, PlayerData);

        _transform = gameObject.GetComponent<Transform>();
        _position = _transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(_position, _finalPosition) > 0.1f)
            _position = Vector3.Lerp(_position, _finalPosition, 0.5f * Time.deltaTime);
        else
            _position = _finalPosition;
    }

    void PlayerData(KingBuffer kingBuffer)
    {
        // TODO get from kingBuffer finalPosition = null;
    }
}
