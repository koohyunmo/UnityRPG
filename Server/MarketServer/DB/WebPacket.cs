using Server.Game;
using System;

public class ResisterItemPacketReq
{
    public int ItemDbId { get; set; }
    public int TemplateId { get; set; }
    public int SellerId { get; set; }
    public int Price { get; set; }
    public string ItemName { get; set; }
}

public class ResisterItemPacketRes
{
    public bool ItemResiterOk { get; set;}
}

public class MarketItemsGetListReq
{

}
public class MarketItemsGetListRes
{
    public List<MarketItem> items { get; set; }
}
public class MarketItemsGetSearchListReq
{
    public string SearchName { get; set; }
}

public class PurchaseItemPacketReq
{
    public int ItemDbId { get; set; }
    public int BuyerId { get; set; }
    public int SellerId { get; set; }
    public int Price { get; set; }
}

public class PurchaseItemPacketRes
{
   public bool ItemPurchaseOk { get; set;}
}

public struct MarketItem
{
    public int ItemDbId { get; set; }
    public int TemplateId { get; set; }
    public int SellerId { get; set; }
    public string ItemName { get; set; }
    public int Price { get; set; }
    public string SellerName { get; set;}
}

public class DeleteItemPacketReq
{
    public int BuyerId { get; set; }
    public int TemplateId { get; set; }
    public int ItemId { get; set; }
    public int SellerId { get; set; }
}

public class DeleteItemPacketRes
{
    public bool DeleteOk { get; set; }
}

