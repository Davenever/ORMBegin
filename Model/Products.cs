﻿using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    [TableAttribute("Products")]
    public class Products: BaseModel
    {
        public string ProductName { get; set; }
        public float ProductPrice { get; set; }
        public string Period { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
