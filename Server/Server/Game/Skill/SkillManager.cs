using Google.Protobuf.Protocol;
using Server.Data;
using Server.Object;
using SharedDB.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Skill
{
    public class SkillManager
    {

        private SkillManager() { }
        public static bool UseSkill(C_Skill skillPacket, Player player, GameRoom room)
        {
            Data.Skill skillData = null;
            if (DataManager.SkillDict.TryGetValue(skillPacket.Info.SkillId, out skillData) == false)
                return false;

            switch (skillPacket.Info.SkillId)
            {
                case 2:
                    {
                        Arrow arrow = ObjectManager.Instance.Add<Arrow>();

                        arrow.Owner = player;
                        arrow.Data = skillData;
                        arrow.PosInfo.State = CreatureState.Moving;
                        arrow.PosInfo.MoveDir = player.PosInfo.MoveDir;
                        arrow.PosInfo.PosX = player.PosInfo.PosX;
                        arrow.PosInfo.PosY = player.PosInfo.PosY;
                        arrow.Speed = skillData.projectile.speed;
                        room.Push(room.EnterGame, arrow, false);
                    }
                    break;

                case 3:
                    {
                        // 첫 번째 화살: 플레이어 위치에서 발사
                        Arrow arrow = ObjectManager.Instance.Add<Arrow>();

                        arrow.Owner = player;
                        arrow.Data = skillData;
                        arrow.PosInfo.State = CreatureState.Moving;
                        arrow.PosInfo.MoveDir = player.PosInfo.MoveDir;
                        arrow.PosInfo.PosX = player.PosInfo.PosX;
                        arrow.PosInfo.PosY = player.PosInfo.PosY;
                        arrow.Speed = skillData.projectile.speed;
                        room.Push(room.EnterGame, arrow, false);

                        // 두 번째 화살: 플레이어 방향 위에서 한 칸 떨어진 위치에서 발사
                        Arrow arrow2 = ObjectManager.Instance.Add<Arrow>();

                        arrow2.Owner = player;
                        arrow2.Data = skillData;
                        arrow2.PosInfo.State = CreatureState.Moving;
                        arrow2.PosInfo.MoveDir = player.PosInfo.MoveDir;
                        arrow2.PosInfo.PosX = player.PosInfo.PosX + (player.PosInfo.MoveDir == MoveDir.Left || player.PosInfo.MoveDir == MoveDir.Right ? 0 : (player.PosInfo.MoveDir == MoveDir.Up ? -1 : 1));
                        arrow2.PosInfo.PosY = player.PosInfo.PosY + (player.PosInfo.MoveDir == MoveDir.Up || player.PosInfo.MoveDir == MoveDir.Down ? 0 : (player.PosInfo.MoveDir == MoveDir.Left ? -1 : 1));
                        arrow2.Speed = skillData.projectile.speed;
                        room.PushAfter(100, room.EnterGame, arrow2, false);

                        Arrow arrow3 = ObjectManager.Instance.Add<Arrow>();

                        arrow3.Owner = player;
                        arrow3.Data = skillData;
                        arrow3.PosInfo.State = CreatureState.Moving;
                        arrow3.PosInfo.MoveDir = player.PosInfo.MoveDir;
                        arrow3.PosInfo.PosX = player.PosInfo.PosX + (player.PosInfo.MoveDir == MoveDir.Left || player.PosInfo.MoveDir == MoveDir.Right ? 0 : (player.PosInfo.MoveDir == MoveDir.Up ? 1 : -1));
                        arrow3.PosInfo.PosY = player.PosInfo.PosY + (player.PosInfo.MoveDir == MoveDir.Up || player.PosInfo.MoveDir == MoveDir.Down ? 0 : (player.PosInfo.MoveDir == MoveDir.Left ? 1 : -1));
                        arrow3.Speed = skillData.projectile.speed;
                        room.PushAfter(100, room.EnterGame, arrow3, false);

                    }
                    break;
                case 4:
                    {

                    }
                    break;
            }
            player.State = CreatureState.Idle;
            Console.WriteLine("HandleSkill" + skillPacket.Info.SkillId);

            return true;
        }
    }
}
