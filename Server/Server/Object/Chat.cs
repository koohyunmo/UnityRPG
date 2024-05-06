using Google.Protobuf.Protocol;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Object
{
    public class Chat : GameObject
    {

        public GameObject Owner { get; set; }
        S_ChatSpawn _packet;
        int totalTick = 0;

        public Chat()
        {
            ObjectType = GameObjectType.Chat;
            _packet = new S_ChatSpawn() { ChatInfo = new ChatInfo() };
        }

        public void Start(S_ChatSpawn packet)
        {
            Update();
            _packet.SenderId = packet.SenderId;
            _packet.ChatId = packet.ChatId;
            _packet.ChatInfo.MergeFrom(packet.ChatInfo);
        }

        public override void Update()
        {
            if (Owner == null || Room == null)
                return;

            int tick = (int)(1000 / 10);
            totalTick += tick;
            if(totalTick < 5000)
            {
                Room.PushAfter(tick, Update);
                Room.PushAfter(100,Room.BroadcastVisionCube, Room, Owner.CellPos, _packet);
            }
            else
            {
                S_ChatDespawn s_ChatDespawn = new S_ChatDespawn();
                s_ChatDespawn.SenderId = _packet.SenderId;
                s_ChatDespawn.ChatId = _packet.ChatId;
                Room.Push(Room.HandleDespawnChat, Owner as Player, Room, s_ChatDespawn);
            }

        }

        public override GameObject GetOwner()
        {
            return Owner;
        }
    }
}
