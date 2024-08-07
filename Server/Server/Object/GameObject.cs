﻿using Google.Protobuf.Protocol;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Object
{
    public class GameObject
    {
        public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
        public int Id
        {
            get { return Info.ObjectId; }
            set { Info.ObjectId = value; }
        }
        public GameRoom Room { get; set; }
        public ObjectInfo Info { get; set; } = new ObjectInfo() { PosInfo = new PositionInfo() };
        public PositionInfo PosInfo { get; private set; } = new PositionInfo();
        public StatInfo Stat { get; private set; } = new StatInfo();

        public virtual int TotalAttack { get { return Stat.Attack; } }
        public virtual int TotalDefence { get { return 0; } }
        public float Speed
        {
            get { return Stat.Speed; }
            set { Stat.Speed = value; }
        }
        public int Hp
        {
            get { return Stat.Hp; }
            set { Stat.Hp = Math.Clamp(value,0,Stat.Hp); }
        }
        public CreatureState State

        {
            get
            {
                return PosInfo.State;
            }
            set
            {
                PosInfo.State = value;
            }
        }

        public MoveDir Dir
        {
            get
            {
                return PosInfo.MoveDir;
            }

            set
            {
                PosInfo.MoveDir = value;
            }
        }

        public GameObject()
        {
            Info.PosInfo = PosInfo;
            Info.StatInfo = Stat;
        }

        public Vector2Int CellPos
        {
            get
            {
                return new Vector2Int(PosInfo.PosX, PosInfo.PosY);
            }
            set
            {
                PosInfo.PosX = value.x;
                PosInfo.PosY = value.y;
            }
        }


        public Vector2Int GetFrontCellPos(MoveDir dir)
        {
            Vector2Int cellPos = CellPos;

            switch (dir)
            {
                case MoveDir.Up:
                    cellPos += Vector2Int.up;
                    break;
                case MoveDir.Down:
                    cellPos += Vector2Int.down;
                    break;
                case MoveDir.Left:
                    cellPos += Vector2Int.left;
                    break;
                case MoveDir.Right:
                    cellPos += Vector2Int.right;
                    break;
            }

            return cellPos;
        }

        public Vector2Int GetFrontCellPos()
        {
            return GetFrontCellPos(PosInfo.MoveDir);
        }

        public static MoveDir GetDirFromVec(Vector2Int dir)
        {
            if (dir.x > 0)
                return MoveDir.Right;
            else if (dir.x < 0)
                return MoveDir.Left;
            else if (dir.y > 0)
                return MoveDir.Up;
            else
                return MoveDir.Down;
        }

        public virtual void Heal(int healAmount)
        {
            this.Stat.Hp = Math.Min(this.Hp + healAmount, this.Stat.MaxHp);

            S_ChangeHp changePacket = new S_ChangeHp();
            changePacket.ObjectId = Id;
            changePacket.Hp = Stat.Hp;
            Room.BroadcastVisionCube(CellPos, changePacket);
        }


        public virtual void OnDamaged(GameObject attacker,int damage)
        {
            if (Room == null)
                return;
            if (attacker == null)
                return;

            damage = Math.Max((damage - TotalDefence),0);
            Stat.Hp = Math.Max(Stat.Hp - damage,0);

            S_ChangeHp changePacket = new S_ChangeHp();
            changePacket.ObjectId = Id;
            changePacket.Hp = Stat.Hp;
            Room.BroadcastVisionCube(CellPos,changePacket);

            S_SpawnDamage s_SpawnDamage = new S_SpawnDamage();
            s_SpawnDamage.Damage = damage;
            s_SpawnDamage.HitObjectId = Id;
            Room.BroadcastVisionCube(CellPos, s_SpawnDamage);

            if (Stat.Hp <= 0)
            {
                OnDead(attacker);
            }

        }

        public virtual void OnDead(GameObject attacker)
        {
            if (Room == null)
                return;

            S_Die diePacket = new S_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;
            Console.WriteLine($"Die Object_{diePacket.ObjectId} By Attacker_{diePacket.ObjectId}");
            Room.BroadcastVisionCube(CellPos,diePacket);

            GameRoom room = Room;
            Room.LeaveGame(Id);

            Stat.Hp = Stat.MaxHp;
            PosInfo.State = CreatureState.Idle;
            PosInfo.MoveDir = MoveDir.Down;

            room.Push(room.EnterGame, this,true);
        }

        public virtual void Update()
        {

        }

        public virtual GameObject GetOwner()
        {
            return this;
        }
    }

}