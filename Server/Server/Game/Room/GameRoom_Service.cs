using Google.Protobuf.Protocol;
using Server.Data;
using Server.DB;
using Server.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void HandleChat(Player player, GameRoom room, C_Chat packet)
        {
            if (player == null)
                return;
            if (room == null)
                return;

 


        }
    }
}
