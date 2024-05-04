using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class CreateAccountPacketReq
{
    public string AccountName;
    public string Password;
}

public class CreateAccountPacketRes
{
    public bool CreateOk;
}

public class LoginAccountPacketReq
{
    public string AccountName;
    public string Password;
}
public class ServerInfo
{
    public string Name;
    public string IpAddress;
    public int Port;
    public int BusyScore;
}

public class LoginAccountPacketRes
{
    public bool LoginOk;
    public int AccountId;
    public int Token;
    public List<ServerInfo> ServerList = new List<ServerInfo>();
}



public class MarketItemsGetListReq
{

}
public class MarketItemsGetListRes
{
    public List<MarketItem> items;
}
public class ResisterItemPacketReq
{
    public int ItemDbId;
    public int TemplateId;
    public int SellerId;
    public string ItemName;
    public int Price;
}

public class ResisterItemPacketRes
{
    public bool ItemResiterOk;
    public string SellerName;
}

public class PurchaseItemPacketReq
{
    public int ItemDbId;
    public int BuyerId;
    public int SellerId;
}

public class PurchaseItemPacketRes
{
    public bool ItemPurchaseOk;
}

public class WebPacket
{
    public static void SendCreateAccount(string account, string Password)
    {
        CreateAccountPacketReq packet = new CreateAccountPacketReq()
        {
            AccountName = account,
            Password = Password
        };

        Managers.Web.SendPostRequest<CreateAccountPacketRes>("account/create", packet, (res) =>
        {
            Debug.Log($"Create Account : {account} | {res.CreateOk}");
        });
    }

    public static void SendLoginAccount(string account, string Password)
    {
        LoginAccountPacketReq packet = new LoginAccountPacketReq()
        {
            AccountName = account,
            Password = Password
        };

        Managers.Web.SendPostRequest<LoginAccountPacketRes>("account/create", packet, (res) =>
        {
            Debug.Log($"Login Account {res.LoginOk}");
        });
    }
}

