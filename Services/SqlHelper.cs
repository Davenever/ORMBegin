using Common;
using Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
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

            //string sql = $"{SqlBuilder<T>.GetSql(SqlBuilder<T>.SqlType.FindOneSql)}'{id}'";
            string sql = SqlBuilder<T>.GetSql(SqlBuilder<T>.SqlType.FindOneSql);
            IEnumerable<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter("@id",id)
            };
            T tResult = this.ExecuteSql<T>(sql, parameters, (command) =>
            {
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
                    return t;
                }
                else
                {
                    throw new Exception("主键查询,没有结果!");
                }
            });
            return tResult;
            //using (SqlConnection conn = new SqlConnection(config.SqlConnStr))
            //{
            //    SqlCommand command = new SqlCommand(sql, conn);
            //    conn.Open();
            //    var reader = command.ExecuteReader();
            //    if (reader.Read())
            //    {
            //        //创建对象
            //        T t = (T)Activator.CreateInstance(type);
            //        foreach (var item in type.GetProperties())
            //        {
            //            //给实体(t)的这个属性(item)设置为这个值reader[item.Name]
            //            //为nul就给null值,不为null就给查到的值
            //            item.SetValue(t, reader[item.GetMappingName()] is DBNull ? null : reader[item.GetMappingName()]);
            //        }
            //        return (T)t;
            //    }
            //    else
            //    {
            //        return default(T);
            //    }
            //}
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

            //using (SqlConnection conn = new SqlConnection(config.SqlConnStr))
            //{
            //    SqlCommand command = new SqlCommand(sql, conn);
            //    command.Parameters.AddRange(parameters.ToArray());
            //    conn.Open();
            //    int result = command.ExecuteNonQuery();
            //    return result == 1;
            //}
            int iResult = this.ExecuteSql<int>(sql, parameters, (m) => m.ExecuteNonQuery());
            return iResult == 1;
        }

        /// <summary>
        /// 数据修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task<bool> Update<T>(T t) where T : BaseModel
        {
            Type type = t.GetType();
            string sql = $"{SqlBuilder<T>.GetSql(SqlBuilder<T>.SqlType.UpdateSql)}'{t.Id}'";
            //参数名称：m.GetMappingName()   值：m.GetValue(t) ?? DBNull.Value
            var sqlParameterList = type.GetPropertyWithoutKey().Select(m => new SqlParameter(m.GetMappingName(), m.GetValue(t) ?? DBNull.Value)).ToArray();

            //using (SqlConnection conn = new SqlConnection(config.SqlConnStr))
            //{
            //    SqlCommand command = new SqlCommand(sql, conn);
            //    //添加参数
            //    command.Parameters.AddRange(sqlParameterList);
            //    conn.Open();
            //    int result = command.ExecuteNonQuery();
            //    return result == 1;
            //}

            int iResult = this.ExecuteSql<int>(sql, sqlParameterList, (m) => m.ExecuteNonQuery());
            return iResult == 1;
        }


        /// <summary>
        /// 数据删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> Delete<T>(Guid id) where T : BaseModel
        {
            Type type = typeof(T);
            //string sql = $"{SqlBuilder<T>.GetSql(SqlBuilder<T>.SqlType.DeleteSql)}'{id}'";
            string sql = SqlBuilder<T>.GetSql(SqlBuilder<T>.SqlType.DeleteSql);
            //using (SqlConnection conn = new SqlConnection(config.SqlConnStr))
            //{
            //    SqlCommand command = new SqlCommand(sql, conn);
            //    //添加参数
            //    //command.Parameters.AddRange(sqlParameterList);
            //    conn.Open();
            //    int result = command.ExecuteNonQuery();
            //    return result == 1;
            //}
            IEnumerable<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter("@id",id)
            };
            int iResult = this.ExecuteSql<int>(sql, parameters, (m) => m.ExecuteNonQuery());
            return iResult == 1;
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<bool> Delete<T>(IEnumerable<T> list) where T : BaseModel
        {
            Type type = typeof(T);
            string Ids = string.Join(",", list.Select(m => $"'{m.Id}'"));
            //一条sql,本来就带事务性质(可以不用再写批处理语句)
            string sql = $"delete from [{type.GetMappingName()}] where id in ({Ids})";
            using (SqlConnection conn = new SqlConnection(config.SqlConnStr))
            {
                SqlTransaction trans = null;
                try
                {
                    conn.Open();
                    //开启事务
                    trans = conn.BeginTransaction();
                    SqlCommand command = new SqlCommand(sql, conn, trans);
                    int iResult = command.ExecuteNonQuery();
                    if (iResult == list.Count())
                    {
                        trans.Commit();
                        return true;
                    }
                    else
                        throw new Exception("删除的数据量不对");
                }
                catch (Exception ex)
                {
                    if (trans != null)
                        trans.Rollback();
                    Console.WriteLine(ex.Message);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 为了代码复用,可以用委托封装
        /// 不同的逻辑,用委托传递
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T ExecuteSql<T>(string sql, IEnumerable<SqlParameter> parameters, Func<SqlCommand, T> func)
        {
            using (SqlConnection conn = new SqlConnection(config.SqlConnStr))
            {
                SqlCommand command = new SqlCommand(sql, conn);
                //添加参数
                command.Parameters.AddRange(parameters.ToArray());
                conn.Open();
                T t = func.Invoke(command);
                return t;
            }
        }
    }
}
