using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MarketServer.DB
{
    // 마켓 아이템 테이블
    [Table("TransactionItem")]
    public class TransactionDb
    {
        public int TransactionDbId { get; set; }
        public int Price { get; set; }
        [Required]
        public string BuyerId { get; set; }
        [Required]
        public string SellerId { get; set; }
    }
    [Table("Market")]
    public class MarketDb
    {

        public int MarketDbId { get; set; } // 게임 DB의 아이템 ID 참조
        [Required]
        public int ItemDbId { get; set; }

        [Required]
        public int SellerId { get; set; } // 판매자의 PlayerDb ID

        public string SellerName { get; set; }

        [Required]
        [StringLength(100)] // 이름 길이 제한
        public string ItemName { get; set; } // 아이템 이름

        [Required]
        public int Price { get; set; } // 즉시 판매 가격
        [Required]
        public int TemplateId { get; set; } // 아이템 템플레이트 아이디

        public DateTime ListedTime { get; set; } // 등록 시간

        public string? BuyerId { get; set; } // 구매자의 PlayerDb ID (구매 완료 시 기록)

        public bool IsSold { get; set; } = false; // 판매 완료 여부
    }
}
