using System;
using System.Collections;
using System.Collections.Concurrent;

namespace Authorization.Managers
{
    class SessionManager
    {
        public readonly ConcurrentDictionary<uint, Entities.Session> Sessions;

        public SessionManager() {
            Sessions = new ConcurrentDictionary<uint, Entities.Session>();
        }

        public void Add(Entities.User u) {
            uint sessionId = 0;

            do {
                sessionId++;
            } while (Sessions.ContainsKey(sessionId));

            u.SetSession(sessionId);
            Sessions.TryAdd(sessionId, new Entities.Session(sessionId, u.ID, u.Name, u.Displayname, u.AccessLevel));
        }

        public Entities.Session Get(uint sessionId) {
            Entities.Session session = null;
            if (Sessions.ContainsKey(sessionId)) {
                Sessions.TryGetValue(sessionId, out session);
            }
            return session;
        }

        public void Remove(uint sessionId) {
            Entities.Session session = null;
            if (Sessions.ContainsKey(sessionId)) {
                Sessions.TryRemove(sessionId, out session);
            }
        }

        private static SessionManager instance;
        public static SessionManager Instance { get { if (instance == null) { instance = new SessionManager(); } return instance; } }
    }
}
