using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    /// <summary>
    /// 数据库映射的特性基类
    /// </summary>
    public class ORMBaseAttribute : Attribute
    {
        private string _Name = string.Empty;
        public ORMBaseAttribute(string name)
        {
            this._Name = name;
        }

        public virtual string GetName()
        {
            return _Name;
        }
    }
}
