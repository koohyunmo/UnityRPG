using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginScene : BaseScene
{
    UI_LoginScene _sceneUI;
    protected override void Init()
    {
        base.Init();

        // TODO 잠시 기생
        SceneType = Define.Scene.Lobby;
        Managers.Web.BaseUrl = "https://localhost:5001/api";

        Screen.SetResolution(640,480,false);

        _sceneUI = Managers.UI.ShowSceneUI<UI_LoginScene>();

    }

    public override void Clear()
    {
        
    }
}
