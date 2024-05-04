using Google.Protobuf;
using Google.Protobuf.Protocol;
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
        req.ItemDbId = purchasePacket.ItemDbId;
        req.BuyerId = player.PlayerDbId;
        req.SellerId = purchasePacket.SellerId;
        req.TemplateId = purchasePacket.TemplateId;
        req.Price = purchasePacket.Price;

        await MyAPIHandler.SendPostRequestMarket<PurchaseItemPacketRes>("market/purchase", req, (res) => {

            if (res.ItemPurchaseOk)
            {
                purchaseResPacket.ItemPurchaseOk = res.ItemPurchaseOk;
            }
            else
            {
                purchaseResPacket.ItemPurchaseOk = res.ItemPurchaseOk;
            }

            room.Push(room.Unicast, player, purchaseResPacket);
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
            await Console.Out.WriteLineAsync($"API 호출 문제 {ex}");
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

        S_ChatSpawn s_ChatSpawn = new S_ChatSpawn();
        s_ChatSpawn.ChatInfo = new ChatInfo();
        s_ChatSpawn.ChatInfo.MergeFrom(s_Chat.ChatInfo);
        s_ChatSpawn.SenderId = player.Id;

        room.Push(room.Broadcast, player, s_Chat);
        room.Push(room.BroadcastVisionCube, player.CellPos, s_ChatSpawn);

    }
}
