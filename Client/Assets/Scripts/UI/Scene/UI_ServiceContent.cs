using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ServiceContent : UI_Scene
{
    enum Images
    {
        MailIcon,
        MarketIcon,
        InvenIcon,
        StatIcon,
        OptionIcon,

    }
    public override void Init()
    {
        Bind<Image>(typeof(Images));

        Get<Image>((int)Images.MailIcon).gameObject.BindEvent(p => Managers.UI.ShowPopupUI<UI_MailPopup>());
        Get<Image>((int)Images.MarketIcon).gameObject.BindEvent(p => Managers.UI.ShowPopupUI<UI_Market>());
        Get<Image>((int)Images.InvenIcon).gameObject.BindEvent(ClickInven);
        Get<Image>((int)Images.StatIcon).gameObject.BindEvent(ClickStat);
        Get<Image>((int)Images.OptionIcon).gameObject.BindEvent(p => Managers.UI.ShowPopupUI<UI_OptionPopup>());
    }

    public void ClickInven(PointerEventData p)
    {
        UI_GameScene gameScene = Managers.UI.SceneUI as UI_GameScene;
        gameScene.InvenUI.gameObject.SetActive(!gameScene.InvenUI.gameObject.activeSelf);
    }

    public void ClickStat(PointerEventData p)
    {
        UI_GameScene gameScene = Managers.UI.SceneUI as UI_GameScene;
        gameScene.StatUI.gameObject.SetActive(!gameScene.StatUI.gameObject.activeSelf);

    }
}
