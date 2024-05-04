using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Minimap : UI_Scene
{
    public Camera miniMapCam;
    public Text MyplayerPosInfo;
    enum Texts
    {
        MyPlayerPosInfoText
    }
    public override void Init()
    {
        if(miniMapCam == null)
        {
            miniMapCam = transform.Find("MiniMapCam").GetComponent<Camera>();
        }

        Bind<Text>(typeof(Texts));

        MyplayerPosInfo = Get<Text>((int)Texts.MyPlayerPosInfoText);
    }

    private void LateUpdate() 
    {
        if(Managers.Object.MyPlayer == null) return;
        var pos = Managers.Object.MyPlayer.transform.position;
        pos.z = miniMapCam.transform.position.z;
        miniMapCam.transform.position = pos;

        MyplayerPosInfo.text = $"현재맵 : {Managers.Map.MapId} 좌표 (X:{Managers.Object.MyPlayer.PosInfo.PosX}, Y:{Managers.Object.MyPlayer.PosInfo.PosY})";
    }
}
