using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class ShopType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Remarks { get; set; }
        public DateTime Date { get; set; }
    }
}
