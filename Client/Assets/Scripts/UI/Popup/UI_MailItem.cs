using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MailItem : UI_Base
{
    enum Texts
    {
        UI_ItemNameText,
        UI_ItemSellerText
    }

    enum Buttons
    {
        UI_ItemReceiveButton
    }

    enum Images
    {
        UI_Icon,
    }

    private Action _notify = null;
    MailItem _mail = new MailItem();


    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));

        Get<Button>((int)Buttons.UI_ItemReceiveButton).gameObject.BindEvent(OnClickReceiveButton);
    }

    public void OnClickReceiveButton(PointerEventData data)
    {
        C_MailItemReceive receive = new C_MailItemReceive();
        receive.MailItem = new MailItem();
        receive.MailItem.MergeFrom(_mail);
        Managers.Network.Send(receive);
    }

    public void SetItem(MailItem mail, Action action = null)
    {
        _notify = null;
        _notify = action;

        _mail.MergeFrom(mail);

        Data.ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(_mail.TemplateId, out itemData);

        Get<Text>((int)Texts.UI_ItemNameText).text = itemData.name;
        Get<Text>((int)Texts.UI_ItemSellerText).text = _mail.SellerName;

        Sprite icon = Managers.Resource.Load<Sprite>(itemData.iconPath);
        Get<Image>((int)Images.UI_Icon).sprite = icon;


    }
}
