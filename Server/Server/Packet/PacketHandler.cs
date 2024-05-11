using Google.Protobuf;
using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.Data;
using Server.DB;
using Server.Game;
using Server.Object;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

class PacketHandler
{
    public static void C_MoveHandler(PacketSession session, IMessage packet)
    {
        C_Move movePacket = packet as C_Move;
        ClientSession clientSession = session as ClientSession;

        //Console.WriteLine($"C_Move ({movePacket.PosInfo.PosX}, {movePacket.PosInfo.PosY})");

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleMove, player, movePacket);
    }

    public static void C_SkillHandler(PacketSession session, IMessage packet)
    {
        C_Skill skillPacket = packet as C_Skill;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleSkill, player, skillPacket);
    }
    public static void C_LoginHandler(PacketSession session, IMessage packet)
    {
        C_Login loginPacket = packet as C_Login;
        ClientSession clientSession = session as ClientSession;

        clientSession.HandleLogin(loginPacket);

    }

    public static void C_CreatePlayerHandler(PacketSession session, IMessage packet)
    {
        C_CreatePlayer createPlayerPacket = (C_CreatePlayer)packet;
        ClientSession clientSession = (ClientSession)session;

        clientSession.HandleCreatePlayer(createPlayerPacket);
    }

    public static void C_EnterGameHandler(PacketSession session, IMessage packet)
    {
        C_EnterGame enterGamePacket = (C_EnterGame)packet;
        ClientSession clientSession = (ClientSession)session;

        Console.WriteLine("C_EnterGameHandler" + enterGamePacket.Name);

        clientSession.HandleEnterGame(enterGamePacket);
    }

    public static void C_EquipItemHandler(PacketSession session, IMessage packet)
    {
        C_EquipItem equipPacket = (C_EquipItem)packet;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;
        GameRoom room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleEquipItem, player, equipPacket);
    }

    public static void C_PongHandler(PacketSession session, IMessage packet)
    {
        ClientSession clientSession = (ClientSession)session;
        clientSession.HandlePong();
    }

    public static void C_TeleportHandler(PacketSession session, IMessage packet)
    {
        C_Teleport teleportPacket = (C_Teleport)packet;
        ClientSession clientSession = (ClientSession)session;


        Player player = clientSession.MyPlayer;
        if (player == null)
            return;
        GameRoom room = player.Room;
        if (room == null)
            return;
        Console.WriteLine($"포탈 패킷 Player_{player.Id}");

        room.Push(room.HandleTeleport, player,teleportPacket);


    }
    public static void C_ChangeItemHandler(PacketSession session, IMessage packet)
    {
        C_ChangeItem itemPacket = (C_ChangeItem)packet;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;
        GameRoom room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleItemRefresh, player, itemPacket);
    }

    public static async void C_ReqMarketListHandler(PacketSession session, IMessage packet)
    {
        C_ReqMarketList req = (C_ReqMarketList)packet;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;
        GameRoom room = player.Room;
        if (room == null)
            return;
        S_ResMarketList marketItemRes = new S_ResMarketList();

        await MyAPIHandler.SendGetRequestMarket<MarketItemsGetListRes>("market/list",new MarketItemsGetListReq(), (res) => {

            if (res.items != null && res.items.Count > 0)
            {
                foreach (var item in res.items)
                {
                    MarketItem newItem = new MarketItem()
                    {
                        ItemDbId = item.ItemDbId,
                        Price = item.Price,
                        SellerId = item.SellerId,
                        TemplateId = item.TemplateId
                    };

                    marketItemRes.MarketItems.Add(newItem);

                }
            }
            else
            {
                Console.WriteLine("아이템 없음");
            }

            room.Push(room.Unicast, player, marketItemRes);

        });


    }

    public static async void C_MarketItemResisterHandler(PacketSession session, IMessage packet)
    {
        C_MarketItemResister resisterReqPacket = (C_MarketItemResister)packet;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;
        GameRoom room = player.Room;
        if (room == null)
            return;

        S_MarketItemResister resisterResPacket = new S_MarketItemResister();
        ResisterItemPacketReq req = new ResisterItemPacketReq();
        // 플레이어 OBJ 추가 알림용
        req.SellerObjId = player.Id;
        req.ItemDbId = resisterReqPacket.ItemDbId;
        req.TemplateId = resisterReqPacket.TemplateId;
        req.SellerId = player.PlayerDbId;
        req.Price = resisterReqPacket.Price;
        req.ItemName = resisterReqPacket.ItemName;

        //await Console.Out.WriteLineAsync($"{req.ItemDbId} : {req.TemplateId} : {req.SellerId} : {req.Price}");

        await MyAPIHandler.SendPostRequestMarket<ResisterItemPacketRes>("market/register", req, (res) => {

            if (res.ItemResiterOk)
            {
                resisterResPacket.ItemResisterOk = res.ItemResiterOk;
            }
            else
            {
                resisterResPacket.ItemResisterOk = res.ItemResiterOk;
            }

            room.Push(room.Unicast, player, resisterResPacket);
        });
    }

    public static async void C_MarketItemPurchaseHandler(PacketSession session, IMessage packet)
    {
        C_MarketItemPurchase purchasePacket = (C_MarketItemPurchase)packet;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;
        GameRoom room = player.Room;
        if (room == null)
            return;

        S_MarketItemPurchase purchaseResPacket = new S_MarketItemPurchase();
        PurchaseItemPacketReq req = new PurchaseItemPacketReq();
        req.BuyerObjId = player.PlayerDbId;
        req.ItemDbId = purchasePacket.ItemDbId;
        req.BuyerId = player.PlayerDbId;
        req.SellerId = purchasePacket.SellerId;
        req.TemplateId = purchasePacket.TemplateId;
        req.Price = purchasePacket.Price;

        await MyAPIHandler.SendPostRequestMarket<PurchaseItemPacketRes>("market/purchase", req, (res) => {

            if (res.ItemPurchaseOk)
            {
                purchaseResPacket.ItemPurchaseOk = res.ItemPurchaseOk;

                // 5-11 고침
                using (AppDbContext db = new AppDbContext())
                {
                    PlayerDb buyerDb = db.Players.AsNoTracking().Where(p => p.PlayerDbId == purchasePacket.BuyerId).FirstOrDefault();
                    if (buyerDb != null)
                    {
                        S_GoldChange goldChangePacket = new S_GoldChange();
                        goldChangePacket.Gold = buyerDb.Gold;
                        player.Session.Send(goldChangePacket);
                    }

                    var sellerPlayer = ObjectManager.Instance.Find(res.SellerObjId);
                    if(sellerPlayer != null)
                    {
                        PlayerDb seller = db.Players.AsNoTracking().Where(p => p.PlayerDbId == purchasePacket.SellerId).FirstOrDefault();
                        if (seller != null)
                        {
                            S_GoldChange goldChangePacket = new S_GoldChange();
                            goldChangePacket.Gold = seller.Gold;
                            sellerPlayer.Session.Send(goldChangePacket);
                        }
                    }
                }
            }
            else
            {
                purchaseResPacket.ItemPurchaseOk = false;
            }

            room.Push(room.Unicast, player, purchaseResPacket);

        });
    }
    public static async void C_MarketItemDeleteHandler(PacketSession session, IMessage packet)
    {
        C_MarketItemDelete deletePacket = (C_MarketItemDelete)packet;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;
        GameRoom room = player.Room;
        if (room == null)
            return;

        S_MarketItemDelete s_MarketItemDelete = new S_MarketItemDelete();
        DeleteItemPacketReq req = new DeleteItemPacketReq();
        req.ItemId = deletePacket.Item.ItemDbId;
        req.SellerId = deletePacket.Item.SellerId;
        req.TemplateId = deletePacket.Item.TemplateId;
        req.BuyerId = player.PlayerDbId;

        await MyAPIHandler.SendPostRequestMarket<DeleteItemPacketRes>("market/delete", req, (res) => {

            if (res.DeleteOk)
            {
                s_MarketItemDelete.DeleteOk = res.DeleteOk;
            }
            else
            {
                s_MarketItemDelete.DeleteOk = false;
            }

            room.Push(room.Unicast, player, s_MarketItemDelete);
        });
    }

    public static void C_MailItemListHandler(PacketSession session, IMessage packet)
    {
        C_MailItemList mailPacket = (C_MailItemList)packet;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;
        GameRoom room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleMailItemList, player, mailPacket);
    }

    public static void C_MailItemReceiveHandler(PacketSession session, IMessage packet)
    {
        C_MailItemReceive receivePacket = (C_MailItemReceive)packet;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;
        GameRoom room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleMailItemReceive, player,room, receivePacket);
    }

    public static async void C_MarketItemSearchHandler(PacketSession session, IMessage packet)
    {
        C_MarketItemSearch searchPacket = (C_MarketItemSearch)packet;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;
        GameRoom room = player.Room;
        if (room == null)
            return;

        S_ResMarketList marketItemRes = new S_ResMarketList();
        MarketItemsGetSearchListReq req = new MarketItemsGetSearchListReq();
        req.SearchName = new string(searchPacket.Name.ToCharArray());

        try
        {
            await Console.Out.WriteLineAsync($"{req.SearchName} 검색할 이름");

            await MyAPIHandler.SendGetRequestMarket<MarketItemsGetListRes>("market/search", req.SearchName, (res) =>
            {

                if (res.items != null && res.items.Count > 0)
                {
                    foreach (var item in res.items)
                    {
                        MarketItem newItem = new MarketItem()
                        {
                            ItemDbId = item.ItemDbId,
                            Price = item.Price,
                            SellerId = item.SellerId,
                            TemplateId = item.TemplateId,
                            SellerName = item.SellerName
                        };
                        marketItemRes.MarketItems.Add(newItem);

                    }
                }
                else
                {
                    Console.WriteLine($"{req.SearchName}으로 검색된 아이템 없음");
                }

                room.Push(room.Unicast, player, marketItemRes);
            });
        }catch (Exception ex) 
        {
            await Console.Out.WriteLineAsync($"market/search API 호출 문제 {ex}");
        }

    }

    public static void C_ChatHandler(PacketSession session, IMessage packet)
    {

        C_Chat c_Chat = (C_Chat)packet;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;
        GameRoom room = player.Room;
        if (room == null)
            return;

        S_Chat s_Chat = new S_Chat();
        s_Chat.ChatInfo = new ChatInfo();
        s_Chat.ChatInfo.PlayerName = player.Info.Name;
        s_Chat.ChatInfo.Chat = c_Chat.Message;

        Random random = new Random();
        int rndId = random.Next(int.MinValue, int.MaxValue);

        
        S_ChatSpawn s_ChatSpawn = new S_ChatSpawn();
        s_ChatSpawn.ChatInfo = new ChatInfo();
        s_ChatSpawn.ChatInfo.MergeFrom(s_Chat.ChatInfo);
        s_ChatSpawn.SenderId = player.Id; ;
        s_ChatSpawn.ChatId = rndId;

        room.Push(room.Broadcast, s_Chat);
        room.Push(room.HandleChat, player,room, s_ChatSpawn);
        //room.PushAfter(5000,room.BroadcastVisionCube, player.CellPos, s_ChatDespawn);
        //room.Push(room.Broadcast, player, s_ChatSpawn);
        //room.PushAfter(5000, room.Broadcast, player, s_ChatDespawn);

    }

    public static void C_ExitGameHandler(PacketSession session, IMessage message)
    {
        C_ExitGame exitPacket = (C_ExitGame)message;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;
        GameRoom room = player.Room;
        if (room == null)
            return;


        S_ExitGame s_ExitGame = new S_ExitGame();
        s_ExitGame.Exit = exitPacket.Exit;

        room.Push(room.LeaveGame, player.Id);
        player.Session.Send(s_ExitGame);
        clientSession.Disconnect();
    }

    public static void C_ItemSlotChangeHandler(PacketSession session, IMessage message)
    {
        C_ItemSlotChange changeItemPacket = (C_ItemSlotChange)message;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;
        GameRoom room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleChangeItem, player, changeItemPacket);
        
    }

    public static void C_ItemDeleteHandler(PacketSession session, IMessage message)
    {
        C_ItemDelete itemDeletePacket = (C_ItemDelete)message;
        ClientSession clientSession = (ClientSession)session;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;
        GameRoom room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleDeleteItem, player, itemDeletePacket);

    }
}
