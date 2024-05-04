﻿using Data;
using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;
using static Define;

public class CreatureController : BaseController
{
    HpBar _hpBar;
    public override StatInfo Stat
    {
        get { return base.Stat; }
        set
        {
            base.Stat = value;
            UpdateHpBar();

        }
    }

    public override int Hp
    {
        get { return base.Hp; }
        set { base.Hp = value; UpdateHpBar(); }
    }

    protected void AddHpBar()
    {
        GameObject go = Managers.Resource.Instantiate("UI/HpBar", transform);
        go.transform.localPosition = new Vector3(0, 0.5f, 0);
        go.name = "HpBar";
        _hpBar = go.GetComponent<HpBar>();
        UpdateHpBar();
    }

    void UpdateHpBar()
    {
        if (_hpBar == null)
        {
            return;
        }

        float ratio = 0.0f;
        if (Stat.MaxHp > 0)
        {
            ratio = (Hp / (float)Stat.MaxHp);
        }
        _hpBar.SetHpBar(ratio);
    }


    
    protected override void Init()
    {
        base.Init();
        AddHpBar();
    }


    public virtual void OnDamaged()
    {

    }

    public virtual void OnDead()
    {
        State = CreatureState.Dead;

        GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
        effect.transform.position = transform.position;
        effect.GetComponent<Animator>().Play("START");
        GameObject.Destroy(effect, 0.5f);
        Managers.Object.Remove(Id);
        Managers.Resource.Destroy(gameObject);
    }

    public virtual void UseSkill(int skillId)
    {
        
    }
}
