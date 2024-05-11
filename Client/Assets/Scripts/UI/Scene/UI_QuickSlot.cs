using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuickSlot : UI_Scene
{

    public class QuickSlotItem
    {
        public string Slot;
    }

    enum Images
    {
        F1,
        F1Icon,
        F2,
        F2Icon,
        F3,
        F3Icon,
        F4,
        F4Icon,
        Q,
        QIcon,
        SpaceBar,
        SpaceBarIcon
    }

    public List<Image> QuickSlot = new List<Image>();
    public List<UI_QuickSlotItem> QuickSlotItems = new List<UI_QuickSlotItem>();

    public SlotType SlotType { get; set; } = SlotType.QuickSlotItem;

    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Images));

        QuickSlot.Add(Get<Image>((int)Images.F1Icon));
        QuickSlot.Add(Get<Image>((int)Images.F2Icon));
        QuickSlot.Add(Get<Image>((int)Images.F3Icon));
        QuickSlot.Add(Get<Image>((int)Images.F4Icon));

        foreach(var img in QuickSlot)
        {
            QuickSlotItems.Add(img.gameObject.GetOrAddComponent<UI_QuickSlotItem>());
            img.gameObject.SetActive(false);
        }
    }

}
