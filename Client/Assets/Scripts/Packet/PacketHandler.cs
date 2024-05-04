using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

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
		{
			return;
		}

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
		//string id = Application.dataPath;
		string id = Managers.Network.AccountId.ToString();
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
			Managers.Network.PlayerName = createPlayerPacket.Name;
		}
		else
		{
			// 무조건 첫번째 로긴
			LobbyPlayerInfo info = loginPacket.Players[0];
			C_EnterGame enterGamePacket = new C_EnterGame();
			enterGamePacket.Name = info.Name;
			Managers.Network.Send(enterGamePacket);

			Managers.Network.PlayerName = info.Name;
			Managers.Network.PlayerUniqueId = info.PlayerDbId;
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

		if(Managers.Object.MyPlayer != null)
		{
			Managers.Object.MyPlayer.RefreshAddtionalStat();
		}

		UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
		gameSceneUI.InvenUI.ReMakeUI();

		Debug.Log("아이템 새로고침");
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

	public static void S_PingHandler(PacketSession session, IMessage message)
    {
        C_Pong pongPacket = new C_Pong();
		Debug.Log("[Server] pingCheck");
		Managers.Network.Send(pongPacket);
    }

    public static void S_TeleportHandler(PacketSession session, IMessage packet)
    {
		S_Teleport teleport = packet as S_Teleport;
		ServerSession serverSession = session as ServerSession;
		Debug.Log($"Teleport {teleport.MapId}");
		Managers.Map.LoadMap(teleport.MapId);
	}

    public static void S_ChangeItemHandler(PacketSession session, IMessage message)
    {
		Debug.Log("TODO");
    }

    public static void S_ResMarketListHandler(PacketSession session, IMessage packet)
    {
		S_ResMarketList s_ResMarketListPacket = packet as S_ResMarketList;

		if(s_ResMarketListPacket.MarketItems != null && s_ResMarketListPacket.MarketItems.Count > 0)
		{
			List<MarketItem> list = s_ResMarketListPacket.MarketItems.ToList();
			Managers.Market.RefreshItems(list);
		}
		else
		{
			Managers.Market.RefreshItems(null);
			Debug.Log("상점이 빔");
		}

	}

    public static void S_MarketItemResisterHandler(PacketSession session, IMessage packet)
    {
		S_MarketItemResister s_MarketItemResister = packet as S_MarketItemResister;

		if(s_MarketItemResister.ItemResisterOk)
		{
			Debug.Log("아이템 등록 성공");
			// 아이템 변경 노티
			C_ChangeItem c_ChangeItem = new C_ChangeItem();
			Managers.Network.Send(c_ChangeItem);
		}

	}

    public static void S_MarketItemPurchaseHandler(PacketSession session, IMessage packet)
    {
        S_MarketItemPurchase s_MarketItemPurchase = packet as S_MarketItemPurchase;

		if(s_MarketItemPurchase.ItemPurchaseOk)
		{
			C_ReqMarketList c_ReqMarketList = new C_ReqMarketList();
			Managers.Network.Send(c_ReqMarketList);
			Debug.Log("구매성공");
		}
		else
		{
			C_ReqMarketList c_ReqMarketList = new C_ReqMarketList();
			Managers.Network.Send(c_ReqMarketList);
			Debug.Log("구매실패");
		}

	}

    public static void S_MailItemListHandler(PacketSession session, IMessage packet)
    {
		S_MailItemList s_MailItemList = packet as S_MailItemList;
		if(s_MailItemList.MailItems != null && s_MailItemList.MailItems.Count > 0)
		{
			List<MailItem> list = s_MailItemList.MailItems.ToList();
			Managers.Mail.SetMailItemList(list);
			Debug.Log("메일 리셋");
		}
		else
		{
			Managers.Mail.SetMailItemList(null);
			Debug.Log("메일 빈칸");
		}
    }

	public static void S_MailItemReceiveHandler(PacketSession session, IMessage message)
    {
		S_MailItemReceive s_ResMarketList = message as S_MailItemReceive;
		if(s_ResMarketList.MailItemReceiveOk)
		{
			C_MailItemList c_MailItemList = new C_MailItemList();
			Managers.Network.Send(c_MailItemList);
			Debug.Log("노티 메일 리셋");
		}
    }

	public static void S_ChatHandler(PacketSession session, IMessage message)
    {
        S_Chat s_Chat = message as S_Chat;
		ServerSession serverSession = session as ServerSession;

		// localChat
		if(s_Chat.ChatInfo.PlayerName.Equals(Managers.Network.PlayerName))
		{
			Debug.Log($"Local Chat | {s_Chat.ChatInfo.Chat}");
		}
		Managers.Chat.AddRoomChat(s_Chat.ChatInfo.PlayerName, s_Chat.ChatInfo.Chat);
		//TODO Room Chat
		Debug.Log($"Room Chat | {s_Chat.ChatInfo.Chat}");
	}

    public static void S_ChatSpawnHandler(PacketSession session, IMessage message)
    {
       S_ChatSpawn s_ChatSpawn= message as S_ChatSpawn;
		Managers.Chat.AddLocalChat(s_ChatSpawn.SenderId, s_ChatSpawn.ChatInfo);
	}
}
