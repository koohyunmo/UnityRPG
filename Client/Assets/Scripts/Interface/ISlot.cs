using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlotType
{
    InventoryItem,
    QuickSlotItem,
}

public interface ISlot 
{
    SlotType SlotType { get; set;}
    Transform GetTransform();
}
