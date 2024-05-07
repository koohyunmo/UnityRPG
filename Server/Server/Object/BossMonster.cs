using Google.Protobuf.Protocol;
using Server.Data;
using Server.DB;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Object
{
    public class BossMonster : Monster
    {
        public BossMonster() 
        {
            ObjectType = GameObjectType.BossMonster;
        }
        public override void OnDead(GameObject attacker)
        {
            // TODO 아이템 생성
            var onwer = attacker.GetOwner();

            if (onwer.ObjectType == GameObjectType.Player)
            {
                Player player = (Player)onwer;
                RewardData rewardData = GetRandomReward();
                if (rewardData != null)
                {
                    MyDbTransaction.RewardPlayer(player, rewardData, Room);
                }
                else
                {
                    MyDbTransaction.RewardGold(player, 10, Room);
                }
                MyDbTransaction.RewardExp(player, 10, Room);
            }

            if (Room == null)
                return;

            S_Die diePacket = new S_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;
            Console.WriteLine($"Die Object_{diePacket.ObjectId} By Attacker_{diePacket.ObjectId}");
            Room.BroadcastVisionCube(CellPos, diePacket);

            GameRoom room = Room;
            Room.LeaveGame(Id);

            Stat.Hp = Stat.MaxHp;
            PosInfo.State = CreatureState.Idle;
            PosInfo.MoveDir = MoveDir.Down;

            room.PushAfter(5000 * 10,room.EnterGame, this, true);
        }

    }
}
