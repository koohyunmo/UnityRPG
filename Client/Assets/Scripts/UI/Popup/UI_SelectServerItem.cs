﻿using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SelectServerItem : UI_Base
{
    public ServerInfo Info { get; set; }
    enum Buttons
    {
        SelectServerButton
    }
    enum Texts
    {
        NameText
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        GetButton((int)Buttons.SelectServerButton).gameObject.BindEvent(OnClickButton);
    }

    public void RefreshUI()
    {
        if(Info != null)
        {
            GetText((int)Texts.NameText).text = Info.Name;
        }

    }

    void OnClickButton(PointerEventData data)
    {
        Managers.Network.ConnectToGame(Info);
        Managers.Scene.LoadScene(Define.Scene.Game);
        Managers.UI.ClosePopupUI();
    }
}
