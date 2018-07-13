using System;
using System.Collections.Generic;
using System.Text;
using NetSql.Test.Common.Model;

namespace NetSql.MySql.Test
{
    public class BlogDbContext : NetSqlContext
    {
        public BlogDbContext() : base("ConnectionString")
        {

        }

        public IDbSet<Article> Articles { get; set; }
    }
}
