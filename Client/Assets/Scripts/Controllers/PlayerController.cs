using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class PlayerController : CreatureController
{
    protected Coroutine _coSkill;
    protected bool _rangedSkill = false;

    GameObject playerNameCanvas = null;

    protected override void Init()
    {
        base.Init();


    }

    public void SetName(string name)
    {
        if (playerNameCanvas == null)
        {
            playerNameCanvas = Managers.Resource.Instantiate("Text/PlayerNameText", transform);
            playerNameCanvas.transform.localPosition = new Vector3(0, -0.8f, 0);
            playerNameCanvas.GetComponentInChildren<Text>().color = Managers.Object.MyPlayer.Id == Id ? Color.green : Color.cyan;
            playerNameCanvas.GetComponentInChildren<Text>().text = name;
        }
        else
        {
            playerNameCanvas.GetComponentInChildren<Text>().color = Managers.Object.MyPlayer.Id == Id ? Color.green : Color.cyan;
            playerNameCanvas.GetComponentInChildren<Text>().text = name;
        }

    }

    protected override void UpdateAnimation()
    {

        if (!_animator || !_sprite)
        {
            Debug.LogWarning("초기화 순서 주의");
            return;
        }

        if (State == CreatureState.Idle)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    _animator.Play("IDLE_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("IDLE_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else if (State == CreatureState.Moving)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    _animator.Play("WALK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("WALK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else if (State == CreatureState.Skill)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    _animator.Play(_rangedSkill ? "ATTACK_WEAPON_BACK" : "ATTACK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play(_rangedSkill ? "ATTACK_WEAPON_FRONT" : "ATTACK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play(_rangedSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_SIDE");
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play(_rangedSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_SIDE");
                    _sprite.flipX = false;
                    break;
            }
        }
        else
        {

        }
    }

    protected override void UpdateController()
    {
        base.UpdateController();
    }

    protected virtual void CheckUpdatedFlag()
    {

    }
    protected IEnumerator CoStartPunch()
    {
        // 대기 시간
        _rangedSkill = false;
        State = CreatureState.Skill;
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Idle;
        _coSkill = null;
        CheckUpdatedFlag();
    }

    IEnumerator CoStartShootArrow()
    {
        // 대기 시간
        _rangedSkill = true;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _coSkill = null;
        CheckUpdatedFlag();
    }

    public override void OnDamaged()
    {
        Debug.Log("Player HIT !");
    }

    public override void  UseSkill(int skillId)
    {
       if(skillId == 1)
       {
            _coSkill = StartCoroutine(CoStartPunch());
       }
       else if(skillId == 2)
       {
            _coSkill = StartCoroutine(CoStartShootArrow());
       }
    }


}
