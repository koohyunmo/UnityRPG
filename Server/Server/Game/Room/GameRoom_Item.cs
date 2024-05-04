using Google.Protobuf;
using Google.Protobuf.Protocol;
using Microsoft.VisualBasic;
using Server.Data;
using Server.DB;
using Server.Object;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Numerics;
using System.Text;

namespace Server.Game
{
    public partial class GameRoom : JobSerializer
    {
        public void HandleEquipItem(Player player, C_EquipItem equipPacket)
        {
            if (player == null)
                return;

            player.HandleEquipItem(equipPacket);

        }

        public void HandleItemRefresh(Player player, C_ChangeItem packet)
        {
            if (player == null)
                return;

            MyDbTransaction.SyncAndSendPlayerInventory(player);
        }
    }
}
