using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HUD : UI_Scene
{
    enum Texts
    {
        GoldText,
    }

    public override void Init()
    {
        base.Init();
        Bind<Text>(typeof(Texts));
    }

    public void UpdateGold(int amount)
    {
        Get<Text>((int)Texts.GoldText).text = amount.ToString("N0");
    }

}
