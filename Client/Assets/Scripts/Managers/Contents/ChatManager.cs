using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager
{

    Transform chatRoomGrid;
    Queue<GameObject> chatQueue = new Queue<GameObject>();
    Dictionary<int,GameObject> localChat = new Dictionary<int,GameObject>();
    public void SetChatRoomGrid(Transform grid)
    {
        chatRoomGrid = grid;
    }

    public void AddRoomChat(string playerName, string chat)
    {
        GameObject go = Managers.Resource.Instantiate("UI/Scene/UI_ChatText", chatRoomGrid);
        chatQueue.Enqueue(go);

        if(playerName.Equals(Managers.Network.PlayerName))
        {
            go.GetComponent<Text>().text = $"<color=green>{playerName}</color> | {chat}";
        }
        else
        {
            go.GetComponent<Text>().text = $"<color=cyan>{playerName}</color> | {chat}";
        }
        if (chatQueue.Count > 300)
        {
            var text = chatQueue.Dequeue();
            Managers.Resource.Destroy(text);
        }
    }

    public void AddLocalChat(int senderId, ChatInfo chatinfo)
    {
        GameObject player = Managers.Object.FindById(senderId);
        if(player == null) return;

        GameObject chatObj = null;

        localChat.TryGetValue(senderId,out chatObj);
        if(chatObj != null)
        {
            Managers.Resource.Destroy(chatObj);
            localChat[senderId] = null;
        }
        
        GameObject go = Managers.Resource.Instantiate("Creature/Chat");
        go.transform.position = player.transform.position + Vector3.up;
        go.transform.SetParent(player.transform);
        localChat[senderId] = go;
    }
}
