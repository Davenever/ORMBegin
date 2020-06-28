using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    /// <summary>
    /// 数据库映射特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : ORMBaseAttribute
    {
        public ColumnAttribute(string name) : base(name)
        {
        }

    }
}
