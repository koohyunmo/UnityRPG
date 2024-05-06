using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using static Item;

public class MyPlayerController : PlayerController
{
    bool _moveKeyPressed = false;
    public int WeaponDamage { get; private set;}
    public int ArmorDefence { get; private set; }

    protected override void Init()
    {
        base.Init();
        RefreshAddtionalStat();
    }

    protected override void UpdateController()
    {
        GetUIKeyInput();
        switch (State)
        {
            case CreatureState.Idle:
                GetDirInput();
                break;
            case CreatureState.Moving:
                GetDirInput();
                break;
        }

        base.UpdateController();
    }

    private void GetUIKeyInput()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            UI_GameScene gameScene = Managers.UI.SceneUI as UI_GameScene;
            if(gameScene)
            {
                UI_Inventory invenUI = gameScene.InvenUI;

                invenUI.gameObject.SetActive(!invenUI.gameObject.activeSelf);

                if (invenUI.gameObject.activeSelf)
                    invenUI.RefreshUI();
            }

        }
        else if(Input.GetKeyDown(KeyCode.C))
        {
            UI_GameScene gameScene = Managers.UI.SceneUI as UI_GameScene;
            if(gameScene)
            {
                UI_Stat statUI = gameScene.StatUI;

                statUI.gameObject.SetActive(!statUI.gameObject.activeSelf);

                if (statUI.gameObject.activeSelf)
                    statUI.RefreshUI();
            }

        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            UI_Market market = Managers.UI.ShowPopupUI<UI_Market>();
        }
        else if(Input.GetKeyDown(KeyCode.M))
        {
            UI_MailPopup mail = Managers.UI.ShowPopupUI<UI_MailPopup>();
        }
    }

    protected override void UpdateIdle()
    {
        // 이동 상태로 갈지 확인
        if (_moveKeyPressed)
        {
            State = CreatureState.Moving;
            return;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            //Debug.Log($"Input Space : {State} _ Coroutine : {_coSkillCoolTime == null} ");
            if (_coSkillCoolTime == null)
            {
                //Debug.Log("Use Arrow Skill");

                C_Skill skill = new C_Skill() { Info = new SkillInfo() };
                skill.Info.SkillId = 2;
                Managers.Network.Send(skill);

                _coSkillCoolTime = StartCoroutine(CoInputCoolTime(0.25f));
            }
        }
        else if(Input.GetKey(KeyCode.Q))
        {
            if (_coSkillCoolTime == null)
            {
                Debug.Log("Use Arrow Skill");

                C_Skill skill = new C_Skill() { Info = new SkillInfo() };
                skill.Info.SkillId = 3;
                Managers.Network.Send(skill);
                _coSkillCoolTime = StartCoroutine(CoInputCoolTime(0.25f));
            }
        }
        else if (Input.GetKey(KeyCode.W))
        {
            if (_coSkillCoolTime == null)
            {
                //Debug.Log("Use Arrow Skill");

                C_Skill skill = new C_Skill() { Info = new SkillInfo() };
                skill.Info.SkillId = 4;
                Managers.Network.Send(skill);
                _coSkillCoolTime = StartCoroutine(CoInputCoolTime(0.25f));
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            C_Teleport teleport = new C_Teleport();
            Managers.Network.Send(teleport);
        }
    }
    Coroutine _coSkillCoolTime = null;
    IEnumerator CoInputCoolTime(float time)
    {
        State = CreatureState.Skill;
        yield return new WaitForSeconds(time);
        State = CreatureState.Idle;
        _coSkillCoolTime = null;
    }

    void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    // 키보드 입력
    void GetDirInput()
    {
        _moveKeyPressed = true;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Dir = MoveDir.Up;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            Dir = MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            Dir = MoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            Dir = MoveDir.Right;
        }
        else
        {
            _moveKeyPressed = false;
        }
    }

    protected override void MoveToNextPos()
    {
        
        if (_moveKeyPressed == false)
        {
            State = CreatureState.Idle;
            CheckUpdatedFlag();
            return;
        }

        Vector3Int destPos = CellPos;

        switch (Dir)
        {
            case MoveDir.Up:
                destPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                destPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                destPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                destPos += Vector3Int.right;
                break;
        }

        if (Managers.Map.CanGo(destPos))
        {
            if (Managers.Object.FindCreature(destPos) == null)
            {
                CellPos = destPos;
            }
        }

        CheckUpdatedFlag();
    }

    /// <summary>
    /// 플레이어의 입력값이 바뀌면 패킷을 보냄
    /// </summary>
    protected override void CheckUpdatedFlag()
    {
        if(_updated)
        {
            C_Move movePacket = new C_Move();
            movePacket.PosInfo = PosInfo;
            Managers.Network.Send(movePacket);
            _updated = false;
        }
    }

    public void RefreshAddtionalStat()
    {
        WeaponDamage = 0;
        ArmorDefence = 0;

        foreach (Item item in Managers.Inven.Items.Values)
        {
            if (item.Equipped == false)
                continue;
            switch (item.ItemType)
            {
                case ItemType.Weapon:
                    WeaponDamage += ((Weapon)item).Damage;
                    break;
                case ItemType.Armor:
                    ArmorDefence += ((Armor)item).Defence;
                    break;
            }
        }

    }
}
