using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_UserInfo : UI_Scene
{
    enum Texts
    {
        HpText,
        ExpText,
        LevelText
    }
    enum Images
    {
        HpFill,
        ExpFill
    }

    public override void Init()
    {
        base.Init();
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
    }
    public void ChangeLevel()
    {
        Get<Text>((int)Texts.LevelText).text = Managers.Object.MyPlayer.Stat.Level > 3 ? "MaxLevel" 
                                                                                 :  $"Lv.{Managers.Object.MyPlayer.Stat.Level}";
    }

    public void ChangeHp()
    {
        float ratio = Managers.Object.MyPlayer.Hp / (float)Managers.Object.MyPlayer.Stat.MaxHp;
        Get<Image>((int)Images.HpFill).fillAmount = ratio;
        Get<Text>((int)Texts.HpText).text = $"{Managers.Object.MyPlayer.Hp}/{Managers.Object.MyPlayer.Stat.MaxHp}";
    }
    public void ChangeExp()
    {
        float ratio = Managers.Object.MyPlayer.Stat.CurrentExp / (float)Managers.Object.MyPlayer.Stat.TotalExp;
        //Debug.Log(ratio);
        Get<Image>((int)Images.ExpFill).fillAmount = ratio;
        float percent = Mathf.Clamp(ratio, 0, 1);
        percent *=100f;
        Get<Text>((int)Texts.ExpText).text = percent.ToString("N2")+"%";
    }

}


