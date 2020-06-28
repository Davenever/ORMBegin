using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Common
{
    /// <summary>
    /// sql生成+缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqlBuilder<T>
    {
        private static string FindOneSql = string.Empty;
        private static string InsertSql = string.Empty;
        static SqlBuilder()
        {
            #region 添加
            Type type = typeof(T);
            //将查询到的(数组列)每一列以逗号隔开拼成字符串  主键是不能够赋值插入的所以会过滤掉主键这个列=>type.GetPropertyWithoutKey()
            string columnString = string.Join(",", type.GetPropertyWithoutKey().Select(m => $"[{m.GetMappingName()}]"));
            //获取值,拼接为字符串
            string valueString = string.Join(",", type.GetPropertyWithoutKey().Select(m => $"@{m.GetMappingName()}"));
            InsertSql = @$"INSERT INTO [{type.GetMappingName()}] ({columnString}) Values({valueString})";
            #endregion

            #region 查询
            //将查询到的(数组列)每一列以逗号隔开拼成字符串
            string columnStrings = string.Join(",", type.GetProperties().Select(m => $"[{m.GetMappingName()}]"));
            //type.GetMappingName()=>得到特性上的参数
            FindOneSql = $@"SELECT {columnStrings} FROM [{type.GetMappingName()}] Where Id=";
            #endregion
        }

        public static string GetSql(SqlType sqlType)
        {
            switch (sqlType)
            {
                case SqlType.FindOneSql:
                    return FindOneSql;
                case SqlType.InsertSql:
                    return InsertSql;
                default:
                    throw new Exception("wrong SqlType");
            }
        }

        public enum SqlType
        {
            FindOneSql,
            InsertSql
        }
    }
}
