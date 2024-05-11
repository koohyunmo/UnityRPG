using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class MarketManager 
{
    public List<MarketItem> MarketItems {get; private set;} = new List<MarketItem>();
    private Action marketListUpdateCallback;

    public void APISyncCallback(Action ac)
    {
        marketListUpdateCallback = null;
        marketListUpdateCallback = ac;
    }

    public void RefreshItems(List<MarketItem> marketItems)
    {
        MarketItems.Clear();

        if(marketItems != null)
        {
            foreach (MarketItem item in marketItems)
            {

                MarketItems.Add(new MarketItem()
                {
                    SellerName = item.SellerName,
                    SellerId = item.SellerId,
                    ItemDbId = item.ItemDbId,
                    Price = item.Price,
                    TemplateId = item.TemplateId
                });
            }
        }
        marketListUpdateCallback?.Invoke();
    }
}
