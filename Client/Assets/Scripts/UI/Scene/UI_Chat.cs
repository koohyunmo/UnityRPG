using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Chat : UI_Scene
{
    enum Inputs
    {
        InputField,
    }
    enum Buttons
    {
        SendButton
    }

    enum GameObjects
    {
        ChatRoom,
        InputField
    }

    InputField inputField;
    public override void Init()
    {
        base.Init();

        Bind<InputField>(typeof(Inputs));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        Get<Button>((int)Buttons.SendButton).gameObject.BindEvent(OnClickSendButton);
        inputField = Get<InputField>((int)Inputs.InputField);
        inputField.gameObject.BindEvent((p) =>
        {
            Managers.Object.MyPlayer.PlayerInputState = MyPlayerController.PlayerInputStates.ChatState;
            Debug.Log("Chat Mode");
        });
        inputField.text = "";
        inputField.characterLimit = 50;  // 입력 필드의 최대 글자 수를 50으로 설정

        Managers.Chat.SetChatRoomGrid(Get<GameObject>((int)GameObjects.ChatRoom).transform);
    }

    public void OnClickSendButton(PointerEventData data)
    {
        if(inputField == null )return;
        if(inputField.text.Equals("")) return;
        C_Chat c_Chat= new C_Chat();
        c_Chat.Message = inputField.text;
        Managers.Network.Send(c_Chat);
        inputField.text = "";
    }
}
