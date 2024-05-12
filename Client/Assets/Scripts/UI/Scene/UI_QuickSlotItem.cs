using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuickSlotItem : UI_Base, ISlot
{
    public SlotType SlotType { get; set;} = SlotType.QuickSlotItem;
    public string QuickSlotKeyCode {get; private set;}
    private Image _icon;
    private Image _coolTimeImage;
    private Item _itemData;
    private Text _countText;

    public Transform GetTransform()
    {
        return transform;
    }

    public override void Init()
    {
        _icon = gameObject.GetComponent<Image>();
    }
    public void SetInfo(string key, Text text)
    {
        QuickSlotKeyCode = key;
        _countText = text;

        _icon.color = new Color(0,0,0,0);
        _countText.gameObject.SetActive(false);
    }

    public void SetIcon(Sprite sp,Item itemData)
    {
        _icon.sprite = sp;
        _itemData = new Item(itemData);
        _countText.text = "x" +_itemData.Count;

        _icon.color = new Color(1,1, 1,1);
        _countText.gameObject.SetActive(true);

        Managers.QuickSlot.AddQuickSlotItme(itemData.ItemDbId,this);
    }

    public void RefreshItem()
    {
        if(Managers.Inven.Items[_itemData.ItemDbId] == null || Managers.Inven.Items[_itemData.ItemDbId].Count == 0)
        {
            _icon.color = new Color(0, 0, 0, 0);
            _countText.gameObject.SetActive(false);
        }
        else
        {
            _icon.color = new Color(1, 1, 1, 1);
            _countText.gameObject.SetActive(true);
            _countText.text = "x" + Managers.Inven.Get(_itemData.ItemDbId).Count.ToString();
            //Debug.Log(Managers.Inven.Items[_itemData.ItemDbId].Count);
        }
    }

}
