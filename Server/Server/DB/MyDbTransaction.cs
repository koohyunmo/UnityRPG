using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Game;
using Server.Migrations;
using Server.Object;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Server.DB
{
    public partial class MyDbTransaction : JobSerializer
    {
        public static MyDbTransaction Instance { get; } = new MyDbTransaction();

        // Me(GameRoom) -> You (Db) -> Me (GameRoom)
        public static void SavePlayerStatus_AllInOne(Player player, GameRoom room)
        {
            if (player == null || room == null)
                return;

            // Me
            PlayerDb playerDb = new PlayerDb();
            playerDb.PlayerDbId = player.PlayerDbId;
            playerDb.Hp = player.Stat.Hp;

            using (AppDbContext db = new AppDbContext())
            {
                // You
                Instance.Push(() =>
                {
                    db.Entry(playerDb).State = EntityState.Unchanged;
                    db.Entry(playerDb).Property(nameof(PlayerDb.Hp)).IsModified = true;
                    if (db.SaveChangesEx())
                    {
                        room.Push(() => Console.WriteLine($"Db Save Player {playerDb.Hp}"));
                    }
                });


            }
        }

        public static void SavePlayerStatus_Step1(Player player, GameRoom room)
        {
            if (player == null || room == null)
                return;

            // Me
            PlayerDb playerDb = new PlayerDb();
            playerDb.PlayerDbId = player.PlayerDbId;
            playerDb.Hp = player.Stat.Hp;
            Instance.Push<PlayerDb, GameRoom>(SavePlayerStatus_Step2, playerDb, room);
        }

        public static void SavePlayerStatus_Step2(PlayerDb playerDb, GameRoom room)
        {
            if (playerDb == null || room == null)
                return;


            using (AppDbContext db = new AppDbContext())
            {
                //You
                db.Entry(playerDb).State = EntityState.Unchanged;
                db.Entry(playerDb).Property(nameof(PlayerDb.Hp)).IsModified = true;
                bool success = db.SaveChangesEx();
                if (success)
                {
                    room.Push(SavePlayerStatus_Step3, playerDb.Hp);
                }
            }
        }

        public static void SavePlayerStatus_Step3(int hp)
        {
            Console.WriteLine($"Db Save Player {hp}");
        }

        public static void RewardPlayer(Player player, RewardData rewardData, GameRoom room)
        {
            if (player == null)
            {
                Console.WriteLine("Player is null");
                return;
            }

            if (rewardData == null)
            {
                Console.WriteLine("rewardData is null");
                return;
            }
            if (room == null)
            {
                Console.WriteLine("room is null");
                return;
            }


            // TODO : 살짝 문제가 있긴 하다...
            int? slot = player.Inven.GetEmptySlot();
            if (slot == null)
            {
                Console.WriteLine($"Player_{player.Info.Name}[{player.Id}] 인베토리가 꽉참");
                return;
            }


            ItemDb itemDb = new ItemDb()
            {
                TemplateId = rewardData.itemId,
                Count = rewardData.count,
                Slot = slot.Value,
                OnwerDbId = player.PlayerDbId
            };

            // You
            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    db.Items.Add(itemDb);
                    bool success = db.SaveChangesEx();
                    if (success)
                    {
                        // Me
                        room.Push(() =>
                        {
                            Item newItem = Item.MakeItem(itemDb);
                            player.Inven.Add(newItem);

                            // Client Noti
                            {
                                S_AddItem itemPacket = new S_AddItem();
                                ItemInfo itemInfo = new ItemInfo();
                                itemInfo.MergeFrom(newItem.Info);
                                itemPacket.Items.Add(itemInfo);

                                player.Session.Send(itemPacket);
                            }

                            Console.WriteLine($"Player_{player.Info.Name}[{player.Id}] Get Item_{newItem.ItemDbId} :  {newItem.TemplateId}");
                        });
                    }
                }
            });

        }
        public static void ReceiveMailItemToPlayer(Player player, MailDb mailDb, GameRoom room)
        {
            if (player == null)
            {
                Console.WriteLine("Player is null");
                return;
            }

            if (mailDb == null)
            {
                Console.WriteLine("rewardData is null");
                return;
            }
            if (room == null)
            {
                Console.WriteLine("room is null");
                return;
            }


            // TODO : 살짝 문제가 있긴 하다...
            int? slot = player.Inven.GetEmptySlot();
            if (slot == null)
            {
                Console.WriteLine($"Player_{player.Info.Name}[{player.Id}] 인베토리가 꽉참");
                return;
            }

            ItemDb itemDb = new ItemDb()
            {
                TemplateId = mailDb.TemplateId,
                Count = 1,
                Slot = slot.Value,
                OnwerDbId = player.PlayerDbId
            };

            try
            {
                // You
                Instance.Push(() =>
                {
                    using (AppDbContext db = new AppDbContext())
                    {
                        using var transaction1 = db.Database.BeginTransaction();

                        var mail = db.Mails
                            .AsNoTracking()
                            .Where(m => m.MailDbId == mailDb.MailDbId && m.TemplateId == mailDb.TemplateId && mailDb.Read == false)
                            .FirstOrDefault();

                        if (mail == null) return;
                        if (mail.Read == true) return;

                        mail.Read = true;
                        db.Mails.Remove(mail);
                        bool success1 = db.SaveChangesEx();
                        db.Items.Add(itemDb);
                        success1 = db.SaveChangesEx();
                        if (success1)
                        {
                            transaction1.Commit();
                            room.Push(() =>
                            {
                                Item newItem = Item.MakeItem(itemDb);
                                player.Inven.Add(newItem);

                                // Client Noti
                                {
                                    S_AddItem itemPacket = new S_AddItem();
                                    ItemInfo itemInfo = new ItemInfo();
                                    itemInfo.MergeFrom(newItem.Info);
                                    itemPacket.Items.Add(itemInfo);

                                    player.Session.Send(itemPacket);
                                }

                                Console.WriteLine($"Player_{player.Info.Name}[{player.Id}] Get Item_{newItem.ItemDbId} :  {newItem.TemplateId}");
                            });

                            S_MailItemReceive res = new S_MailItemReceive();
                            res.MailItemReceiveOk = true;
                            room.Push(room.Unicast, player, res);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static void RewardGold(Player player, int rewardGold, GameRoom room)
        {

            if (player == null)
            {
                Console.WriteLine("Player is null");
                return;
            }
            if (room == null)
            {
                Console.WriteLine("room is null");
                return;
            }

            // You
            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    var playerDb = db.Players.Where(p => player.Info.Name == p.PlayerName).FirstOrDefault();
                    if (player == null)
                        return;

                    playerDb.Gold += rewardGold;
                    bool success = db.SaveChangesEx();
                    if (success)
                    {
                        // Me
                        room.Push(() =>
                        {
                            // Client Noti
                            {
                                Console.WriteLine("TODO Client Notiy");
                            }
                        });
                    }
                }
            });

        }


        public static void SyncAndSendPlayerInventory(Player player)
        {
            if (player == null)
            {
                Console.WriteLine("Player is null");
                return;
            }

            // 비동기 스케줄러에 작업 등록
            Instance.Push(() => 
            {
                using (AppDbContext db = new AppDbContext())
                {
                    // DB에서 아이템 정보를 가져옴
                    List<ItemDb> itemsFromDb = db.Items
                        .Where(i => i.OnwerDbId == player.PlayerDbId)
                        .ToList();

                    // 메모리 상의 플레이어 인벤토리를 클리어하고 새로운 데이터로 채움

                    lock (player.Inven.Items) // 플레이어의 인벤토리에 대한 락
                    {
                        player.Inven.Items.Clear();

                        foreach (ItemDb itemDb in itemsFromDb)
                        {
                            Item newItem = Item.MakeItem(itemDb);
                            player.Inven.Add(newItem);
                        }

                    }

                    // 아이템 리스트 패킷 생성 및 전송
                    S_ItemList itemListPacket = new S_ItemList();
                    foreach (Item item in player.Inven.Items.Values)
                    {
                        ItemInfo itemInfo = new ItemInfo
                        {
                            ItemDbId = item.ItemDbId,
                            TemplateId = item.TemplateId,
                            Count = item.Count,
                            Slot = item.Slot,
                            Equipped = item.Equipped,
                        };
                        itemListPacket.Items.Add(itemInfo);
                    }

                    player.Session.Send(itemListPacket);

                    Console.WriteLine($"Synced and sent DB item list to Player_{player.Info.Name}[{player.Id}] with {player.Inven.Items.Count} items.");
                }
            });

        }

    }
}
