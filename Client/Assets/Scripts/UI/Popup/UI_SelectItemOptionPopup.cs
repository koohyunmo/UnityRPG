using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SelectItemOptionPopup : UI_Popup
{
    enum Buttons
    {
        EquipButton,
        SellButton,
        DropButton
    }
    enum Images
    {
        BG
    }

    enum Texts
    {
        UseOrEquipText,
    }


    public GameObject popup;
    Item _itemData;

    private Action<PointerEventData> _equipEvt = null;

    private Action<PointerEventData> _sellEvt = null;

    private Action<PointerEventData> _dropEvt = null;

    public void SetItemData(Item itemData, Action<PointerEventData> equipOrUseCallback, Action<PointerEventData> sellCallback, Action<PointerEventData> dropCallback)
    {
        _itemData = new Item(itemData);

        _equipEvt = null;
        _equipEvt = equipOrUseCallback;

        _sellEvt = null;
        _sellEvt = sellCallback;

        _dropEvt = null;
        _dropEvt = dropCallback;

        Get<Button>((int)Buttons.EquipButton).gameObject.BindEvent(p => {_equipEvt?.Invoke(p); Managers.UI.ClosePopupUI();});
        Get<Button>((int)Buttons.SellButton).gameObject.BindEvent(p => {_sellEvt?.Invoke(p); Managers.UI.ClosePopupUI();});
        Get<Button>((int)Buttons.DropButton).gameObject.BindEvent(p => {_dropEvt?.Invoke(p); Managers.UI.ClosePopupUI();});

        if(_itemData.ItemType != Google.Protobuf.Protocol.ItemType.Consumable)
        {
            Get<Button>((int)Buttons.EquipButton).gameObject.BindEvent(p => { _equipEvt?.Invoke(p); Managers.UI.ClosePopupUI(); });
            Get<Text>((int)Texts.UseOrEquipText).text = "장착하기";
        }
        else
        {
            Get<Button>((int)Buttons.EquipButton).gameObject.BindEvent(p => {Debug.Log("USE"); Managers.UI.ClosePopupUI(); });
            Get<Text>((int)Texts.UseOrEquipText).text = "사용하기";
        }
    }
    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));

        Get<Image>((int)Images.BG).gameObject.BindEvent( p => Managers.UI.ClosePopupUI());
        Get<Image>((int)Images.BG).gameObject.BindEvent(p => Managers.UI.ClosePopupUI(),isLeftClick : false);
    }
}
