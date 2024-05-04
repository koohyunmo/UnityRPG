using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Market : UI_ServicePopup
{

    List<MarketItem> marketItems= new List<MarketItem>();

    public override void Init()
    {
        base.Init();

        ContentItemClear();

        C_ReqMarketList c_ReqMarketList= new C_ReqMarketList();
        Managers.Network.Send(c_ReqMarketList);
        Managers.Market.ApiSyncCallback(MakeItemList);
    }

    private void MakeItemList()
    {
        ContentItemClear();
        if (Managers.Market.MarketItems.Count > 0)
        {
            foreach (var item in Managers.Market.MarketItems)
            {
                MarketItem newItem = new MarketItem()
                {
                    ItemDbId = item.ItemDbId,
                    Price = item.Price,
                    SellerId = item.SellerId,
                    SellerName = item.SellerName,
                    TemplateId = item.TemplateId
                };

                marketItems.Add(newItem);
                GameObject go = Managers.Resource.Instantiate("UI/Popup/UI_MarketItem", sharedContentGrid);
                go.GetComponent<UI_MarketItem>().SetItem(newItem, MakeItemList);

            }
        }
        else
        {
            Debug.Log($"아이템 리스트를 갖고올수없음");
        }
    }

    public override void OnClickSearch(PointerEventData data)
    {
        string s = sharedInputField.text;

        // 빈칸 확인
        if (string.IsNullOrWhiteSpace(s))
        {
            sharedInputField.text = "";
            return;
        }

        C_MarketItemSearch c_MarketItemSearch= new C_MarketItemSearch();
        c_MarketItemSearch.Name = s;

        Managers.Network.Send(c_MarketItemSearch);
    }

    public override void ContentItemClear()
    {
        base.ContentItemClear();
        marketItems.Clear();
    }
}
