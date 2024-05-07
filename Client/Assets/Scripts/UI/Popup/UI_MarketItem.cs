using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MarketItem : UI_Base
{

    MarketItem _marketItem = new MarketItem();
    Action _notify;
    enum Texts
    {
        UI_ItemNameText,
        UI_ItemPriceText,
        UI_ItemOwnerText
    }

    enum Buttons
    {
        BuyButton,
        DeleteItemButton
    }

    enum Images
    {
        UI_Icon,
    }
    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));

        Get<Button>((int)Buttons.BuyButton).gameObject.BindEvent(ClickBuyButton);
        Get<Button>((int)Buttons.DeleteItemButton).gameObject.BindEvent(ClickDeleteButton);
    }

    public void ClickBuyButton(PointerEventData data)
    {
        C_MarketItemPurchase packet = new C_MarketItemPurchase();

        packet.ItemDbId = _marketItem.ItemDbId;
        packet.SellerId = _marketItem.SellerId;
        packet.BuyerId = Managers.Network.PlayerDbId;
        packet.Price = _marketItem.Price;

        Managers.Network.Send(packet);
    }

    public void ClickDeleteButton(PointerEventData data)
    {
        C_MarketItemDelete packet = new C_MarketItemDelete(){Item = new MarketItem()};

        packet.Item.MergeFrom(_marketItem);

        Managers.Network.Send(packet);
    }


    public void SetItem(MarketItem marketItem, Action action = null)
    {
        _notify = null;
        _notify = action;

        _marketItem.MergeFrom(marketItem);

        Data.ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(marketItem.TemplateId, out itemData);

        Get<Text>((int)Texts.UI_ItemNameText).text = itemData.name;
        Get<Text>((int)Texts.UI_ItemPriceText).text = _marketItem.Price.ToString("N0");
        Get<Text>((int)Texts.UI_ItemOwnerText).text = _marketItem.SellerName.ToString();

        Sprite icon = Managers.Resource.Load<Sprite>(itemData.iconPath);
        Get<Image>((int)Images.UI_Icon).sprite = icon;


        if (_marketItem.SellerId == Managers.Network.PlayerDbId || _marketItem.SellerName.Equals(Managers.Network.PlayerName))
        {
            Get<Button>((int)Buttons.BuyButton).gameObject.SetActive(false);
        }
        else
        {
            Get<Button>((int)Buttons.DeleteItemButton).gameObject.SetActive(false);
        }



    }
}
