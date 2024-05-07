using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class UI_OptionPopup : UI_Popup
{
    enum Images
    {
        BG
    }

    enum Buttons
    {
        CloseButton,
        LobbyButton,
        ExitButton
    }

    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));

        Get<Button>((int)Buttons.LobbyButton).gameObject.BindEvent((p) => { LobbyGame();});
        Get<Button>((int)Buttons.ExitButton).gameObject.BindEvent((p) => { ExitGame();});
        Get<Button>((int)Buttons.CloseButton).gameObject.BindEvent((p) => Managers.UI.ClosePopupUI());
        Get<Image>((int)Images.BG).gameObject.BindEvent((p) => Managers.UI.ClosePopupUI());
    }

    private void ExitGame()
    {
        C_ExitGame c_ExitGame= new C_ExitGame();
        c_ExitGame.Exit = true;
        Managers.Network.Send(c_ExitGame);
        Application.Quit();
    }

    private void LobbyGame()
    {
        C_ExitGame c_ExitGame = new C_ExitGame();
        c_ExitGame.Exit = false;
        Managers.Network.Send(c_ExitGame);
        Managers.Scene.LoadScene(Define.Scene.Login);
    }


}
