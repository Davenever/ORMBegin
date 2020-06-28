using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common
{
    public static class DBAttributeExtend
    {
        /// <summary>
        /// 数据库映射
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetMappingName<T>(this T type) where T : MemberInfo
        {
            //是否有这个特性(TableAttribute)标识
            if (type.IsDefined(typeof(ORMBaseAttribute), true))
            {
                //用反射获取这个特性的实例对象
                ORMBaseAttribute attribute = (ORMBaseAttribute)type.GetCustomAttribute(typeof(ORMBaseAttribute), true);
                //调用特性中的方法
                return attribute.GetName();
            }
            else
                return type.Name;
        }

        /// <summary>
        /// 实体类中的属性的映射
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        //public static string GetMappingName(this PropertyInfo prop)
        //{
        //    //是否有这个特性(ColumnAttribute)标识
        //    if (prop.IsDefined(typeof(ColumnAttribute), true))
        //    {
        //        //用反射获取这个特性的实例对象
        //        ColumnAttribute attribute = (ColumnAttribute)prop.GetCustomAttribute(typeof(ColumnAttribute), true);
        //        //调用特性中的方法
        //        return attribute.GetName();
        //    }
        //    else
        //        return prop.Name;
        //}

        
        public static IEnumerable<PropertyInfo> GetPropertyWithoutKey(this Type type)
        {
            //将类型传进来,过滤掉属性上有KeyAttribute的字段=>主键不能插入赋值
            return type.GetProperties().Where(m => !m.IsDefined(typeof(KeyAttribute), true));
        }
    }
}
