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
        SpaceBarIcon,
        F1CoolTime,
        F2CoolTime,
        F3CoolTime,
        F4CoolTime,
        QCoolTime,
        SpaceBarCoolTime,
    }

    enum Texts
    {
        F1Count,
        F2Count,
        F3Count,
        F4Count,
    }

    public List<Image> QuickSlot = new List<Image>();
    public List<Image> CoolTimeSlot = new List<Image>();
    public List<UI_QuickSlotItem> QuickSlotItems = new List<UI_QuickSlotItem>();

    // 미완성 쿨타임 과 슬롯이미지들을  UI_QuickSlotItem으로 옮겨야함
    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));

        QuickSlot.Add(Get<Image>((int)Images.F1Icon));
        QuickSlot.Add(Get<Image>((int)Images.F2Icon));
        QuickSlot.Add(Get<Image>((int)Images.F3Icon));
        QuickSlot.Add(Get<Image>((int)Images.F4Icon));

        CoolTimeSlot.Add(Get<Image>((int)Images.F1CoolTime));
        CoolTimeSlot.Add(Get<Image>((int)Images.F2CoolTime));
        CoolTimeSlot.Add(Get<Image>((int)Images.F3CoolTime));
        CoolTimeSlot.Add(Get<Image>((int)Images.F4CoolTime));
        CoolTimeSlot.Add(Get<Image>((int)Images.QCoolTime));
        CoolTimeSlot.Add(Get<Image>((int)Images.SpaceBarCoolTime));

        Managers.QuickSlot.AddCoolTimeSlot("F1", CoolTimeSlot[0]);
        Managers.QuickSlot.AddCoolTimeSlot("F2", CoolTimeSlot[1]);
        Managers.QuickSlot.AddCoolTimeSlot("F3", CoolTimeSlot[2]);
        Managers.QuickSlot.AddCoolTimeSlot("F4", CoolTimeSlot[3]);
        Managers.QuickSlot.AddCoolTimeSlot("Q",  CoolTimeSlot[4]);
        Managers.QuickSlot.AddCoolTimeSlot("SpaceBar", CoolTimeSlot[5]);


        int count = 0;
        foreach(var img in QuickSlot)
        {
            var item = img.gameObject.GetOrAddComponent<UI_QuickSlotItem>();
            QuickSlotItems.Add(item);
            item.SetInfo((Images.F1 + count * 2 ).ToString(),
                            Get<Text>((int)Texts.F1Count + count));
            count++;
        }
    }

}
