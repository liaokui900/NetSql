using System;
using System.Collections.Generic;
using System.Diagnostics;
using NetSql.Internal;
using NetSql.Test.Common.Model;
using Xunit;

namespace NetSql.MySql.Test
{
    public class CrudTests
    {
        private readonly BlogDbContext _dbContext = new BlogDbContext();

        [Fact]
        public void AddTest()
        {
            var article = new Article
            {
                Title1 = "test",
                Category = Category.Blog,
                Summary = "这是一篇测试文章",
                Body = "这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章",
                ReadCount = 10,
                IsDeleted = true,
                CreatedTime = DateTime.Now
            };

            var b = _dbContext.Articles.AddAsync(article).Result;

            Assert.True(article.Id > 0);
        }

        [Fact]
        public void BatchAddTest()
        {
            var list = new List<Article>();
            for (var i = 0; i < 10; i++)
            {
                list.Add(new Article
                {
                    Title1 = "test" + i,
                    Category = i % 3 == 1 ? Category.Blog : Category.Movie,
                    Summary = "这是一篇测试文章",
                    Body = "这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章",
                    ReadCount = 10,
                    IsDeleted = i % 2 == 0,
                    CreatedTime = DateTime.Now
                });
            }

            //预热
            var b1 = _dbContext.Articles.BatchAddtAsync(list).Result;

            list.Clear();

            for (var i = 0; i < 10000; i++)
            {
                list.Add(new Article
                {
                    Title1 = "test" + i,
                    Category = i % 3 == 1 ? Category.Blog : Category.Movie,
                    Summary = "这是一篇测试文章",
                    Body = "这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章",
                    ReadCount = 10,
                    IsDeleted = i % 2 == 0,
                    CreatedTime = DateTime.Now
                });
            }


            var sw = new Stopwatch();
            sw.Start();

            var b = _dbContext.Articles.BatchAddtAsync(list).Result;

            sw.Stop();

            var s = sw.ElapsedMilliseconds;

            /*********批量插入1w数据耗时**************
             *
             * SqlServer：1415ms
             *     MySql：1347ms
             *    SQLite：262ms
             *
             *********************************/

            Assert.True(b);
        }

        [Fact]
        public void RemoveTest()
        {
            var b = _dbContext.Articles.RemoveAsync(2).Result;

            Assert.True(b > 0);
        }

        [Fact]
        public void RemoveByExpressionTest()
        {
            var b = _dbContext.Articles.RemoveAsync(m => m.Title1 == "hahaha").Result;

            Assert.True(b > 0);
        }

        [Fact]
        public void BatchRemoveTest()
        {
            BatchAddTest();

            var idList = new List<int>();
            for (var i = 0; i < 10000; i++)
            {
                idList.Add(i);
            }

            var sw = new Stopwatch();
            sw.Start();

            var b = _dbContext.Articles.BatchRemoveAsync(idList).Result;

            sw.Stop();

            var s = sw.ElapsedMilliseconds;

            /*********批量删除1w数据耗时***********
            *
            * SqlServer：1145ms
            *     MySql：73ms
            *    SQLite：43ms
            *
            ************************************/

            Assert.True(b);
        }

        [Fact]
        public void UpdateTest()
        {
            var entity = new Article
            {
                Id = 1,
                Title1 = "更新测试",
                IsDeleted = false
            };

            var b = _dbContext.Articles.UpdateAsync(entity).Result;

            Assert.True(b > 0);
        }

        [Fact]
        public void UpdateByExpressionTest()
        {
            BatchAddTest();

            var b = _dbContext.Articles.UpdateAsync(m => m.Id > 10, n => new Article
            {
                Title1 = "哈哈",
                ReadCount = 100
            }).Result;

            Assert.True(b > 0);
        }

        [Fact]
        public void BatchUpdateTest()
        {
            BatchAddTest();

            var list = new List<Article>();
            for (var i = 1; i < 10000; i++)
            {
                list.Add(new Article
                {
                    Id = i,
                    Title1 = "更新测试" + i,
                    Summary = "更新测试这是一篇测试文章",
                    Body = "更新测试这一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章",
                    ReadCount = 20,
                    IsDeleted = i % 2 == 1
                });
            }

            var sw = new Stopwatch();
            sw.Start();

            var b = _dbContext.Articles.BatchUpdateAsync(list).Result;

            sw.Stop();

            var s = sw.ElapsedMilliseconds;

            /*********批量更新1w数据耗时***********
            *
            * SqlServer：845ms
            *     MySql：1872ms
            *    SQLite：339ms
            *
            ************************************/

            Assert.True(b);
        }

        [Fact]
        public void GetTest()
        {
            var entity = _dbContext.Articles.GetAsync(2).Result;

            Assert.NotNull(entity);
        }
    }
}
