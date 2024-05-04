using System.Collections;
using System.Collections.Generic;
using Data;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;
using static Item;

public class UI_Stat : UI_Base
{

    public enum Iamges
    {
        Slot_Armor,
        Slot_Helmet,
        Slot_Boots,
        Slot_Shield,
        Slot_Weapon
    }

    public enum Texts
    {
        NameText,
        AttackValueText,
        DefenceValueText
    }


    bool _init = false;
    public override void Init()
    {
        Bind<Image>(typeof(Iamges));
        Bind<Text>(typeof(Texts));

        _init = true;
        RefreshUI();
    }

    public void RefreshUI()
    {

        if (_init == false)
        { return; }

        // 우선 다 가린다.
        Get<Image>((int)Iamges.Slot_Helmet).enabled = false;
        Get<Image>((int)Iamges.Slot_Armor).enabled = false;
        Get<Image>((int)Iamges.Slot_Boots).enabled = false;
        Get<Image>((int)Iamges.Slot_Shield).enabled = false;
        Get<Image>((int)Iamges.Slot_Weapon).enabled = false;

        // 채워준다,
        foreach (Item item in Managers.Inven.Items.Values)
        {
            if (item.Equipped == false)
                continue;

            ItemData itemData = null;
            Managers.Data.ItemDict.TryGetValue(item.TemplateId, out itemData);
            Sprite icon = Managers.Resource.Load<Sprite>(itemData.iconPath);

            if (item.ItemType == ItemType.Weapon)
            {
                Get<Image>((int)Iamges.Slot_Weapon).enabled = true;
                Get<Image>((int)Iamges.Slot_Weapon).sprite = icon;
            }
            else if (item.ItemType == ItemType.Armor)
            {
                Armor armor = (Armor)item;
                switch (armor.ArmorType)
                {
                    case ArmorType.Helmet:
                        {
                            Get<Image>((int)Iamges.Slot_Helmet).enabled = true;
                            Get<Image>((int)Iamges.Slot_Helmet).sprite = icon;
                            break;
                        }
                    case ArmorType.Armor:
                        {
                            Get<Image>((int)Iamges.Slot_Armor).enabled = true;
                            Get<Image>((int)Iamges.Slot_Armor).sprite = icon;
                            break;
                        }
                    case ArmorType.Boots:
                        {
                            Get<Image>((int)Iamges.Slot_Boots).enabled = true;
                            Get<Image>((int)Iamges.Slot_Boots).sprite = icon;
                            break;
                        }

                }
            }
        }

        // Text
        MyPlayerController player = Managers.Object.MyPlayer;
        player.RefreshAddtionalStat();

        Get<Text>((int)Texts.NameText).text = player.name;
        int totalDamage = player.Stat.Attack + player.WeaponDamage;
        Get<Text>((int)Texts.AttackValueText).text = $"{totalDamage} (+{player.WeaponDamage})";
        Get<Text>((int)Texts.DefenceValueText).text = $"{player.ArmorDefence}";


    }

}
