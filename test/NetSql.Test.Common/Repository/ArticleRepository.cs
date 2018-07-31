using System;
using System.Collections.Generic;
using System.Text;
using NetSql.DDDLite;
using NetSql.Test.Common.Model;

namespace NetSql.Test.Common.Repository
{
    public class ArticleRepository : RepositoryAbstract<Article>
    {
        public ArticleRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}
