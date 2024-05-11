using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_QuickSlotItem : UI_Base, ISlot
{
    public SlotType SlotType { get; set;} = SlotType.QuickSlotItem;

    public Transform GetTransform()
    {
        return transform;
    }

    public override void Init()
    {

    }

    public void Swap<T>(T target, T select) where T : UI_Base
    {
        throw new System.NotImplementedException();
    }
}
