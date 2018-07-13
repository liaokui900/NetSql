using System;
using System.Collections.Generic;
using System.Text;
using NetSql.Test.Common.Model;
using Xunit;

namespace NetSql.MySql.Test
{
    public class CrudTests
    {
        private readonly BlogDbContext _dbContext = new BlogDbContext();

        [Fact]
        public void Insert()
        {
            var article = new Article
            {
                Title = "test",
                IsDeleted = true
            };

            _dbContext.Articles.Insert(article);

            Assert.True(article.Id > 0);
        }
    }
}
