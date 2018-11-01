using Assets.Scripts;
using ChatMultiplayerDemoShared;
using KingNetwork.Client;
using KingNetwork.Shared;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ChatDemo : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The InputField the user can type in.")]
    InputField input;

    [SerializeField]
    [Tooltip("The transform to place new messages in.")]
    Transform chatWindow;

    [SerializeField]
    [Tooltip("The scrollrect for the chat window (if present).")]
    ScrollRect scrollRect;

    [SerializeField]
    [Tooltip("The message prefab where messages will be added.")]
    GameObject messagePrefab;
    
    private void Start()
    {
        Thread.Sleep(100);
        NetworkManager.GetClient().PutHandler(MyPackets.Message, OnMessageReceived);
    }

    public void OnMessageReceived(IKingBuffer kingBuffer)
    {
        AddMessage(kingBuffer.ReadString());
    }
    
    void AddMessage(string message)
    {
        var messageObj = Instantiate(messagePrefab) as GameObject;
        messageObj.transform.SetParent(chatWindow);
        
        var text = messageObj.GetComponentInChildren<Text>();
        
        if (text != null)
            text.text = message;
        else
            Debug.LogError("Message object does not contain a Text component!");

        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
    
    public void MessageEntered()
    {
        using (var kingBuffer = new KingBuffer())
        {
            kingBuffer.WriteMessagePacket(MyPackets.Message);
            kingBuffer.WriteString(input.text);

            NetworkManager.GetClient().SendMessage(kingBuffer);
        }
    }
}
