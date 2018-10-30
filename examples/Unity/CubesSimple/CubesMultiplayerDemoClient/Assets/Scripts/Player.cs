using UnityEngine;

public class Player : MonoBehaviour {

    private readonly float moveSpeed = 5f;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var vertical = Input.GetAxis("Vertical");
        var horizontal = Input.GetAxis("Horizontal");
        
        transform.Translate(moveSpeed * horizontal * Time.deltaTime, 0f, moveSpeed * vertical * Time.deltaTime);
    }
}
