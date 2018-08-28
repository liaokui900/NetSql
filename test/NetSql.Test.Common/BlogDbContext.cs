using NetSql.Test.Common.Model;

namespace NetSql.Test.Common
{
    public class BlogDbContext : DbContext
    {
        public IDbSet<Article> Articles { get; set; }

        public BlogDbContext(IDbContextOptions options) : base(options)
        {
        }
    }
}
