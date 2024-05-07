using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.Protocol;
using Newtonsoft.Json;

namespace Server.DB
{
    public static class MyAPIHandler
    {
        private static readonly HttpClient client = new HttpClient();
        public static string MarketUrl { get; set; } = "https://localhost:5061/api";

        public static async Task SendPostRequestMarket<T>(string url, object obj, Action<T> res)
        {
            await SendWebRequest(url, HttpMethod.Post, obj, res, MarketUrl);
        }
        public static async Task SendGetRequestMarket<T>(string url, string query, Action<T> res)
        {
            // 쿼리 매개변수를 URL에 추가
            string queryString = $"?SearchName={Uri.EscapeDataString(query)}";
            await SendWebRequest<T>(url + queryString, HttpMethod.Get, null, res, MarketUrl);
        }

        public static async Task SendGetRequestMarket<T>(string url, object obj, Action<T> res)
        {
            await SendWebRequest(url, HttpMethod.Get, obj, res, MarketUrl);
        }

        private static async Task SendWebRequest<T>(string url, HttpMethod method, object obj, Action<T> res, string baseUrl)
        {
            string sendUrl = $"{baseUrl}/{url}";

            var request = new HttpRequestMessage(method, sendUrl);

            if (obj != null)
            {
                string jsonStr = JsonConvert.SerializeObject(obj);
                request.Content = new StringContent(jsonStr, Encoding.UTF8, "application/json");
            }

            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                T resObj = JsonConvert.DeserializeObject<T>(responseBody);
                res.Invoke(resObj);
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
    }

    public class ResisterItemPacketReq
    {
        public int ItemDbId;
        public int TemplateId;
        public int SellerId;
        public int Price;
        public string ItemName;
    }

    public class ResisterItemPacketRes
    {
        public bool ItemResiterOk;
    }

    public class MarketItemsGetListReq
    {

    }
    public class MarketItemsGetListRes
    {
        public List<MarketItem> items;
    }

    public class MarketItemsGetSearchListReq
    {
        public string SearchName;
    }
    public class PurchaseItemPacketReq
    {
        public int ItemDbId;
        public int TemplateId;
        public int BuyerId;
        public int SellerId;
        public int Price;
    }

    public class PurchaseItemPacketRes
    {
        public bool ItemPurchaseOk;
    }

    public class DeleteItemPacketReq
    {
        public int BuyerId;
        public int TemplateId;
        public int ItemId;
        public int SellerId;
    }

    public class DeleteItemPacketRes
    {
        public bool DeleteOk;
    }
}
