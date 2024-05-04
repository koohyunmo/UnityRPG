﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
	class SessionManager
	{
		static SessionManager _session = new SessionManager();
		public static SessionManager Instance { get { return _session; } }

		int _sessionId = 0;
		Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
		object _lock = new object();

        public int GetBusyScore()
        {
            int count = 0;
            lock (_lock)
            {
                count = _sessions.Count;
            }

            return count / 100;
        }

        public List<ClientSession> GetSession()
		{
			List<ClientSession> sessions = new List<ClientSession>();

			lock(_lock)
			{
                sessions = _sessions.Values.ToList(); // 정말 사소하니깐 걱정말자
            }


            return sessions;
		}

		public ClientSession Generate()
		{
			lock (_lock)
			{
				int sessionId = ++_sessionId;

				ClientSession session = new ClientSession();
				session.SessionId = sessionId;
				_sessions.Add(sessionId, session);
                Console.WriteLine("Connected " + _sessions.Count);

                return session;
			}
		}

		public ClientSession Find(int id)
		{
			lock (_lock)
			{
				ClientSession session = null;
				_sessions.TryGetValue(id, out session);
				return session;
			}
		}

		public void Remove(ClientSession session)
		{
			lock (_lock)
			{
				_sessions.Remove(session.SessionId);
                Console.WriteLine("Disconnected " + _sessions.Count);
            }
		}
	}
}
