using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using static System.Net.Mime.MediaTypeNames;

public class PacketHandler
{
    public static void S_EnterGameHandler(PacketSession session, IMessage packet)
    {
        S_EnterGame enterGamePacket = packet as S_EnterGame;
        Managers.Object.Add(enterGamePacket.Player, isMyPlayer: true);

        // 메서드 호출 예
        Debug.Log("Player entered the game.");
    }
    public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        S_LeaveGame leaveGamePacket = packet as S_LeaveGame;
        Managers.Object.Clear();

        Debug.Log($"S_LeaveGameHandler | {leaveGamePacket}");
    }
    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        S_Spawn spawnPacket = packet as S_Spawn;
        foreach (ObjectInfo player in spawnPacket.Objects)
        {
            Managers.Object.Add(player, isMyPlayer: false);
        }

        //Util.PacketLog<S_Spawn>($"S_SpawnHandler | {spawnPacket.Objects}");
    }
    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        S_Despawn despawnPacket = packet as S_Despawn;

        foreach (int id in despawnPacket.ObjectIds)
        {
            Managers.Object.Remove(id);
        }
        //Util.PacketLog<S_Despawn>($"S_DespawnHandler | {despawnPacket.ObjectIds}");
    }
    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        S_Move movePacket = packet as S_Move;
        ServerSession serverSession = session as ServerSession;

        GameObject go = Managers.Object.FindById(movePacket.ObjectId);
        if (go == null)
            return;
        if (Managers.Object.MyPlayer.Id == movePacket.ObjectId)
            return;
        BaseController bc = go.GetComponent<BaseController>();
        if (bc == null)
            return;

        bc.PosInfo = movePacket.PosInfo;

        //Util.PacketLog<S_Move>($"S_MoveHandler |  Player_{movePacket.ObjectId} ({movePacket.PosInfo.PosX},{movePacket.PosInfo.PosY})");
    }

    public static void S_SkillHandler(PacketSession session, IMessage packet)
    {
        S_Skill skillPacket = packet as S_Skill;
        ServerSession serverSession = session as ServerSession;

        GameObject go = Managers.Object.FindById(skillPacket.ObjectId);
        if (go == null)
        {
            Debug.LogError("스킬을 사용한 오브젝트를 찾을 수 없음");
            return;

        }

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc != null)
        {
            cc.UseSkill(skillPacket.Info.SkillId);
        }
        else
        {
            Debug.LogError("스킬을 사용한 플레이어를 찾을 수 없음");
        }
        //Util.PacketLog<S_Skill>($"S_SkillHandler |  Object_{skillPacket.ObjectId} ({skillPacket.Info.SkillId})");
    }

    public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
    {
        S_ChangeHp changeHpPacket = packet as S_ChangeHp;
        ServerSession serverSession = session as ServerSession;

        GameObject go = Managers.Object.FindById(changeHpPacket.ObjectId);
        if (go == null)
        {
            return;
        }

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc)
        {
            cc.Hp = changeHpPacket.Hp;
            //Util.PacketLog<S_ChangeHp>($"S_ChangehpHandler |  Object_{changeHpPacket.ObjectId} Changed HP : ({changeHpPacket.Hp})");
        }
    }

    public static void S_DieHandler(PacketSession session, IMessage packet)
    {
        S_Die diePacket = packet as S_Die;
        ServerSession serverSession = session as ServerSession;

        GameObject go = Managers.Object.FindById(diePacket.ObjectId);
        if (go == null)
        {
            return;
        }

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc)
        {
            cc.Hp = 0;
            cc.OnDead();
            //Util.PacketLog<S_Die>($"S_DieHandler |  Object_{diePacket.ObjectId} Attacker_({diePacket.AttackerId})");
        }
    }

    public static void S_ConnectedHandler(PacketSession session, IMessage packet)
    {
        C_Login loginPacket = new C_Login();
        string id = Application.dataPath;
        loginPacket.UniqueId = id.GetHashCode().ToString();
        Managers.Network.Send(loginPacket);
        Debug.Log($"S_ConnectedHandler {loginPacket.UniqueId}");
    }

    public static void S_LoginHandler(PacketSession session, IMessage packet)
    {

        S_Login loginPacket = packet as S_Login;
        // 조건부 연산자를 사용하지 않고, 분기를 명시적으로 처리
        string loginStatus = (loginPacket.LoginOk == 1) ? "ok" : "banned";
        Debug.Log("S_LoginHandler loginPacket : " + loginStatus);

        // TODO : 로비 UI에서 캐릭터 보여주고 선택할수록
        if (loginPacket.Players == null || loginPacket.Players.Count == 0)
        {
            C_CreatePlayer createPlayerPacket = new C_CreatePlayer();
            createPlayerPacket.Name = $"Player_{UnityEngine.Random.Range(0, 10000000).ToString()}";
            Managers.Network.Send(createPlayerPacket);
        }
        else
        {
            // 무조건 첫번째 로긴
            LobbyPlayerInfo info = loginPacket.Players[0];
            C_EnterGame enterGamePacket = new C_EnterGame();
            enterGamePacket.Name = info.Name;
            Managers.Network.Send(enterGamePacket);
        }
    }

    public static void S_CreatePlayerHandler(PacketSession session, IMessage packet)
    {
        S_CreatePlayer createPlayerPacket = packet as S_CreatePlayer;

        if (createPlayerPacket.Player == null)
        {
            C_CreatePlayer createPacket = new C_CreatePlayer();
            createPacket.Name = $"Player_{UnityEngine.Random.Range(0, 10000000).ToString()}";
            Managers.Network.Send(createPlayerPacket);
            Debug.Log($"Create Player {createPacket.Name}");
        }
        else
        {
            C_EnterGame enterGamePacket = new C_EnterGame();
            enterGamePacket.Name = createPlayerPacket.Player.Name;
            Managers.Network.Send(enterGamePacket);
            Debug.Log($"Enter Player {enterGamePacket.Name}");
        }
    }

    public static void S_ItemListHandler(PacketSession session, IMessage packet)
    {
        S_ItemList itemList = (S_ItemList)packet;

        Managers.Inven.Clear();

        // 메모리에 아이템 정보 적용
        foreach (ItemInfo itemInfo in itemList.Items)
        {
            Item item = Item.MakeItem(itemInfo);
            Managers.Inven.Add(item);
        }

        if (Managers.Object.MyPlayer != null)
        {
            Managers.Object.MyPlayer.RefreshAddtionalStat();
        }
    }

    public static void S_AddItemHandler(PacketSession session, IMessage packet)
    {
        S_AddItem itemList = (S_AddItem)packet;

        // 메모리에 아이템 정보 적용
        foreach (ItemInfo itemInfo in itemList.Items)
        {
            Item item = Item.MakeItem(itemInfo);
            Managers.Inven.Add(item);
        }

        Debug.Log("아이템을 획득했습니다!");
        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        gameSceneUI.InvenUI.RefreshUI();
        gameSceneUI.StatUI.RefreshUI();

        if (Managers.Object.MyPlayer != null)
        {
            Managers.Object.MyPlayer.RefreshAddtionalStat();
        }
    }

    public static void S_EquipItemHandler(PacketSession session, IMessage packet)
    {
        S_EquipItem equipItemOk = (S_EquipItem)packet;

        // 메모리에 아이템 정보 적용
        Item item = Managers.Inven.Get(equipItemOk.ItemDbId);
        if (item == null)
            return;

        item.Equipped = equipItemOk.Equipped;
        Debug.Log("아이템 착용 변경!");

        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        gameSceneUI.InvenUI.RefreshUI();
        gameSceneUI.StatUI.RefreshUI();

        if (Managers.Object.MyPlayer != null)
        {
            Managers.Object.MyPlayer.RefreshAddtionalStat();
        }
    }
    public static void S_ChangeStatHandler(PacketSession session, IMessage packet)
    {
        S_ChangeStat statPacket = (S_ChangeStat)packet;

        Debug.Log("스텟 변경 획득했습니다!");
    }

    internal static void S_PingHandler(PacketSession session, IMessage message)
    {
        C_Pong pongPacket = new C_Pong();
        Debug.Log("[Server] pingCheck");
        Managers.Network.Send(pongPacket);
    }
}
