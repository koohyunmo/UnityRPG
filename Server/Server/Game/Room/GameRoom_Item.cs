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
using System.Diagnostics;
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

        public void HandleItemSlotChange(Player player, C_ItemSlotChange packet)
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

        public void HandleUseItem(Player player, C_ItemUse packet)
        {
            if (player == null || packet == null || player.Room == null)
                return;

            player.Heal(50);
            player.Room.Push(HandleUseItemStep_2, player, packet);
        }
        // DB 병목이있음
        // 메모리에서만 처리하고 DB 접근은 안하도록 바꿔야함
        public void HandleUseItemStep_2(Player player, C_ItemUse packet)
        {
            if (player == null || packet == null || player.Room == null)
                return;

            using (AppDbContext db = new AppDbContext())
            {
                var itemDb = db.Items.Where(i => i.OnwerDbId == player.PlayerDbId && i.ItemDbId == packet.ItemDbId).FirstOrDefault();
                if (itemDb == null)
                    return;

                itemDb.Count -= 1;
                if(itemDb.Count <= 0 )
                {
                    db.Items.Remove(itemDb);
                }
                bool success = db.SaveChangesEx();
                if (success)
                {
                    S_ItemRefresh s_ItemRefresh = new S_ItemRefresh() { ItemInfo = new ItemInfo()};
                    s_ItemRefresh.ItemInfo.MergeFrom(Item.MakeItem(itemDb).Info);
                    player.Session.Send(s_ItemRefresh);

                    Console.WriteLine($"Player_{player.Id} Use : {itemDb.ItemDbId} | remain : {itemDb.Count}");
                }
            }

            player.Room.Push(HandleUseItemStep_3, player, packet);
        }

        public void HandleUseItemStep_3(Player player, C_ItemUse packet)
        {
            if (player == null || packet == null || player.Room == null)
                return;

            using (AppDbContext db = new AppDbContext())
            {
                var playerDb = db.Players.Where(p => p.PlayerDbId == player.PlayerDbId).FirstOrDefault();
                if (playerDb == null)
                    return;

                playerDb.Hp = player.Stat.Hp;
                bool success = db.SaveChangesEx();
                if (success)
                {
                    Console.WriteLine($"Player_{player.Id} : {player.Hp}");
                }
            }
        }


    }
}
