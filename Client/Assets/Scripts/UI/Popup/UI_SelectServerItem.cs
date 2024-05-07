using System.Collections;
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

    enum Images
    {
        BusyFill
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        GetButton((int)Buttons.SelectServerButton).gameObject.BindEvent(OnClickButton);
    }

    public void RefreshUI()
    {
        if(Info != null)
        {
            GetText((int)Texts.NameText).text = Info.Name;
            float ratio = Info.BusyScore / (float)10;;
            GetImage((int)Images.BusyFill).fillAmount = Mathf.Clamp(ratio,0.1f,1f);
            GetImage((int)Images.BusyFill).color = Color.Lerp(Color.green,Color.red,ratio);
        }

    }

    void OnClickButton(PointerEventData data)
    {
        Managers.Network.ConnectToGame(Info);
        Managers.Scene.LoadScene(Define.Scene.Game);
        Managers.UI.ClosePopupUI();
    }
}
