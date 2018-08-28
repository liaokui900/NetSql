using System;
using System.Collections.Generic;
using System.Diagnostics;
using NetSql.Pagination;
using NetSql.SQLite;
using NetSql.Test.Common;
using NetSql.Test.Common.Model;
using NetSql.Test.Common.Repository;
using Xunit;

namespace NetSql.MySql.Test
{
    public class RepositoryTest
    {
        private readonly IArticleRepository _repository;

        public RepositoryTest()
        {
            var dbContext = new BlogDbContext(new SQLiteDbContextOptions("Filename=./Database/Test.db"));
            _repository = new ArticleRepository(dbContext);
        }

        [Fact]
        public async void AddTest()
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

            await _repository.AddAsync(article);

            Assert.True(article.Id > 0);
        }

        [Fact]
        public void BatchInsertTest()
        {
            var list = new List<Article>();
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
                list.Add(article);
            }
            var sw = new Stopwatch();
            sw.Start();

            _repository.AddAsync(list);

            sw.Stop();
            var s = sw.ElapsedMilliseconds;

            Assert.True(s > 0);
        }

        [Fact]
        public async void DeleteTest()
        {
            var b = await _repository.DeleteAsync(2);

            Assert.True(b);
        }

        [Fact]
        public async void UpdateTest()
        {
            var article = await _repository.GetAsync(2);
            article.Title1 = "修改测试";

            var b = await _repository.UpdateAsync(article);

            Assert.True(b);
        }

        [Fact]
        public async void PaginationTest()
        {
            var paging = new Paging(1, 20);
            var list = await _repository.PaginationAsync(paging, m => m.Id > 1000);

            Assert.True(paging.TotalCount > 0);
        }
    }
}
