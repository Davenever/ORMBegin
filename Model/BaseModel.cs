using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class BaseModel
    {
        [Key]
        public Guid Id { get; set; }
    }
}
