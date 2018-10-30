using UnityEngine;

public class NetPlayer
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public GameObject GameObject { get; set; }

    public bool GameObjectAdded { get; set; }

    public NetPlayer()
    {
        GameObjectAdded = false;
    }

    public Vector3 Position => new Vector3(X, Y, Z);
}
