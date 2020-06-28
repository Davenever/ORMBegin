using Common;
using Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class SqlHelper
    {

        public async Task<Products> FindProducts(Guid id)
        {
            string sql = $@"SELECT [Id]
                           ,[ProductName]
                           ,[ProductPrice]
                           ,[Period]
                           ,[CreateDate]
                       FROM [CoreFrame].[dbo].[Products]
                       Where Id='{id}'";
            using (SqlConnection conn = new SqlConnection(config.SqlConnStr))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                conn.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Products products = new Products()
                    {
                        Id = (Guid)reader["Id"],
                        ProductName = reader["ProductName"].ToString(),
                        ProductPrice = (float)reader["ProductPrice"],
                        Period = reader["Period"].ToString(),
                        CreateDate = (DateTime)reader["CreateDate"]
                    };
                    return products;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 数据查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> Find<T>(Guid id)
        {
            //不同的T代表不同的sql--反射拼装sql
            Type type = typeof(T);
            //将查询到的(数组列)每一列以逗号隔开拼成字符串
            //string columnString = string.Join(",", type.GetProperties().Select(m => $"[{m.GetMappingName()}]"));
            //type.GetMappingName()=>得到特性上的参数
            //string sql = $@"SELECT {columnString} FROM [{type.GetMappingName()}] Where Id='{id}'";

            string sql = $"{SqlBuilder<T>.GetSql(SqlBuilder<T>.SqlType.FindOneSql)}'{id}'";
            using (SqlConnection conn = new SqlConnection(config.SqlConnStr))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                conn.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    //创建对象
                    T t = (T)Activator.CreateInstance(type);
                    foreach (var item in type.GetProperties())
                    {
                        //给实体(t)的这个属性(item)设置为这个值reader[item.Name]
                        //为nul就给null值,不为null就给查到的值
                        item.SetValue(t, reader[item.GetMappingName()] is DBNull ? null : reader[item.GetMappingName()]);
                    }
                    return (T)t;
                }
                else
                {
                    return default(T);
                }
            }
        }

        /// <summary>
        /// 数据插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<bool> Insert<T>(T t)
        {

            Type type = typeof(T);
            //将查询到的(数组列)每一列以逗号隔开拼成字符串  主键是不能够赋值插入的所以会过滤掉主键这个列=>type.GetPropertyWithoutKey()
            //string columnString = string.Join(",", type.GetPropertyWithoutKey().Select(m => $"[{m.GetMappingName()}]"));
            //获取值,拼接为字符串
            //string valueString = string.Join(",", type.GetPropertyWithoutKey().Select(m => $"@{m.GetMappingName()}"));
            //string sql = @$"INSERT INTO [{type.GetMappingName()}] ({columnString}) Values({valueString})";

            string sql = SqlBuilder<T>.GetSql(SqlBuilder<T>.SqlType.InsertSql);
            //转成参数列表  属性名称--值  Select=>Foreach
            IEnumerable<SqlParameter> parameters = type.GetPropertyWithoutKey().Select(m => new SqlParameter($"@{m.Name}", m.GetValue(t) ?? DBNull.Value));

            using (SqlConnection conn = new SqlConnection(config.SqlConnStr))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddRange(parameters.ToArray());
                conn.Open();
                int result = command.ExecuteNonQuery();
                return result == 1;
            }
        }
    }
}
