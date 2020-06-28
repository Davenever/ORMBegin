using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    [Table("Shops")]
    public class Shops
    {
        public Guid Id { get; set; }
        [Column("Name")]
        public string ShopName { get; set; }
        public string Remarks { get; set; }
        public DateTime Date { get; set; }
    }
}
