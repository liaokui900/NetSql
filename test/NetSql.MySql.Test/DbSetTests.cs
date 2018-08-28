using System;
using System.Diagnostics;
using System.Linq;
using NetSql.Enums;
using NetSql.SQLite;
using NetSql.Test.Common;
using NetSql.Test.Common.Model;
using Xunit;

namespace NetSql.MySql.Test
{
    public class DbSetTests
    {
        private readonly BlogDbContext _dbContext;
        private readonly IDbSet<Article> _dbSet;

        public DbSetTests()
        {
            _dbContext = new BlogDbContext(new SQLiteDbContextOptions("Filename=./Database/Test.db"));
            _dbSet = _dbContext.Set<Article>();

            //预热
            _dbSet.Find().First();
        }

        [Fact]
        public async void InsertTest()
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

            await _dbSet.InsertAsync(article);

            Assert.True(article.Id > 0);
        }

        [Fact]
        public void BatchInsertTest()
        {
            var sw = new Stopwatch();
            sw.Start();

            var tran = _dbContext.BeginTransaction();

            for (var i = 0; i < 10000; i++)
            {
                var article = new Article
                {
                    Title1 = "test" + i,
                    Category = i % 3 == 1 ? Category.Blog : Category.Movie,
                    Summary = "这是一篇测试文章",
                    Body = "这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章这是一篇测试文章",
                    ReadCount = 10,
                    IsDeleted = i % 2 == 0,
                    CreatedTime = DateTime.Now
                };

                _dbSet.InsertAsync(article, tran);
            }

            tran.Commit();

            sw.Stop();

            var s = sw.ElapsedMilliseconds;

            Assert.True(s > 0);
        }

        [Fact]
        public void DeleteTest()
        {
            var b = _dbSet.DeleteAsync(3).Result;

            Assert.True(b);
        }

        [Fact]
        public async void DeleteWhereTest()
        {
            var b = await _dbSet.Find(m => m.Id > 10)
                .Where(m => m.CreatedTime > DateTime.Now).Delete();

            Assert.True(b);
        }

        [Fact]
        public async void UpdateTest()
        {
            var article = await _dbSet.Find().First();
            article.Title1 = "修改测试";

            var b = await _dbSet.UpdateAsync(article);

            Assert.True(b);
        }

        [Fact]
        public async void UpdateWhereTest()
        {
            var b = await _dbSet.Find(m => m.Id == 1000).Update(m => new Article
            {
                Title1 = "hahahaah",
                ReadCount = 1000
            });

            Assert.True(b);
        }

        [Fact]
        public void GetTest()
        {
            var article = _dbSet.GetAsync(100).Result;

            Assert.NotNull(article);
        }

        [Fact]
        public async void GetWehreTest()
        {
            var article = await _dbSet.Find().Where(m => m.Id > 1).First();

            Assert.NotNull(article);
        }

        [Fact]
        public async void FindTest()
        {
            var list = await _dbSet.Find(m => m.Id > 100 && m.Id < 120).ToList();

            Assert.Equal(19, list.Count);
        }

        [Theory]
        [InlineData(1)]
        public void WhereTest(int id)
        {
            var query = _dbSet.Find().WhereIf(id > 1, m => m.Id > 200);

            var list = query.ToList();

            Assert.Equal(99, list.Result.Count);
        }

        [Fact]
        public async void OrderByTest()
        {
            var query = _dbSet.Find(m => m.Id > 200 && m.Id < 1000).OrderBy(m => m.Id, SortType.Desc);
            var list = await query.ToList();

            Assert.Equal(99, list.Count);
        }

        [Fact]
        public void FirstTest()
        {
            var article = _dbSet.Find(m => m.Id > 100 && m.Id < 120).First().Result;

            Assert.NotNull(article);
        }

        [Fact]
        public void LimitTest()
        {
            var list = _dbSet.Find(m => m.Id > 100 && m.Id < 120).Limit(5, 10).ToList().Result;

            Assert.Equal(10, list.Count);
        }

        [Fact]
        public void MaxTest()
        {
            var maxReadCount = _dbSet.Find().Max(m => m.ReadCount).Result;

            Assert.True(maxReadCount > 0);
        }

        [Fact]
        public void MinTest()
        {
            var maxReadCount = _dbSet.Find().Min(m => m.ReadCount).Result;

            Assert.True(maxReadCount > 0);
        }

        [Fact]
        public void CountTest()
        {
            var sw = new Stopwatch();
            sw.Start();

            var count = _dbSet.Find(m => m.Id > 1000).Count().Result;

            sw.Stop();

            Assert.True(count > 0);
        }

        [Fact]
        public void InTest()
        {
            var ids = new[] { 100, 200 };
            var list = _dbSet.Find(m => ids.Contains(m.Id)).ToList().Result;

            Assert.Equal(2, list.Count);
        }

        [Fact]
        public void StartsWithTest()
        {
            var list = _dbSet.Find(m => m.Title1.StartsWith("test11")).ToList().Result;

            Assert.NotEmpty(list);
        }

        [Fact]
        public void EndsWithTest()
        {
            var list = _dbSet.Find(m => m.Title1.EndsWith("11")).ToList().Result;

            Assert.NotEmpty(list);
        }

        [Fact]
        public void ContainsTest()
        {
            var list = _dbSet.Find(m => m.Title1.Contains("11")).ToList().Result;

            Assert.NotEmpty(list);
        }

        [Fact]
        public async void EqualsTest()
        {
            var query = _dbSet.Find(m => m.Id.Equals(1));
            var sql = query.ToSql();
            var list = await query.ToList();

            Assert.NotEmpty(list);
        }

        [Fact]
        public async void SelectTest()
        {
            var query = _dbSet.Find().Select(m => new { m.Id, m.Title1 }).Limit(0, 10);
            var list = await query.ToList();

            Assert.NotEmpty(list);
        }

    }
}
