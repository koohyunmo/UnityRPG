using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Server.DB;
using Server.Game;
using Server.Game.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Object
{
    public class Player : GameObject
    {
        public int PlayerDbId { get; set; }
        public ClientSession Session { get; set; }
        public VisionCube Vision { get; private set; }
        public Inventory Inven { get; private set; } = new Inventory();
        public int Gold { get; set; }

        public int WeaponDamage { get; private set; }
        public int ArmorDefence { get; private set; }

        public override int TotalAttack => base.TotalAttack + WeaponDamage;
        public override int TotalDefence => base.TotalDefence + ArmorDefence;

        public Player() 
        {
            ObjectType = GameObjectType.Player;
            Vision = new VisionCube(this);
        }

        public override void OnDamaged(GameObject attacker, int damage)
        {
            Console.WriteLine($" Player_{Id} : OnDamaged : {damage}! To {attacker.Id}");
            base.OnDamaged(attacker, damage);

        }

        public override void OnDead(GameObject attacker)
        {
            if (Room == null)
                return;

            S_Die diePacket = new S_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;
            Console.WriteLine($"Die Object_{diePacket.ObjectId} By Attacker_{diePacket.ObjectId}");
            Room.BroadcastVisionCube(CellPos, diePacket);

            //GameRoom room = Room;
            //Room.LeaveGame(Id);

            //Stat.Hp = Stat.MaxHp;
            //PosInfo.State = CreatureState.Idle;
            //PosInfo.MoveDir = MoveDir.Down;

            //room.Push(room.EnterGame, this, true);

            Room.Push(Room.HandlePlayerHomeTeleport, this);
        }

        public void OnLeaveGame()
        {
            // DB 연동?
            // 단점
            // 서버 다운되면 아직 저장되지 않은 정보 날아감
            // 코드 흐름을 다 막아버린다.
            // 해결법
            // 비동기 사용?
            // DB 전용 쓰레드 사용
            // DB 쓰레드 문제점
            // -- 결과를 받아서 이어서 처리를 해야 하는 경우가 많음.
            // -- 아이템 생성
            // 해결법은 DB 전용 쓰레드 + 비동기를 사용해서 동기화?
            MyDbTransaction.SavePlayerStatus_Step1(this, Room);
        }

        public void HandleEquipItem(C_EquipItem equipPacket)
        {

            Item item = Inven.Get(equipPacket.ItemDbId);
            if (item == null)
            {
                return;
            }
            if (item.ItemType == ItemType.Consumable)
            {
                return;
            }
            // 착용 요청이라면 겹치는 부위 해제
            if (equipPacket.Equipped)
            {
                Item unequipItem = null;
                if (item.ItemType == ItemType.Weapon)
                {
                    unequipItem = Inven.Find(i => i.Equipped && i.ItemType == ItemType.Weapon);
                }
                else if (item.ItemType == ItemType.Armor)
                {
                    ArmorType armorType = ((Armor)item).ArmorType;
                    unequipItem = Inven.Find(i => i.Equipped && i.ItemType == ItemType.Armor
                                                                    && ((Armor)i).ArmorType == armorType);
                }

                if (unequipItem != null)
                {
                    // 메모리 선 적용
                    unequipItem.Equipped = false;
                    // DB에 Noti
                    MyDbTransaction.EquipItemNoti(this, unequipItem);
                    // 클라에 통보 TODO
                    S_EquipItem equipOkItem = new S_EquipItem();
                    equipOkItem.ItemDbId = unequipItem.ItemDbId;
                    equipOkItem.Equipped = unequipItem.Equipped;
                    Session.Send(equipOkItem);
                }
            }

            {
                // 메모리 선 적용
                item.Equipped = equipPacket.Equipped;
                // DB에 Noti
                MyDbTransaction.EquipItemNoti(this, item);
                // 클라에 통보 TODO
                S_EquipItem equipOkItem = new S_EquipItem();
                equipOkItem.ItemDbId = equipPacket.ItemDbId;
                equipOkItem.Equipped = equipPacket.Equipped;
                Session.Send(equipOkItem);
            }

            RefreshAdditionalStat();
        }

        public void RefreshAdditionalStat()
        {
            WeaponDamage = 0;
            ArmorDefence = 0;

            foreach(Item item in Inven.Items.Values)
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
}