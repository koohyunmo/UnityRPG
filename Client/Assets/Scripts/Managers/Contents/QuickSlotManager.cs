using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotManager
{

    public Dictionary<string,int> QuickSlot = new Dictionary<string, int>();
    public Dictionary<int, UI_QuickSlotItem> UiQuickSlotItem = new Dictionary<int, UI_QuickSlotItem>();
    public Dictionary<string, Image> coolTimeImage = new Dictionary<string, Image>();
    public Dictionary<string, bool> isOnCooldown = new Dictionary<string, bool>();


    public void AddCoolTimeSlot(string key, Image coolTime)
    {
        coolTime.gameObject.SetActive(false);
        coolTimeImage.Add(key, coolTime);
        isOnCooldown.Add(key, false);
    }

    public void AddQuickSlotItme(int dbId, UI_QuickSlotItem item)
    {
        UiQuickSlotItem.Add(dbId, item);
    }
    public void Add(string key, int dbId)
    {
        if (QuickSlot.ContainsKey(key))
        {
            // 키가 이미 존재하면 dbId를 업데이트
            QuickSlot[key] = dbId;
        }
        else
        {
            // 키가 존재하지 않으면 추가
            QuickSlot.Add(key, dbId);
        }
    }

    public void Remove(string key)
    {
        QuickSlot.Remove(key);
    }

    private void ChnageKey(string key, int dbId)
    {
        QuickSlot[key] = dbId;
    }

    public void UseItem(string key)
    {
        // TODO 패킷 보내기
        if (QuickSlot.ContainsKey(key) == false) return;
        if(isOnCooldown[key] == true) return;

        isOnCooldown[key] = false;
        Managers.Instance.StartCoroutine(co_CoolTime(key,0.5f));

        C_ItemUse c_ItemUse= new C_ItemUse();
        c_ItemUse.ItemDbId = QuickSlot[key];
        c_ItemUse.ItemType = ItemType.Consumable;

        Managers.Network.Send(c_ItemUse);
        Debug.Log($"{key} : Use {QuickSlot[key]}");
    }

    public void UseSkill(string key,float coolDown)
    {
        if (isOnCooldown[key] == true) return;

        isOnCooldown[key] = false;
        Managers.Instance.StartCoroutine(co_CoolTime(key, coolDown));
    }

    public void UseSkill(string key,int skillId)
    {
        C_Skill skill = new C_Skill() { Info = new SkillInfo() };
        skill.Info.SkillId = skillId;
        Managers.Network.Send(skill);
    }

    public void RefreshItem(int dbId)
    {
        if (UiQuickSlotItem.ContainsKey(dbId) == false) return;
        UiQuickSlotItem[dbId].RefreshItem();
    }

    IEnumerator co_CoolTime(string key, float cooldown)
    {
        if (!coolTimeImage.ContainsKey(key))
        {
            yield break;  // key가 존재하지 않으면 코루틴을 종료
        }

        coolTimeImage[key].gameObject.SetActive(true);
        coolTimeImage[key].fillAmount = 1;
        float elapsedTime = 0;  // 경과 시간을 추적

        while (coolTimeImage[key].fillAmount > 0)
        {
            elapsedTime += Time.deltaTime;  // 경과 시간 갱신
            coolTimeImage[key].fillAmount = 1 - elapsedTime / cooldown;  // 총 쿨다운 시간에 대한 비율 계산
            yield return null;

            if (elapsedTime >= cooldown)  // 쿨다운 시간이 끝나면 반복 종료
            {
                coolTimeImage[key].fillAmount = 0;  // fillAmount를 확실히 0으로 설정
                break;
            }
        }
        isOnCooldown[key] = false;  // cooldown이 끝나면 쿨다운 상태로 설정
    }


}
