using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Object
{
    public class ObjectManager
    {
        public static ObjectManager Instance { get; } = new ObjectManager();

        object _lock = new object();
        Dictionary<int, Player> _players = new Dictionary<int, Player>();
        //[UNUSED(1bit)]{TYPE(7bit)}[ID(24 bit)]
        int _counter = 0;

        public T Add<T>() where T : GameObject, new()
        {
            T gameObejct = new T();
            lock(_lock)
            {
                gameObejct.Id =  GenerateId(gameObejct.ObjectType);
                if(gameObejct.ObjectType == GameObjectType.Player)
                {
                    _players.Add(gameObejct.Id, gameObejct as Player);
                }

            }
            return gameObejct;
        }

        int GenerateId(GameObjectType type)
        {
            lock (_lock)
            {
                int result = 0;
                result = ((int)type << 24) | (_counter++);
                Console.WriteLine($"Object 생성 ID : {result}");
                return result;
            }
        }

        public static GameObjectType GetObjectTypeById(int id)
        {
            int type = (id>>24) & 0x7F; // 0x7F 8bit 다킨상태
            return (GameObjectType)type;
        }

        public bool Remove(int objectId)
        {
            GameObjectType objectType = GetObjectTypeById(objectId);
            lock (_lock)
            {
                if(objectType == GameObjectType.Player)
                    return _players.Remove(objectId);
            }
            return false;
        }
        public Player Find(int objectId)
        {
            GameObjectType objectType = GetObjectTypeById(objectId);
            lock (_lock)
            {
                if(objectType == GameObjectType.Player)
                {
                    Player player = null;
                    if (_players.TryGetValue(objectId, out player))
                        return player;
                }
                return null;
            }
        }
    }
}
