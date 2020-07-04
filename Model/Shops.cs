using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    [Table("Shops")]
    public class Shops: BaseModel
    {
        [Column("Name")]
        public string ShopName { get; set; }
        public string Remarks { get; set; }
        public DateTime Date { get; set; }
    }
}
