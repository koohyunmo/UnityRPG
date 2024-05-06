using Google.Protobuf.Protocol;
using Server.Data;
using Server.DB;
using Server.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Server.Game
{
    public partial class GameRoom : JobSerializer
    {

        public void HandleMailItemList(Player player, C_MailItemList packet)
        {
            if (player == null)
                return;
            S_MailItemList mailitemListPacket = new S_MailItemList();

            // 아이템 목록을 갖고 온다.
            using (AppDbContext db = new AppDbContext())
            {
                List<MailDb> items = db.Mails
                    .Where(i => i.OwnerId == player.PlayerDbId && i.Read == false)
                    .ToList();

                foreach (var item in items)
                {
                    mailitemListPacket.MailItems.Add(new MailItem()
                    {
                        ItemDbId = item.MailDbId,
                        TemplateId = item.TemplateId,
                        SellerId = item.SenderId,
                        SellerName = item.SenderName,
                    });
                }

                Push(Unicast, player,mailitemListPacket);
            }

        }

        public void HandleMailItemReceive(Player player,GameRoom room, C_MailItemReceive packet)
        {
            if (player == null)
                return;
            if (room == null)
                return;



            // 아이템 목록을 갖고 온다.
            using (AppDbContext db = new AppDbContext())
            {
                MailDb mailDb = db.Mails
                    .Where(i => i.MailDbId == packet.MailItem.ItemDbId && i.TemplateId == packet.MailItem.TemplateId)
                    .FirstOrDefault();

                if (mailDb == null)
                    return;
                if (mailDb.Read == true)
                    return;

                MyDbTransaction.ReceiveMailItemToPlayer(player, mailDb, room);
            }

        }

        public void HandleChat(Player player, GameRoom room, S_ChatSpawn packet)
        {
            if (player == null)
                return;
            if (room == null)
                return;

            Chat prevChat = null;
            if (_chat.Remove(packet.SenderId, out prevChat))
            {
                prevChat.Room = null;
            }

            Chat chat = new Chat();
            chat.Id = packet.ChatId;
            chat.Owner = player;
            chat.Room = this;
            chat.Start(packet);
            room._chat.Add(packet.SenderId,chat);

            // 스폰패킷
            {
                //room.Push(BroadcastVisionCube, player.CellPos, packet);
            }
            // 디스폰패킷
            {
                //S_ChatDespawn s_ChatDespawn = new S_ChatDespawn();
                //s_ChatDespawn.SenderId = packet.SenderId;
                //s_ChatDespawn.ChatId = packet.ChatId;
                //room.PushAfter(5000, room.HandleDespawnChat, player, room, s_ChatDespawn);
            }
        }

        public void HandleDespawnChat(Player player, GameRoom room, S_ChatDespawn packet)
        {
            if (player == null)
                return;
            if (room == null)
                return;

            Chat chat = null;
            if (_chat.Remove(packet.SenderId, out chat) == false)
                return;
            chat.Room = null;

            BroadcastVisionCube(player.CellPos, packet);
        }
    }
}
