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
        private static string UpdateSql = string.Empty;
        private static string DeleteSql = string.Empty;
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
            FindOneSql = $@"SELECT {columnStrings} FROM [{type.GetMappingName()}] Where Id=@id";
            #endregion

            #region 修改
            //type.GetPropertyWithoutKey()  过滤掉主键,主键不能更新,不然会报错
            //m.GetMappingName() 映射--解决数据库中名称与程序中名称不一致
            string updateStr = string.Join(",", type.GetPropertyWithoutKey().Select(m => $"{m.GetMappingName()}=@{m.GetMappingName()}"));
            UpdateSql = @$"update [{type.GetMappingName()}] set {updateStr} where id=";
            #endregion

            #region 删除
            DeleteSql = $"delete from {type.GetMappingName()} where id=@id";
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
                case SqlType.UpdateSql:
                    return UpdateSql;
                case SqlType.DeleteSql:
                    return DeleteSql;
                default:
                    throw new Exception("wrong SqlType");
            }
        }

        public enum SqlType
        {
            FindOneSql,
            InsertSql,
            UpdateSql,
            DeleteSql
        }
    }
}
