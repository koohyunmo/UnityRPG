using MarketServer.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.Game;
using Server.Migrations;
using Server.Object;
using SharedDB;
using System.Diagnostics;
using System.Transactions;

namespace MarketServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketController : ControllerBase
    {
        MarketAppDbContext _context;
        Server.DB.AppDbContext _gameDbContext;

        public MarketController(MarketAppDbContext context, Server.DB.AppDbContext gameDbContext)
        {
            _context = context;
            _gameDbContext = gameDbContext;
        }

        [HttpPost]
        [Route("register")]
        public ResisterItemPacketRes RegisterMarketItem([FromBody] ResisterItemPacketReq req)
        {
            ResisterItemPacketRes res = new ResisterItemPacketRes();

            using var transaction1 = _gameDbContext.Database.BeginTransaction();
            using var transaction2 = _context.Database.BeginTransaction();

            bool sucess1 = false;
            bool sucess2 = false;

            try
            {

                var playerItem = _gameDbContext.Items
                    .Where(i => i.ItemDbId == req.ItemDbId && i.Equipped == false)
                    .FirstOrDefault();

                if (playerItem == null)
                {
                    throw new InvalidOperationException("No unequipped item found with the provided ID.");
                }

                _gameDbContext.Items.Remove(playerItem);
                sucess1 = _gameDbContext.SaveChangesEx();

                var marketItem = _context.MarketItems
                    .Where(i => i.ItemDbId == req.ItemDbId && i.Price == req.Price && i.TemplateId == req.TemplateId)
                    .FirstOrDefault();

                if (marketItem != null)
                {
                    throw new InvalidOperationException("Item already registered in the market.");
                }

                var seller = _gameDbContext.Players
                    .Where(p => p.PlayerDbId == (req.SellerId))
                    .FirstOrDefault();

                if(seller == null)
                {
                    throw new InvalidOperationException($"player_{req.SellerId} is Null ");
                }

                _context.MarketItems.Add(new MarketDb
                {
                    ItemDbId = req.ItemDbId,
                    Price = req.Price,
                    SellerId = req.SellerId,
                    TemplateId = req.TemplateId,
                    SellerName = seller.PlayerName,
                    ItemName = req.ItemName,
                });
                sucess2 = _context.SaveChangesEx();

                if (sucess1 && sucess2)
                {
                    transaction1.Commit();
                    transaction2.Commit();
                    res.ItemResiterOk = true;
                }
            }
            catch (Exception ex)
            {
                transaction1.Rollback();
                transaction2.Rollback();
                Console.WriteLine(ex);
                res.ItemResiterOk = false;
                return res;
            }


            if (sucess1 == false || sucess2 == false)
            {
                transaction1.Rollback();
                transaction2.Rollback();
                res.ItemResiterOk = false;
            }

            return res;

        }


        // 05-03 구매까지
        [HttpPost]
        [Route("purchase")]
        public PurchaseItemPacketRes PurchaseMarketItem([FromBody] PurchaseItemPacketReq req)
        {
            PurchaseItemPacketRes res = new PurchaseItemPacketRes();

            using var transaction1 = _gameDbContext.Database.BeginTransaction();
            using var transaction2 = _context.Database.BeginTransaction();

            bool sucess1 = false;
            bool sucess2 = false;
            bool sucess3 = false;

            try
            {
                if(req.SellerId == req.BuyerId)
                {
                    res.ItemPurchaseOk = false;
                    return res;
                }
                var buyer = _gameDbContext.Players
                    .Where(p => p.PlayerDbId == (req.BuyerId))
                    .FirstOrDefault();

                if (buyer == null)
                {
                    Console.WriteLine($" {req.BuyerId} Buyer is Null");
                    res.ItemPurchaseOk = false;
                    return res;
                }

                var marketItem = _context.MarketItems
                    .Where(i => i.ItemDbId == req.ItemDbId && i.SellerId == req.SellerId && i.Price == req.Price && i.IsSold == false)
                    .FirstOrDefault();

                if (marketItem == null)
                {
                    Console.WriteLine($"marketItem is Null {req.SellerId}");
                    res.ItemPurchaseOk = false;
                    return res;
                }

                var seller = _gameDbContext.Players
                    .Where(p => p.PlayerDbId == req.SellerId)
                    .FirstOrDefault();

                if (seller == null)
                {
                    Console.WriteLine("seller is Null");
                    res.ItemPurchaseOk = false;
                    return res;
                }

                if (buyer.Gold < marketItem.Price)
                {
                    res.ItemPurchaseOk = false;
                     Console.WriteLine("price check");
                     return res;
                }

                // Deduct gold from the buyer's account
                var buyerMail = _gameDbContext.Mails;
                buyer.Gold = buyer.Gold  -  marketItem.Price;
                sucess1 = _gameDbContext.SaveChangesEx();
                buyerMail.Add(new Server.DB.MailDb()
                {
                    OwnerId = buyer.PlayerDbId,
                    SenderName = seller.PlayerName,
                    SenderId = seller.PlayerDbId,
                    ReceicerId = buyer.PlayerDbId,
                    Count = 1,
                    Read = false,
                    Owner = buyer,
                    TemplateId = marketItem.TemplateId
                });
               sucess1 = _gameDbContext.SaveChangesEx() && sucess1;

               seller.Gold = seller.Gold  + marketItem.Price;
               sucess2 = _gameDbContext.SaveChangesEx();

                // Assuming the item is being removed from the market or some status is updated
                marketItem.IsSold = true;  // This is an example property
                sucess3 = _context.SaveChangesEx();

                if (sucess1 && sucess2 && sucess3)
                {
                    transaction1.Commit();
                    transaction2.Commit();
                    res.ItemPurchaseOk = true;
                    Console.WriteLine($" {seller.PlayerName} => {marketItem.TemplateId} => {buyer.PlayerName}");
                }
            }
            catch (Exception ex)
            {
                transaction1.Rollback();
                transaction2.Rollback();
                Console.WriteLine(ex);
                res.ItemPurchaseOk = false;
                return res;
            }


            if (sucess1 == false || sucess2 == false || sucess3 == false)
            {
                transaction1.Rollback();
                transaction2.Rollback();
                res.ItemPurchaseOk = false;
            }

            transaction1.Dispose();
            transaction2.Dispose();

            return res;

        }

        [HttpPost]
        [Route("delete")]
        public DeleteItemPacketRes DeleteMarketItem([FromBody] DeleteItemPacketReq req)
        {
            DeleteItemPacketRes res = new DeleteItemPacketRes();

            using var transaction1 = _gameDbContext.Database.BeginTransaction();
            using var transaction2 = _context.Database.BeginTransaction();

            bool sucess1 = false;
            bool sucess2 = false;

            try
            {

                if (req.SellerId != req.BuyerId)
                {
                    res.DeleteOk = false;
                    return res;
                }

                var buyer = _gameDbContext.Players
                    .Where(p => p.PlayerDbId == (req.BuyerId))
                    .FirstOrDefault();

                if (buyer == null)
                {
                    Console.WriteLine("Buyer is Null");
                    res.DeleteOk = false;
                    return res;
                }

                var marketItem = _context.MarketItems
                    .Where(i => i.ItemDbId == req.ItemId && i.SellerId == req.SellerId)
                    .FirstOrDefault();

                if (marketItem == null)
                {
                    Console.WriteLine($"marketItem is Null {req.SellerId}");
                    res.DeleteOk = false;
                    return res;
                }

                _context.MarketItems.Remove(marketItem);
                sucess1 = _context.SaveChangesEx();


                var buyerMail = _gameDbContext.Mails;
                buyerMail.Add(new Server.DB.MailDb()
                {
                    OwnerId = buyer.PlayerDbId,
                    SenderName = buyer.PlayerName,
                    SenderId = buyer.PlayerDbId,
                    ReceicerId = buyer.PlayerDbId,
                    Count = 1,
                    Read = false,
                    Owner = buyer,
                    TemplateId = marketItem.TemplateId
                });
                sucess2 = _gameDbContext.SaveChangesEx();


                if (sucess1 && sucess2)
                {
                    transaction1.Commit();
                    transaction2.Commit();
                    res.DeleteOk = true;
                }
            }
            catch (Exception ex)
            {
                transaction1.Rollback();
                transaction2.Rollback();
                Console.WriteLine(ex);
                res.DeleteOk = false;
                return res;
            }


            if (sucess1 == false || sucess2 == false)
            {
                transaction1.Rollback();
                transaction2.Rollback();
                res.DeleteOk = false;
            }

            transaction1.Dispose();
            transaction2.Dispose();

            return res;
        }


        [HttpGet]
        [Route("list")]
        public MarketItemsGetListRes ListItems([FromQuery] MarketItemsGetListReq req)
        {
            MarketItemsGetListRes res = new MarketItemsGetListRes();
            res.items = null;
            try
            {
                var items = _context.MarketItems
                    .AsNoTracking()
                    .Where(i => i.IsSold == false)
                    .Select(item => new MarketItem
                    {
                        ItemDbId = item.ItemDbId,
                        ItemName = item.ItemName,
                        Price = item.Price,
                        SellerId = item.SellerId,
                        TemplateId = item.TemplateId,
                        SellerName = item.SellerName
                    })
                    .ToList();

                if (items.Count > 0)
                    res.items = items;

                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return res;
            }
        }

        [HttpGet]
        [Route("search")]
        public MarketItemsGetListRes SearchListItems([FromQuery] string searchName)
        {
            MarketItemsGetListRes res = new MarketItemsGetListRes();
            res.items = null;
            try
            {
                var items = _context.MarketItems
                    .AsNoTracking()
                    .Where(i => i.IsSold == false && i.ItemName.Contains(searchName))
                    .Select(item => new MarketItem
                    {
                        ItemDbId = item.ItemDbId,
                        ItemName = item.ItemName,
                        Price = item.Price,
                        SellerId = item.SellerId,
                        TemplateId = item.TemplateId,
                        SellerName = item.SellerName
                    })
                    .ToList();

                if (items.Count > 0)
                    res.items = items;

                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return res;
            }
        }

    }
}
