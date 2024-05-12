using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class InventoryManager
{
    public Dictionary<int, Item> Items {get;} = new Dictionary<int, Item>();

    public void Change(int dbId , ItemInfo itemInfo)
    {
        if(Items.ContainsKey(dbId) == false) return;
        if(itemInfo.Count <= 0)
        {
            Items[dbId] = null;
            return;
        }

        Items[dbId].Info.MergeFrom(itemInfo);

    }

    public void Add(Item item)
    {
        if(item.ItemType == Google.Protobuf.Protocol.ItemType.Consumable && Items.ContainsKey(item.ItemDbId) == true)
        {
            Items[item.ItemDbId] = item;
        }
        else
        {
            Items.Add(item.ItemDbId, item);
        }

    }
    public Item Get(int itemDbId)
    {
        Item item = null;
        Items.TryGetValue(itemDbId, out item);
        return item;
    }

    public Item Find(Func<Item, bool> condition)
    {
        foreach (Item item in Items.Values)
        {
            if (condition.Invoke(item))
            {
                return item;
            }
        }

        return null;
    }

    public void Clear()
    {
        Items.Clear();
    }
}
