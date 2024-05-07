using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inventory_Item : UI_Base
{
    [SerializeField]
    Image _icon = null;
    [SerializeField]
    Image _frame = null;

    public int ItemDbId { get; private set; }
    public int TemplateId { get; private set; }
    public int Count { get; private set; }
    public bool Equipped { get; private set; }
    public string ItemName {get; private set;}

    public override void Init()
    {
        _icon.gameObject.BindEvent((e) =>
        {
            Debug.Log("Click Item");

            Data.ItemData itemData = null;
            Managers.Data.ItemDict.TryGetValue(TemplateId, out itemData);
            if (itemData == null)
                return;

            // TODO USE Item
            if (itemData.itemType == ItemType.Consumable)
            { return; }

            C_EquipItem equipPacket = new C_EquipItem();
            equipPacket.ItemDbId = ItemDbId;
            equipPacket.Equipped = !Equipped;

            Managers.Network.Send(equipPacket);
        });

        _icon.gameObject.BindEvent(ResisterItem, type: Define.UIEvent.Click, isLeftClick: false);

    }

    public void ResisterItem(PointerEventData data)
    {
        if(_icon == null)
            return;
        C_MarketItemResister packet = new C_MarketItemResister()
        {
            ItemDbId = this.ItemDbId,
            TemplateId = this.TemplateId,
            Price = 0,
            SellerId = Managers.Network.PlayerDbId,
            ItemName = this.ItemName,
        };

        Managers.Network.Send(packet);

        Debug.Log($"{ItemDbId} Register...");
    }
    public void SetItem(Item item)
    {
        if (item == null)
        {
            ItemDbId = 0;
            TemplateId = 0;
            Count = 0;
            Equipped = false;
            _icon.gameObject.SetActive(false);
            _frame.gameObject.SetActive(false);
        }
        else
        {
            ItemDbId = item.ItemDbId;
            TemplateId = item.TemplateId;
            Count = item.Count;
            Equipped = item.Equipped;

            Data.ItemData itemData = null;
            Managers.Data.ItemDict.TryGetValue(TemplateId, out itemData);

            ItemName = itemData.name;

            Sprite icon = Managers.Resource.Load<Sprite>(itemData.iconPath);
            _icon.sprite = icon;

            _icon.gameObject.SetActive(true);
            _frame.gameObject.SetActive(Equipped);
        }
    }
}
