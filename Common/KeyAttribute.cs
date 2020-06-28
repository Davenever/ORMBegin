using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : Attribute
    {
        public KeyAttribute()
        {

        }
    }
}
