using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SelectServerPopup : UI_Popup
{

    public List<UI_SelectServerItem> ServerItems {get;} = new List<UI_SelectServerItem>();

    public override void Init()
    {
       base.Init();
    }

    public void SetServer(List<ServerInfo> servers)
    {
        ServerItems.Clear();

        GameObject grid = GetComponentInChildren<GridLayoutGroup>().gameObject;
        foreach (Transform child in grid.transform)
            Destroy(child.gameObject);

        foreach(var server in servers)
        {
            GameObject go = Managers.Resource.Instantiate("UI/Popup/UI_SelectServerItem", grid.transform);
            UI_SelectServerItem item = go.GetOrAddComponent<UI_SelectServerItem>();
            ServerItems.Add(item);
            item.Info = server;
        }

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (ServerItems.Count == 0)
            return;


        foreach (var item in ServerItems)
        {
            item.RefreshUI();
        }
    }

}
