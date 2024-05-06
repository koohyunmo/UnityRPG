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
    
    public void SetItem(MarketItem marketItem, Action action = null)
    {
        _notify = null;
        _notify = action;

        _marketItem.SellerId = marketItem.SellerId;
        _marketItem.ItemDbId = marketItem.ItemDbId;
        _marketItem.Price = marketItem.Price;
        _marketItem.SellerName = marketItem.SellerName;

        Data.ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(marketItem.TemplateId, out itemData);
     
        Get<Text>((int)Texts.UI_ItemNameText).text = itemData.name;
        Get<Text>((int)Texts.UI_ItemPriceText).text = marketItem.Price.ToString("N0");
        Get<Text>((int)Texts.UI_ItemOwnerText).text = marketItem.SellerName.ToString();

        Sprite icon = Managers.Resource.Load<Sprite>(itemData.iconPath);
        Get<Image>((int)Images.UI_Icon).sprite = icon;


    }
}
