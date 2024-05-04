﻿using Google.Protobuf.Protocol;
using Server.Object;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class Zone
    {
        public int IndexY { get; private set; }
        public int IndexX { get; private set; }

        public HashSet<Player> Players { get; private set; } = new HashSet<Player>();
        public HashSet<Monster> Monsters = new HashSet<Monster>();
        public HashSet<Projectile> Projectiles = new HashSet<Projectile>();

        public Zone(int y, int x)
        {
            IndexY = y;
            IndexX = x;
        }

        public Player FindOne(Func<Player, bool> condition)
        {
            foreach (Player player in Players)
            {
                if (condition.Invoke(player))
                    return player;
            }

            return null;
        }

        public List<Player> FindAll(Func<Player, bool> condition)
        {
            List<Player> findList = new List<Player>();
            foreach (Player player in Players)
            {
                if (condition.Invoke(player))
                    findList.Add(player);
            }

            return findList;
        }

        public void Remove(GameObject gameObject)
        {
            GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);

            switch (type)
            {
                case GameObjectType.Player:
                    Players.Remove((Player)gameObject);
                    break;
                case GameObjectType.Monster:
                    Monsters.Remove((Monster)gameObject);
                    break;
                case GameObjectType.Projectile:
                    Projectiles.Remove((Projectile)gameObject);
                    break;
            }

        }


    }
}