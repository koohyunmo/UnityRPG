using Google.Protobuf;
using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Server.Data;
using Server.DB;
using Server.Object;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
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
        // TODO 포션 사용
        public void HandleItemUse(Player player, C_EquipItem equipPacket)
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

        public void HandleChangeItem(Player player, C_ItemSlotChange packet)
        {
            if (player == null) return;

            if (packet.SelectedSlot == -1 || packet.TargetSlot == -1) return;

            bool success = false;

            using (AppDbContext db = new AppDbContext())
            {
                var transaction = db.Database.BeginTransaction();

                try
                {
                    var selectItem = db.Items.FirstOrDefault(i => i.OnwerDbId == player.PlayerDbId && i.ItemDbId == packet.SelectedItemDbId);
                    if (selectItem == null || selectItem.Slot != packet.SelectedSlot) return;

                    ItemDb targetItem = db.Items.FirstOrDefault(i => i.OnwerDbId == player.PlayerDbId && i.ItemDbId == packet.TargetItemDbId);
                    if (targetItem != null && targetItem.Slot != packet.TargetSlot) return;

                    // Swap slots
                    int tempSlot = selectItem.Slot;
                    selectItem.Slot = packet.TargetSlot;
                    if (targetItem != null) targetItem.Slot = tempSlot;

                    success = db.SaveChangesEx();
                    if (success)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }
                }
                catch (Exception e)
                {
                    success = false;
                    transaction.Rollback();
                    // Log or handle the exception
                    Console.WriteLine(e.Message);
                }
            }

            // Handle success or failure
            if (success)
            {
                MyDbTransaction.SyncAndSendPlayerInventory(player);
            }
            else
            {
                // Handle failure
            }
        }

        public void HandleDeleteItem(Player player, C_ItemDelete packet)
        {
            if (player == null) return;

            bool success = false;

            using (AppDbContext db = new AppDbContext())
            {
                var transaction = db.Database.BeginTransaction();

                try
                {
                    var deleteItem = db.Items.FirstOrDefault(i => i.OnwerDbId == player.PlayerDbId && i.ItemDbId == packet.ItemDbId);
                    db.Items.Remove(deleteItem);
                    success = db.SaveChangesEx();
                    if(success)
                    { 
                        transaction.Commit(); 
                    }
                    else
                    {
                        transaction.Rollback();
                    }
  
                }
                catch (Exception e)
                {
                    success = false;
                    transaction.Rollback();
                    // Log or handle the exception
                    Console.WriteLine(e.Message);
                }
            }

            // Handle success or failure
            if (success)
            {
                MyDbTransaction.SyncAndSendPlayerInventory(player);
            }
            else
            {
                // Handle failure
            }
        }

    }
}
