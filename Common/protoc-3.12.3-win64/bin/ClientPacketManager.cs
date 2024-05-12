using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();
		
	public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

	public void Register()
	{		
		_onRecv.Add((ushort)MsgId.SEnterGame, MakePacket<S_EnterGame>);
		_handler.Add((ushort)MsgId.SEnterGame, PacketHandler.S_EnterGameHandler);		
		_onRecv.Add((ushort)MsgId.SLeaveGame, MakePacket<S_LeaveGame>);
		_handler.Add((ushort)MsgId.SLeaveGame, PacketHandler.S_LeaveGameHandler);		
		_onRecv.Add((ushort)MsgId.SSpawn, MakePacket<S_Spawn>);
		_handler.Add((ushort)MsgId.SSpawn, PacketHandler.S_SpawnHandler);		
		_onRecv.Add((ushort)MsgId.SDespawn, MakePacket<S_Despawn>);
		_handler.Add((ushort)MsgId.SDespawn, PacketHandler.S_DespawnHandler);		
		_onRecv.Add((ushort)MsgId.SMove, MakePacket<S_Move>);
		_handler.Add((ushort)MsgId.SMove, PacketHandler.S_MoveHandler);		
		_onRecv.Add((ushort)MsgId.SSkill, MakePacket<S_Skill>);
		_handler.Add((ushort)MsgId.SSkill, PacketHandler.S_SkillHandler);		
		_onRecv.Add((ushort)MsgId.SChangeHp, MakePacket<S_ChangeHp>);
		_handler.Add((ushort)MsgId.SChangeHp, PacketHandler.S_ChangeHpHandler);		
		_onRecv.Add((ushort)MsgId.SDie, MakePacket<S_Die>);
		_handler.Add((ushort)MsgId.SDie, PacketHandler.S_DieHandler);		
		_onRecv.Add((ushort)MsgId.SConnected, MakePacket<S_Connected>);
		_handler.Add((ushort)MsgId.SConnected, PacketHandler.S_ConnectedHandler);		
		_onRecv.Add((ushort)MsgId.SLogin, MakePacket<S_Login>);
		_handler.Add((ushort)MsgId.SLogin, PacketHandler.S_LoginHandler);		
		_onRecv.Add((ushort)MsgId.SCreatePlayer, MakePacket<S_CreatePlayer>);
		_handler.Add((ushort)MsgId.SCreatePlayer, PacketHandler.S_CreatePlayerHandler);		
		_onRecv.Add((ushort)MsgId.SItemList, MakePacket<S_ItemList>);
		_handler.Add((ushort)MsgId.SItemList, PacketHandler.S_ItemListHandler);		
		_onRecv.Add((ushort)MsgId.SAddItem, MakePacket<S_AddItem>);
		_handler.Add((ushort)MsgId.SAddItem, PacketHandler.S_AddItemHandler);		
		_onRecv.Add((ushort)MsgId.SEquipItem, MakePacket<S_EquipItem>);
		_handler.Add((ushort)MsgId.SEquipItem, PacketHandler.S_EquipItemHandler);		
		_onRecv.Add((ushort)MsgId.SChangeStat, MakePacket<S_ChangeStat>);
		_handler.Add((ushort)MsgId.SChangeStat, PacketHandler.S_ChangeStatHandler);		
		_onRecv.Add((ushort)MsgId.SPing, MakePacket<S_Ping>);
		_handler.Add((ushort)MsgId.SPing, PacketHandler.S_PingHandler);		
		_onRecv.Add((ushort)MsgId.STeleport, MakePacket<S_Teleport>);
		_handler.Add((ushort)MsgId.STeleport, PacketHandler.S_TeleportHandler);		
		_onRecv.Add((ushort)MsgId.SChangeItem, MakePacket<S_ChangeItem>);
		_handler.Add((ushort)MsgId.SChangeItem, PacketHandler.S_ChangeItemHandler);		
		_onRecv.Add((ushort)MsgId.SResMarketList, MakePacket<S_ResMarketList>);
		_handler.Add((ushort)MsgId.SResMarketList, PacketHandler.S_ResMarketListHandler);		
		_onRecv.Add((ushort)MsgId.SMarketItemResister, MakePacket<S_MarketItemResister>);
		_handler.Add((ushort)MsgId.SMarketItemResister, PacketHandler.S_MarketItemResisterHandler);		
		_onRecv.Add((ushort)MsgId.SMarketItemPurchase, MakePacket<S_MarketItemPurchase>);
		_handler.Add((ushort)MsgId.SMarketItemPurchase, PacketHandler.S_MarketItemPurchaseHandler);		
		_onRecv.Add((ushort)MsgId.SMailItemList, MakePacket<S_MailItemList>);
		_handler.Add((ushort)MsgId.SMailItemList, PacketHandler.S_MailItemListHandler);		
		_onRecv.Add((ushort)MsgId.SMailItemReceive, MakePacket<S_MailItemReceive>);
		_handler.Add((ushort)MsgId.SMailItemReceive, PacketHandler.S_MailItemReceiveHandler);		
		_onRecv.Add((ushort)MsgId.SChat, MakePacket<S_Chat>);
		_handler.Add((ushort)MsgId.SChat, PacketHandler.S_ChatHandler);		
		_onRecv.Add((ushort)MsgId.SChatSpawn, MakePacket<S_ChatSpawn>);
		_handler.Add((ushort)MsgId.SChatSpawn, PacketHandler.S_ChatSpawnHandler);		
		_onRecv.Add((ushort)MsgId.SChatDespawn, MakePacket<S_ChatDespawn>);
		_handler.Add((ushort)MsgId.SChatDespawn, PacketHandler.S_ChatDespawnHandler);		
		_onRecv.Add((ushort)MsgId.SSpawnDamage, MakePacket<S_SpawnDamage>);
		_handler.Add((ushort)MsgId.SSpawnDamage, PacketHandler.S_SpawnDamageHandler);		
		_onRecv.Add((ushort)MsgId.SGoldChange, MakePacket<S_GoldChange>);
		_handler.Add((ushort)MsgId.SGoldChange, PacketHandler.S_GoldChangeHandler);		
		_onRecv.Add((ushort)MsgId.SExpChange, MakePacket<S_ExpChange>);
		_handler.Add((ushort)MsgId.SExpChange, PacketHandler.S_ExpChangeHandler);		
		_onRecv.Add((ushort)MsgId.SExitGame, MakePacket<S_ExitGame>);
		_handler.Add((ushort)MsgId.SExitGame, PacketHandler.S_ExitGameHandler);		
		_onRecv.Add((ushort)MsgId.SMarketItemDelete, MakePacket<S_MarketItemDelete>);
		_handler.Add((ushort)MsgId.SMarketItemDelete, PacketHandler.S_MarketItemDeleteHandler);		
		_onRecv.Add((ushort)MsgId.SItemSlotChange, MakePacket<S_ItemSlotChange>);
		_handler.Add((ushort)MsgId.SItemSlotChange, PacketHandler.S_ItemSlotChangeHandler);		
		_onRecv.Add((ushort)MsgId.SItemRefresh, MakePacket<S_ItemRefresh>);
		_handler.Add((ushort)MsgId.SItemRefresh, PacketHandler.S_ItemRefreshHandler);
	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);

		if (CustomHandler != null)
		{
			CustomHandler.Invoke(session, pkt, id);
		}
		else
		{
			Action<PacketSession, IMessage> action = null;
			if (_handler.TryGetValue(id, out action))
				action.Invoke(session, pkt);
		}
	}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}
}