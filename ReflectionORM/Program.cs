using Model;
using Services;
using System;
using System.Threading.Tasks;

namespace ReflectionORM
{
    class Program
    {
        static async Task Main(string[] args)
        {
            SqlHelper sqlHelper = new SqlHelper();
            var data = await sqlHelper.Find<Products>(Guid.Parse("C27DB4DF-E208-4E6F-B416-D0F475FF9051"));
            var data1 = await sqlHelper.Find<Shops>(Guid.Parse("C27DB4DF-E208-4E6F-B416-698743FF9051"));
            bool result = await sqlHelper.Insert<ShopType>(new ShopType
            {
                Name = "类型1",
                Remarks = "无备注",
                Date = DateTime.Now
            });
            Console.ReadKey();
            Console.WriteLine("Hello World!");
        }
    }
}
