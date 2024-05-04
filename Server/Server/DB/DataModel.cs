using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Server.DB
{
    [Table("Account")]
    public class AccountDb
    {
        public int AccountDbId { get; set; }
        public string AccountName { get; set; }
        public ICollection<PlayerDb> Players { get; set; }
    }
    [Table("Player")]
    public class PlayerDb
    {
        public int PlayerDbId { get; set; }
        public string PlayerName { get; set; }
        [ForeignKey("Account")]
        public int AccountDbId { get; set; }
        public AccountDb Account { get; set; }

        public ICollection<ItemDb> Items { get; set; } // 1 : 다 
        public ICollection<MailDb> Mails { get; set; }

        public int Level { get; set; }
        public int Hp { get; set; }
        public int MaxHp { get; set; }
        public int Attack { get; set; }
        public float Speed { get; set; }
        public int TotalExp { get; set; }
        public int Gold { get; set; }
    }

    [Table("Item")]
    public class ItemDb
    {
        public int ItemDbId { get; set; }
        public int TemplateId { get; set; }
        public int Count { get; set; }
        public int Slot { get; set; }
        public bool Equipped { get; set; } = false;

        [ForeignKey("Owner")]
        public int? OnwerDbId { get; set; }
        public PlayerDb Owner { get; set; }
    }

    [Table("Mail")]
    public class MailDb
    {

        public int MailDbId { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public int ReceicerId { get; set; }
        public int TemplateId { get; set; } // 아이템 아이디
        public int Count { get; set; } // 아이템 수량
        public bool Read { get; set; }

        [ForeignKey("Owner")]
        public int? OwnerId { get; set; }
        public PlayerDb Owner { get; set; }
    }

}
