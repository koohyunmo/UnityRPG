using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ServiceContent : UI_Scene
{
    enum Images
    {
        MailIcon,
        MarketIcon,
    }
    public override void Init()
    {
        Bind<Image>(typeof(Images));

        Get<Image>((int)Images.MailIcon).gameObject.BindEvent(p => Managers.UI.ShowPopupUI<UI_MailPopup>());
        Get<Image>((int)Images.MarketIcon).gameObject.BindEvent(p => Managers.UI.ShowPopupUI<UI_Market>());
    }
}
