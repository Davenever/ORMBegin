using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    /// <summary>
    /// 数据库映射特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : ORMBaseAttribute
    {
        public TableAttribute(string name) : base(name)
        {

        }
    }
}
