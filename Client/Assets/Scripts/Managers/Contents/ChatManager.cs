using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;

public class ChatManager
{

    Transform chatRoomGrid;
    Queue<GameObject> chatQueue = new Queue<GameObject>();
    Dictionary<int, BallonChat> ballonChat = new Dictionary<int, BallonChat>();
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

    public void AddBallonChat(int senderId,int chatId, ChatInfo chatinfo)
    {
        GameObject target = Managers.Object.FindById(senderId);
        if(target == null) return;
        GameObject go = Managers.Resource.Instantiate("Chat/BallonChat");
        if(go == null) return;
        CreatureController cc = target.GetComponent<CreatureController>();
        if(cc == null) return;

        ClearPrevBallonChat(senderId);

        var bc = go.GetComponent<BallonChat>();
        bc.OnDialog(chatinfo.Chat);
        bc.SetChatId(chatId);

        //go.transform.position = Managers.Map.CurrentGrid.CellToWorld(cc.CellPos) + Vector3.up + new Vector3(0.5f, 0.5f);
        go.transform.position = target.transform.position + Vector3.up + new Vector3(0.5f, 0.5f);
        go.transform.SetParent(target.transform);
        ballonChat[senderId] = bc;
    }

    private void ClearPrevBallonChat(int senderId)
    {
        BallonChat bc = null;
        ballonChat.TryGetValue(senderId, out bc);
        if (bc != null)
        {
            Managers.Resource.Destroy(bc.gameObject);
            ballonChat.Remove(senderId);
        }
    }
    public void ClearBallonChat(int senderId, int chatId)
    {
        BallonChat bc = null;
        ballonChat.TryGetValue(senderId, out bc);
        if (bc != null && bc.chatId == chatId)
        {
            Managers.Resource.Destroy(bc.gameObject);
            ballonChat.Remove(senderId);
        }
    }
}
