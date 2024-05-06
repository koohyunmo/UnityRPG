﻿using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using Server.Game.Skill;
using Server.Object;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public partial class GameRoom : JobSerializer
    {

        public void HandleMove(Player player, C_Move movePacket)
        {
            if (player == null)
                return;

            // TODO : 검증
            PositionInfo movePosInfo = movePacket.PosInfo;
            ObjectInfo info = player.Info;

            // 다른 좌표로 이동할 경우, 갈 수 있는지 체크
            if (movePosInfo.PosX != info.PosInfo.PosX || movePosInfo.PosY != info.PosInfo.PosY)
            {
                if (Map.CanGo(new Vector2Int(movePosInfo.PosX, movePosInfo.PosY)) == false)
                    return;
            }


            info.PosInfo.State = movePosInfo.State;
            info.PosInfo.MoveDir = movePosInfo.MoveDir;
            Map.ApplyMove(player, new Vector2Int(movePosInfo.PosX, movePosInfo.PosY));


            // 다른 플레이어한테도 알려준다
            S_Move resMovePacket = new S_Move();
            resMovePacket.ObjectId = player.Info.ObjectId;
            resMovePacket.PosInfo = movePacket.PosInfo;
            resMovePacket.PosInfo.TileInfo = Map.GetTileInfo(player.CellPos);


            BroadcastVisionCube(player.CellPos, resMovePacket);
        }

        public void HandleTeleport(Player player, C_Teleport teleportPacket)
        {
            if (player == null)
                return;

            if (Map.IsPortal(player.CellPos))
            {
                S_Teleport resTeleportPacket = new S_Teleport();
                resTeleportPacket.MapId = 2;

                LeaveGame(player.Id);
                Unicast(player, resTeleportPacket);
                GameRoom newRoom = GameLogic.Instance.Find(2);
                newRoom.Push(newRoom.EnterGame, player, true);
            }


        }
        static Dictionary<int, Dictionary<int, long>> _playerSkillCoolDowns = new Dictionary<int, Dictionary<int, long>>();
        public void HandleSkill(Player player, C_Skill skillPacket)
        {
            if (player == null)
                return;


            ObjectInfo info = player.Info;
            if (info.PosInfo.State != CreatureState.Idle)
                return;

            // TODO : 스킬 사용 가능 여부 체크
            info.PosInfo.State = CreatureState.Skill;
            S_Skill skill = new S_Skill() { Info = new SkillInfo() };
            skill.ObjectId = info.ObjectId;
            skill.Info.SkillId = skillPacket.Info.SkillId;


            //BroadcastVisionCube(player.CellPos, skill);
            //Console.WriteLine("BroadcastVisionCube + HandleSkill" + skillPacket.Info.SkillId);

            Data.Skill skillData = null;
            if (DataManager.SkillDict.TryGetValue(skillPacket.Info.SkillId, out skillData) == false)
                return;

            long currentTicks = DateTime.UtcNow.Ticks;

            if (!_playerSkillCoolDowns.ContainsKey(player.Id))
            {
                _playerSkillCoolDowns[player.Id] = new Dictionary<int, long>();
            }
            if (!_playerSkillCoolDowns[player.Id].ContainsKey(skillPacket.Info.SkillId))
            {
                // skillData.cooldown이 초 단위로 주어지고, 틱으로 변환
                _playerSkillCoolDowns[player.Id].Add(skillPacket.Info.SkillId, currentTicks + (long)(skillData.cooldown * 10000000));
            }
            else
            {
                if (_playerSkillCoolDowns[player.Id][skillPacket.Info.SkillId] > currentTicks)
                {
                    Console.WriteLine($"Remain Ticks : {_playerSkillCoolDowns[player.Id][skillPacket.Info.SkillId] - currentTicks}");
                    player.State = CreatureState.Idle;
                    return; // 스킬 쿨다운이 남아있을 경우 추가적인 처리가 필요
                }
                else
                {
                    // 쿨다운 완료, 스킬 사용 가능
                    _playerSkillCoolDowns[player.Id][skillPacket.Info.SkillId] = currentTicks + (long)(skillData.cooldown * 10000000);
                    Console.WriteLine($"Skill {skillPacket.Info.SkillId} used by player {player.Id}. Cooldown reset.");
                }
            }

            switch (skillData.skillType)
            {
                case SkillType.SkillAuto:
                    {
                        Vector2Int skillPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
                        GameObject target = Map.Find(skillPos);
                        if (target != null)
                        {
                            Console.WriteLine("Hit GameObject !");
                        }
                    }
                    break;
                case SkillType.SkillProjectile:
                    {
                        if (SkillManager.UseSkill(skillPacket, player, player.Room) == false)
                        {
                            Console.WriteLine($"SkillId:{skillPacket.Info.SkillId} 문제있음");
                            return;
                        }


                        //Arrow arrow = ObjectManager.Instance.Add<Arrow>();
                        //if (arrow == null)
                        //    return;

                        //arrow.Owner = player;
                        //arrow.Data = skillData;
                        //arrow.PosInfo.State = CreatureState.Moving;
                        //arrow.PosInfo.MoveDir = player.PosInfo.MoveDir;
                        //arrow.PosInfo.PosX = player.PosInfo.PosX;
                        //arrow.PosInfo.PosY = player.PosInfo.PosY;
                        //arrow.Speed = skillData.projectile.speed;
                        //Push(EnterGame, arrow, false);
                        //Console.WriteLine("HandleSkill" + skillPacket.Info.SkillId);
                    }
                    break;
            }

            BroadcastVisionCube(player.CellPos, skill);
            Console.WriteLine("BroadcastVisionCube + HandleSkill" + skillPacket.Info.SkillId);
        }

    }
}
