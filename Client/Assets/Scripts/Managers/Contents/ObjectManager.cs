using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class ObjectManager
{
    public MyPlayerController MyPlayer {get; set;}
    Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();
    
    public static GameObjectType GetObjectTypeById(int id)
    {
        int type = (id >> 24 ) & 0x7F;
        return (GameObjectType)type;
    }

    public void Add(ObjectInfo info, bool isMyPlayer = false)
    {
        if(MyPlayer != null && info.ObjectId == MyPlayer.Id)
        {
            return;
        }
        if(_objects.ContainsKey(info.ObjectId))
        {
            return;
        }
        GameObjectType objectType = GetObjectTypeById(info.ObjectId);
        if(objectType == GameObjectType.Player)
        {
            if (isMyPlayer)
            {
                GameObject go = Managers.Resource.Instantiate("Creature/MyPlayer");
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);

                MyPlayer = go.GetComponent<MyPlayerController>();
                MyPlayer.Id = info.ObjectId;
                MyPlayer.PosInfo = info.PosInfo;
                //MyPlayer.Stat = info.StatInfo;
                MyPlayer.Stat.MergeFrom(info.StatInfo);
                MyPlayer.SyncPos();
                MyPlayer.SetName(info.Name);
                Managers.Notify.ChangeUserInfo();
            }
            else
            {
                GameObject go = Managers.Resource.Instantiate("Creature/Player");
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);

                PlayerController pc = go.GetComponent<PlayerController>();
                pc.Id = info.ObjectId;
                pc.PosInfo = info.PosInfo;
                pc.Stat.MergeFrom(info.StatInfo);
                pc.SyncPos();
                pc.SetName(info.Name);
            }
        }
        else if(objectType == GameObjectType.Monster)
        {
            GameObject go = Managers.Resource.Instantiate("Creature/Monster");
            go.name = info.Name;
            _objects.Add(info.ObjectId, go);

            MonsterController mc = go.GetComponent<MonsterController>();
            mc.Id = info.ObjectId;
            mc.PosInfo = info.PosInfo;
            mc.Stat.MergeFrom(info.StatInfo);
            mc.SyncPos();
        }
        else if (objectType == GameObjectType.BossMonster)
        {
            //Debug.Log("몬스터 생성");

            GameObject go = Managers.Resource.Instantiate("Creature/BossMonster");
            go.name = info.Name;
            _objects.Add(info.ObjectId, go);

            MonsterController mc = go.GetComponent<MonsterController>();
            mc.Id = info.ObjectId;
            mc.PosInfo = info.PosInfo;
            mc.Stat.MergeFrom(info.StatInfo);
            mc.SyncPos();

            Debug.Break();
        }
        else if(objectType == GameObjectType.Projectile)
        {
            GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
            go.name = "Arrow";
            _objects.Add(info.ObjectId,go);

            ArrowController ac = go.GetComponent<ArrowController>();
            ac.PosInfo = info.PosInfo;
            ac.Stat.MergeFrom(info.StatInfo);
            ac.SyncPos();
        }
        else
        {
            return;
        }

    }
    public void Remove(int id)
    {
        if (MyPlayer != null && id== MyPlayer.Id)
        {
            return;
        }

        if (_objects.ContainsKey(id) == false)
        {
            return;
        }

        var go = FindById(id);
        _objects.Remove(id);
        Managers.Resource.Destroy(go);
    }
    
    public GameObject FindById(int id)
    {
        GameObject go = null;
        _objects.TryGetValue(id, out go);
        return go;
    }
    public GameObject FindCreature(Vector3Int cellPos)
    {
        foreach(GameObject obj in _objects.Values)
        {
            if(obj == null)
                continue;
            CreatureController cc= obj.GetComponent<CreatureController>();
            if(cc == null)
                return null;
            
            if(cc.CellPos == cellPos)
                return obj;
        }

        return null;
    }

    public GameObject Find(Func<GameObject,bool> condition)
    {
        foreach (GameObject obj in _objects.Values)
        {
            if(condition.Invoke(obj))
                return obj;
        }

        return null;
    }

    public void Clear()
    {
        foreach (GameObject obj in _objects.Values)
        {
            Managers.Resource.Destroy(obj);
        }
        _objects.Clear();
        MyPlayer = null;
    }
}
