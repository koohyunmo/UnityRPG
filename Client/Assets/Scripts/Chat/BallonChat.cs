using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BallonChat : MonoBehaviour
{
    [SerializeField]GameObject bg;
    [SerializeField]Text text;

    Queue<string> sentences = new Queue<string>();
    public int chatId {get; private set;}

    public void OnDialog(string message)
    {

        // 메시지가 50자 이상이면 처음 50자만 가져오기
        if (message.Length > 50)
        {
            message = message.Substring(0, 50);
        }

        text.text = message;

        var x = Math.Min(6, text.preferredWidth);
        var y = Math.Min(6, text.preferredHeight);

        bg.transform.localScale = new Vector3(x, y, 0);
        transform.localPosition = new Vector3(0,1.25f,0);
    }

    public void SetChatId(int chatId)
    {
        this.chatId = chatId;
    }
}
