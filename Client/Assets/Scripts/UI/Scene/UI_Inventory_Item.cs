using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inventory_Item : UI_Base, ISlot
{
    [SerializeField]
    Image _icon = null;
    [SerializeField]
    Image _frame = null;
    [SerializeField]
    Text itemCountText;
    UI_Inventory _inventory;
    public Item ItemData { get; private set; }
    public int ItemDbId { get; private set; } = -1;
    public string ItemName { get; private set; }
    public int Slot {get; set;} = -1;
    public SlotType SlotType { get; set; } = SlotType.InventoryItem;

    bool isDrag;

    public override void Init()
    {
        _icon.gameObject.BindEvent(EquipItem, Define.UIEvent.DoubleClick);
        _icon.gameObject.BindEvent(ItemDrag, Define.UIEvent.Drag);
        _icon.gameObject.BindEvent(ItemPointUp, Define.UIEvent.PointUp);
        _icon.gameObject.BindEvent(ShowItemOptionPopup, type: Define.UIEvent.Click, isLeftClick: false);
        itemCountText.gameObject.SetActive(false);
    }

    private void ItemDrag(PointerEventData data)
    {
        if (ItemDbId == -1)
            return;
        if (Slot == -1)
            return;

        isDrag = true;
        _icon.gameObject.transform.position = new Vector3(data.position.x, data.position.y, 0);
    }

    private void ItemPointUp(PointerEventData data)
    {
        if (ItemDbId == -1)
            return;
        if (Slot == -1)
            return;

        if (isDrag)
        {
            UI_Inventory_Item swapTaget = FindSlotItem();

            if (swapTaget)
            {
                Swap(swapTaget, this);
            }
            else
            {
                _icon.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            }
            isDrag = false;
        }
    }

    public void EquipItem(PointerEventData data)
    {
        if (isDrag) return;

        Data.ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(ItemData.TemplateId, out itemData);
        if (itemData == null)
            return;

        // TODO USE Item
        if (itemData.itemType == ItemType.Consumable)
        { return; }

        // TODO 슬롯바뀐걸 서버 적용해줘야함 지금은 로컬에서만 슬롯이 변경됨
        C_EquipItem equipPacket = new C_EquipItem();
        equipPacket.ItemDbId = ItemDbId;
        equipPacket.Equipped = !ItemData.Equipped;

        Debug.Log("아이템 장착");

        Managers.Network.Send(equipPacket);
    }

    public void ShowItemOptionPopup(PointerEventData data)
    {
        if (_icon == null)
            return;

        var itemPopup = Managers.UI.ShowPopupUI<UI_SelectItemOptionPopup>();
        Vector3 offset = new Vector3(100f, -50f, 0); // X 축으로 -100, Y 축으로 -50 이동
        itemPopup.popup.transform.position = new Vector3(data.position.x, data.position.y, 0) + offset;
        itemPopup.SetItemData(ItemData, EquipItem, Resister, Drop);
    }

    private void Resister(PointerEventData data)
    {
        C_MarketItemResister packet = new C_MarketItemResister()
        {
            ItemDbId = this.ItemDbId,
            TemplateId = this.ItemData.TemplateId,
            Price = 100,
            SellerId = Managers.Network.PlayerDbId,
            ItemName = this.ItemName,
        };

        Managers.Network.Send(packet);

        Debug.Log($"{ItemDbId} Register...");
    }

    private void Drop(PointerEventData data)
    {
        if (ItemDbId == -1) return;
        C_ItemDelete c_ItemDelete = new C_ItemDelete();
        c_ItemDelete.ItemDbId = this.ItemDbId;
        Managers.Network.Send(c_ItemDelete);
        Debug.Log("아이템 삭제");
    }
    public void SetItem(Item item,UI_Inventory inventory)
    {
        if (item == null)
        {
            ItemData = null;
            ItemDbId = -1;
            //_icon.sprite = null;
            _icon.gameObject.SetActive(false);
            _frame.gameObject.SetActive(false);
            itemCountText.gameObject.SetActive(false);
            _icon.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
        else
        {
            ItemData = new Item(item);

            ItemDbId = item.ItemDbId;
            ItemData.TemplateId = item.TemplateId;
            ItemData.Count = item.Count;
            ItemData.Equipped = item.Equipped;

            Data.ItemData itemData = null;
            Managers.Data.ItemDict.TryGetValue(ItemData.TemplateId, out itemData);

            ItemName = itemData.name;

            Sprite icon = Managers.Resource.Load<Sprite>(itemData.iconPath);
            _icon.sprite = icon;
            _icon.transform.position = new Vector3(transform.position.x, transform.position.y, 0);

            _icon.gameObject.SetActive(true);
            _frame.gameObject.SetActive(ItemData.Equipped);

            itemCountText.text = "X" + item.Count.ToString();
            itemCountText.gameObject.SetActive(true);
        }

        _inventory = inventory;
    }

    public void Swap(UI_Inventory_Item swapItem, UI_Inventory_Item sellectedItem)
    {
        Item tempItem = null;
        if (swapItem.ItemDbId != -1) tempItem = new Item(swapItem.ItemData);

        C_ItemSlotChange c_ItemSlotChange = new C_ItemSlotChange();
        c_ItemSlotChange.SelectedItemDbId = sellectedItem.ItemDbId;
        c_ItemSlotChange.SelectedSlot = sellectedItem.ItemData.Slot;
        c_ItemSlotChange.TargetItemDbId = swapItem.ItemDbId;
        c_ItemSlotChange.TargetSlot = (swapItem.ItemDbId != -1) ? swapItem.ItemData.Slot : swapItem.Slot;
        Managers.Network.Send(c_ItemSlotChange);
        Debug.Log($"{sellectedItem.ItemData.Slot} => {c_ItemSlotChange.TargetSlot}");


        // 현재 아이템 데이터를 다른 아이템에 전달
        swapItem.SetItem(ItemData, _inventory);
        // 다른 아이템의 데이터를 현재 아이템에 설정
        sellectedItem.SetItem(tempItem, _inventory);

        Debug.Log("아이템 스왑");
    }

    private UI_Inventory_Item FindSlotItem()
    {
        if (_inventory == null) return null;

        UI_Inventory_Item result = null;
        //IEnumerable<ISlot> itemList = _inventory.Items;
        //UI_GameScene gameScene = Managers.UI.SceneUI as UI_GameScene;
        //gameScene.QuickSlot.QuickSlot

        foreach (var other in _inventory.Items)
        {
            if (other == this) continue;

            float sqrDistance = (other.transform.position - _icon.transform.position).sqrMagnitude;

            if (sqrDistance < 10f * 10f)
            {
                result = other;

                return result;
            }
        }

        return result;
    }

    public Transform GetTransform()
    {
        return transform;
    }


    private void OnDrawGizmos()
    {
        if (_icon == null) return;
        if (ItemDbId == -1)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.white;
        }

        Gizmos.DrawWireSphere(_icon.transform.position, 10);

    }
}
