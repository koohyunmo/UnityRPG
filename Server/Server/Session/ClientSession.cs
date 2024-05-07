using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using Server.Game;
using Server.Object;
using Server.Data;

namespace Server
{
	public partial class ClientSession : PacketSession
	{
		public Player MyPlayer { get; set; }
		public int SessionId { get; set; }

		List<ArraySegment<byte>> _reserveQueue = new List<ArraySegment<byte>>();
		object _lock = new object();
		public PlayerServerState ServerState { get; private set; } = PlayerServerState.ServerStateLogin;

		// 패킷 모아 보내기
		int _reservedSendBytes = 0;
		long _lastSendTick = 0;

		long _pingpongTick = 0;
		public void Ping()
		{
			if(_pingpongTick > 0)
			{
				long delta = (System.Environment.TickCount64 - _pingpongTick);
				if(delta > 30 * 1000)
				{
                    Console.WriteLine(	$"Disconnected by ping");
                    Disconnect();
					return;
				}
			}

			S_Ping pingPacket = new S_Ping();
			Send(pingPacket);

			GameLogic.Instance.PushAfter(5000, Ping);

		}
		public void HandlePong()
		{
			_pingpongTick = System.Environment.TickCount64;
		}

		#region Network
		/// <summary>
		/// 패킷 전송 예약만 하고 보내지는 않는다.
		/// </summary>
		/// <param name="packet"></param>
		public void Send(IMessage packet)
		{
			string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
			MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName); //SChangeHp 소문자가 들어옴?

			ushort size = (ushort)packet.CalculateSize();
			byte[] sendBuffer = new byte[size + 4]; // 2바이트 사이즈 2바이트 포로토콜
			Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort)); // 0~2byte Size
			Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort)); //2~3byte ProtocolId
			Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);

			lock (_lock)
			{
				_reserveQueue.Add(sendBuffer);
				_reservedSendBytes += sendBuffer.Length;
			}

			//Send(new ArraySegment<byte>(sendBuffer));
		}

		// 실제 Network IO 보내는 부분
		// 패킷 모아 보내기
		public void FlushSend()
		{
			List<ArraySegment<byte>> sendList = null;
			lock (_lock)
			{
				// 0.1초가 지났거나, 1024바이트 패킷이 쌓였을때
				long delta = (System.Environment.TickCount64 - _lastSendTick);
				if (delta < 100 && _reservedSendBytes < 1024)
				//	return;
				if (_reserveQueue.Count == 0)
					return;

				//// 패킷 모아 보내기
				_reservedSendBytes = 0;
				_lastSendTick = System.Environment.TickCount64;

				sendList = _reserveQueue;
				_reserveQueue = new List<ArraySegment<byte>>();
			}

			Send(sendList);
		}
		/// <summary>
		/// 클라이언트 연결 처리
		/// </summary>
		/// <param name="endPoint"></param>
		public override void OnConnected(EndPoint endPoint)
		{
			//Console.WriteLine($"OnConnected : {endPoint}");

			{
				S_Connected connectedPacket = new S_Connected();
				Send(connectedPacket);
			}


            S_Ping pingPacket = new S_Ping();
            Send(pingPacket);
            GameLogic.Instance.PushAfter(5000, Ping);

        }

		/// <summary>
		/// 클라이언트 패킷 받는곳
		/// </summary>
		/// <param name="buffer"></param>
		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
            //Console.WriteLine($"Packet Recevied {buffer.Count} byte");
            PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		public override void OnDisconnected(EndPoint endPoint)
		{

            GameLogic.Instance.Push(() =>
            {
				if (MyPlayer == null) return;

                GameRoom room = GameLogic.Instance.Find(1);
                room.Push(room.LeaveGame, MyPlayer.Info.ObjectId);
            });


            SessionManager.Instance.Remove(this);

			Console.WriteLine($"OnDisconnected : {endPoint}");
		}

		public override void OnSend(int numOfBytes)
		{
			//Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}
        #endregion
    }
}
