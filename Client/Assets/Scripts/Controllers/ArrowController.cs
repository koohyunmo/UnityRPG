﻿using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ArrowController : BaseController
{
    protected override void Init()
    {
        switch (Dir)
        {
            case MoveDir.Up:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case MoveDir.Down:
                transform.rotation = Quaternion.Euler(0, 0, -180);
                break;
            case MoveDir.Left:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case MoveDir.Right:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
        }

        State = CreatureState.Moving;

        base.Init();
    }

    protected override void UpdateAnimation()
    {

    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Unit"))
        {
            GameObject tmp = Managers.Resource.Instantiate("Text/DamageTMP");
            if (tmp != null)
            {
                tmp.transform.position = other.transform.position;
                var dmageTmp = tmp.GetComponent<DamageTMP>();
                dmageTmp.SpawnMiss();
            }
        }
    }
}
