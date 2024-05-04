using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Unity.Collections;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    Coroutine _coSkill;

    protected override void Init()
    {
        base.Init();
    }
    protected override void UpdateController()
    {
        base.UpdateController();
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }

    public override void OnDamaged()
    {
        base.OnDead();
    }

    public override void UseSkill(int skillId)
    {
        if (skillId == 1)
        {
            State = CreatureState.Skill;
        }
        else if (skillId == 2)
        {
            State = CreatureState.Skill;
        }
    }
}
