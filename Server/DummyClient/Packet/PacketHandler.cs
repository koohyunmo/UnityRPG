using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

public class PacketHandler
{
    public static void S_EnterGameHandler(PacketSession session, IMessage packet)
    {
        S_EnterGame enterGamePacket = packet as S_EnterGame;
    }
    public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        S_LeaveGame leaveGamePacket = packet as S_LeaveGame;
    }
    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        S_Spawn spawnPacket = packet as S_Spawn;
    }
    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        S_Despawn despawnPacket = packet as S_Despawn;

    }
    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        S_Move movePacket = packet as S_Move;
    }

    public static void S_SkillHandler(PacketSession session, IMessage packet)
    {
        S_Skill skillPacket = packet as S_Skill;
    }

    public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
    {
        S_ChangeHp changeHpPacket = packet as S_ChangeHp;
    }

    public static void S_DieHandler(PacketSession session, IMessage packet)
    {
        S_Die diePacket = packet as S_Die;
    }
    // Step_1
    public static void S_ConnectedHandler(PacketSession session, IMessage packet)
    {
        C_Login loginPacket = new C_Login();
        ServerSession serverSession = (ServerSession)session;
        loginPacket.UniqueId = $"DummyClient_{serverSession.DummyId.ToString("0000")}";
        serverSession.Send(loginPacket);
        Console.Write($"S_ConnectedHandler {loginPacket.UniqueId}");
    }

    // Step_2
    public static void S_LoginHandler(PacketSession session, IMessage packet)
    {

        S_Login loginPacket = packet as S_Login;
        ServerSession serverSession = (ServerSession)session;
        // 조건부 연산자를 사용하지 않고, 분기를 명시적으로 처리
        //string loginStatus = (loginPacket.LoginOk == 1) ? "ok" : "banned";
        //Console.Write (loginStatus + " ");

        // TODO : 로비 UI에서 캐릭터 보여주고 선택할수록
        if (loginPacket.Players == null || loginPacket.Players.Count == 0)
        {
            C_CreatePlayer createPlayerPacket = new C_CreatePlayer();
            createPlayerPacket.Name = $"DummyClient_{serverSession.DummyId.ToString("0000")}";
            serverSession.Send(createPlayerPacket);
        }
        else
        {
            // 무조건 첫번째 로긴
            LobbyPlayerInfo info = loginPacket.Players[0];
            C_EnterGame enterGamePacket = new C_EnterGame();
            enterGamePacket.Name = info.Name;
            serverSession.Send(enterGamePacket);
        }
    }
    // Step_3
    public static void S_CreatePlayerHandler(PacketSession session, IMessage packet)
    {
        S_CreatePlayer createPlayerPacket = packet as S_CreatePlayer;
        ServerSession serverSession = (ServerSession)session;

        if (createPlayerPacket.Player == null)
        {
            // 생략
        }
        else
        {
            C_EnterGame enterGamePacket = new C_EnterGame();
            enterGamePacket.Name = createPlayerPacket.Player.Name;
            serverSession.Send(enterGamePacket);
            Console.WriteLine($"Enter Player {enterGamePacket.Name}");
        }
    }

    public static void S_ItemListHandler(PacketSession session, IMessage packet)
    {
        S_ItemList itemList = (S_ItemList)packet;
    }

    public static void S_AddItemHandler(PacketSession session, IMessage packet)
    {
        S_AddItem itemList = (S_AddItem)packet;
    }

    public static void S_EquipItemHandler(PacketSession session, IMessage packet)
    {
        S_EquipItem equipItemOk = (S_EquipItem)packet;

    }
    public static void S_ChangeStatHandler(PacketSession session, IMessage packet)
    {
        S_ChangeStat statPacket = (S_ChangeStat)packet;
    }

    public static void S_PingHandler(PacketSession session, IMessage message)
    {
        C_Pong pongPacket = new C_Pong();
    }

    public static void S_TeleportHandler(PacketSession session, IMessage message)
    {
       C_Teleport c_Teleport = new C_Teleport();
    }

    public static void S_ChangeItemHandler(PacketSession session, IMessage message)
    {
        C_ChangeItem c_ChangeItem = new C_ChangeItem();
    }

    public static void S_ResMarketListHandler(PacketSession session, IMessage message)
    {
        C_ReqMarketList c_ChangeItem = new C_ReqMarketList();
    }

    public static void S_MarketItemResisterHandler(PacketSession session, IMessage message)
    {
        S_MarketItemResister s_MarketItemResister = new S_MarketItemResister();
    }

    public static void S_MarketItemPurchaseHandler(PacketSession session, IMessage message)
    {
        S_MarketItemPurchase s_MarketItemPurchase = new S_MarketItemPurchase();
    }

    public static void S_MailItemListHandler(PacketSession session, IMessage message)
    {
       S_ResMarketList s_ResMarketList = new S_ResMarketList();
    }

    public static void S_MailItemReceiveHandler(PacketSession session, IMessage message)
    {
        S_MailItemReceive s_MailItemReceive = new S_MailItemReceive();
    }

    internal static void S_MarketItemSearchHandler(PacketSession session, IMessage message)
    {

    }

    internal static void S_ChatHandler(PacketSession session, IMessage message)
    {

    }

    public static void S_ChatSpawnHandler(PacketSession session, IMessage message)
    {

    }

    internal static void S_ChatDespawnHandler(PacketSession session, IMessage message)
    {


    }

    internal static void S_SpawnDamageHandler(PacketSession session, IMessage message)
    {

    }

    internal static void S_GoldChangeHandler(PacketSession session, IMessage message)
    {
    }

    internal static void S_ExpChangeHandler(PacketSession session, IMessage message)
    {
    }

    internal static void S_ExitGameHandler(PacketSession session, IMessage message)
    {
    }

    internal static void S_MarketItemDeleteHandler(PacketSession session, IMessage message)
    {
    }

    internal static void S_ItemSlotChangeHandler(PacketSession session, IMessage message)
    {
        throw new NotImplementedException();
    }
}
