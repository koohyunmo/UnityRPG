using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Server.DB;
using MyDbTransaction = Server.DB.MyDbTransaction;

namespace Server.Object
{
    public class Monster : GameObject
    {
        public int TemplateId { get; private set; }
        public Monster()
        {
            ObjectType = GameObjectType.Monster;
        }

        public void Init(int templateId)
        {            //Temp
            TemplateId = templateId;

            MonsterData monster = null;
            DataManager.MonsterDict.TryGetValue(templateId, out monster);
            Stat.MergeFrom(monster.stat);
            Stat.Hp = monster.stat.MaxHp;
            State = CreatureState.Idle;
        }
        protected IJob _job;
        public override void Update()
        {
            switch (State)
            {
                case CreatureState.Idle:
                    UpdateIdle();
                    break;
                case CreatureState.Moving:
                    UpdateMoving();
                    break;
                case CreatureState.Skill:
                    UpdateSkill();
                    break;
                case CreatureState.Dead:
                    UpdateDead();
                    break;
            }
            // 5프레임 마다 Update
            if (Room != null)
                _job = Room.PushAfter(200, Update);
        }

        Player _target;
        long _nexSearchTick = 0;
        int _searchCellDist = 10;
        int _chaseCellDist = 15;
        protected virtual void UpdateIdle()
        { 
            if(_nexSearchTick > Environment.TickCount64) // 비효율적임
            {
                return;
            }

            _nexSearchTick = Environment.TickCount64 +1000; // 비효율적임

            Player target = Room.FindClosestPlayer(CellPos, _searchCellDist);

            if (target == null)
                return;

            _target = target;
            State = CreatureState.Moving;
        }
        int _skillRange = 1;
        long _nextMoveTick = 0;
        protected virtual void UpdateMoving()
        {
            if (_nextMoveTick > Environment.TickCount64)
                return;

            int moveTick = (int)(1000 / Speed);
            _nextMoveTick = Environment.TickCount64 + moveTick;

            if(_target == null || _target.Room != Room) // 내가 쫒고있는 플레이어가 사라지거나 나갈경우
            {
                _target = null;
                State = CreatureState.Idle;
                BoradcastMove();
                return;
            }

            Vector2Int dir = _target.CellPos - CellPos;
            int dist = dir.cellDistFromZero;

            if(dist == 0 || dist > _chaseCellDist) // 플레이어가 너무 멀리 도망가면 쫒기를 포기
            {
                _target = null;
                State = CreatureState.Idle;
                BoradcastMove();
                return;
            }

             List<Vector2Int> path = Room.Map.FindPath(CellPos, _target.CellPos, checkObjects: true); // checkObject 오브젝트 무시여부
            if(path.Count < 2 || path.Count > _chaseCellDist)// 플레이어가 없거나 멀면 도망가면 쫒기를 포기
            {
                _target = null;
                State = CreatureState.Idle;
                BoradcastMove();
                return;
            }

            // 스킬로 넘어갈지
            if(dist <= _skillRange && (dir.x ==0 || dir.y == 0))
            {
                _coolTick = 0;
                State = CreatureState.Skill;
                return;
            }

            // 이동
            Dir = GetDirFromVec(path[1] - CellPos);
            Room.Map.ApplyMove(this, path[1]);

            //
            BoradcastMove();
        }

        void BoradcastMove()
        {
            S_Move movePacket = new S_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = PosInfo;
            Room.BroadcastVisionCube(CellPos, movePacket);
        }
        long _coolTick = 0;
        protected virtual void UpdateSkill()
        {
            if(_coolTick == 0)
            {
                // 유효한 타겟인지
                if(_target == null || _target.Room != Room || _target.Hp == 0)
                {
                    _target = null;
                    State = CreatureState.Moving;
                    BoradcastMove();
                    return;
                }

                // 스킬이 아직 사용 가능한지
                Vector2Int dir = (_target.CellPos - CellPos);
                int dist = dir.cellDistFromZero;
                bool canUseSkill = (dist <= _skillRange && (dir.x == 0 || dir.y == 0));
                if(canUseSkill == false)
                {
                    _target = null;
                    State = CreatureState.Moving;
                    BoradcastMove();
                    return;
                }

                // 타케팅 방향 주시
                MoveDir lookDir = GetDirFromVec(dir);
                if(Dir != lookDir)
                {
                    Dir = lookDir;
                    BoradcastMove();
                }

                Skill skillData = null;
                DataManager.SkillDict.TryGetValue(1, out skillData);

                // 데미지 판정
                _target.OnDamaged(this, skillData.damage + TotalAttack);

                // 스킬 사용 Broadcast
                S_Skill skill = new S_Skill() { Info  = new SkillInfo()};
                skill.ObjectId = Id;
                skill.Info.SkillId = skillData.id;
                Room.BroadcastVisionCube(CellPos,skill);


                // 스킬 쿨타임 적용
                int coolTick = (int)(1000 * skillData.cooldown);
                _coolTick = Environment.TickCount64 + coolTick;
            }

            if (_coolTick > Environment.TickCount)
                return;

            _coolTick = 0;
        }
        protected virtual void UpdateDead()
        { 
        }

        public override void OnDead(GameObject attacker)
        {

            // TODO 아이템 생성
            var onwer = attacker.GetOwner();

            if (onwer.ObjectType == GameObjectType.Player)
            {
                Player player = (Player)onwer;
                RewardData rewardData = GetRandomReward();
                if(rewardData !=null)
                {
                    MyDbTransaction.RewardPlayer(player, rewardData, Room);
                }
                else
                {
                    MyDbTransaction.RewardGold(player, 10, Room);
                }
                MyDbTransaction.RewardExp(player, 10, Room);
            }

            if(_job != null)
            {
                _job.Cancle = true;
                _job = null;
            }

            base.OnDead(attacker);
        }

        protected RewardData GetRandomReward()
        {
            MonsterData monsterData = null;
            DataManager.MonsterDict.TryGetValue(TemplateId, out monsterData);

            int rand = new Random().Next(0, 101);

            int sum = 0;
            foreach (RewardData rewardData in monsterData.rewards)
            {
                sum += rewardData.probability;

                if (rand <= sum)
                {
                    Console.WriteLine($"Item Drop : {rewardData.itemId}");
                    return rewardData;
                }
            }

            return null;
        }
    }

}
