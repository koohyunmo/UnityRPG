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
		_onRecv.Add((ushort)MsgId.CMove, MakePacket<C_Move>);
		_handler.Add((ushort)MsgId.CMove, PacketHandler.C_MoveHandler);		
		_onRecv.Add((ushort)MsgId.CSkill, MakePacket<C_Skill>);
		_handler.Add((ushort)MsgId.CSkill, PacketHandler.C_SkillHandler);		
		_onRecv.Add((ushort)MsgId.CLogin, MakePacket<C_Login>);
		_handler.Add((ushort)MsgId.CLogin, PacketHandler.C_LoginHandler);		
		_onRecv.Add((ushort)MsgId.CEnterGame, MakePacket<C_EnterGame>);
		_handler.Add((ushort)MsgId.CEnterGame, PacketHandler.C_EnterGameHandler);		
		_onRecv.Add((ushort)MsgId.CCreatePlayer, MakePacket<C_CreatePlayer>);
		_handler.Add((ushort)MsgId.CCreatePlayer, PacketHandler.C_CreatePlayerHandler);		
		_onRecv.Add((ushort)MsgId.CEquipItem, MakePacket<C_EquipItem>);
		_handler.Add((ushort)MsgId.CEquipItem, PacketHandler.C_EquipItemHandler);		
		_onRecv.Add((ushort)MsgId.CPong, MakePacket<C_Pong>);
		_handler.Add((ushort)MsgId.CPong, PacketHandler.C_PongHandler);		
		_onRecv.Add((ushort)MsgId.CTeleport, MakePacket<C_Teleport>);
		_handler.Add((ushort)MsgId.CTeleport, PacketHandler.C_TeleportHandler);		
		_onRecv.Add((ushort)MsgId.CChangeItem, MakePacket<C_ChangeItem>);
		_handler.Add((ushort)MsgId.CChangeItem, PacketHandler.C_ChangeItemHandler);		
		_onRecv.Add((ushort)MsgId.CReqMarketList, MakePacket<C_ReqMarketList>);
		_handler.Add((ushort)MsgId.CReqMarketList, PacketHandler.C_ReqMarketListHandler);		
		_onRecv.Add((ushort)MsgId.CMarketItemResister, MakePacket<C_MarketItemResister>);
		_handler.Add((ushort)MsgId.CMarketItemResister, PacketHandler.C_MarketItemResisterHandler);		
		_onRecv.Add((ushort)MsgId.CMarketItemPurchase, MakePacket<C_MarketItemPurchase>);
		_handler.Add((ushort)MsgId.CMarketItemPurchase, PacketHandler.C_MarketItemPurchaseHandler);		
		_onRecv.Add((ushort)MsgId.CMailItemList, MakePacket<C_MailItemList>);
		_handler.Add((ushort)MsgId.CMailItemList, PacketHandler.C_MailItemListHandler);		
		_onRecv.Add((ushort)MsgId.CMailItemReceive, MakePacket<C_MailItemReceive>);
		_handler.Add((ushort)MsgId.CMailItemReceive, PacketHandler.C_MailItemReceiveHandler);		
		_onRecv.Add((ushort)MsgId.CMarketItemSearch, MakePacket<C_MarketItemSearch>);
		_handler.Add((ushort)MsgId.CMarketItemSearch, PacketHandler.C_MarketItemSearchHandler);		
		_onRecv.Add((ushort)MsgId.CChat, MakePacket<C_Chat>);
		_handler.Add((ushort)MsgId.CChat, PacketHandler.C_ChatHandler);
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