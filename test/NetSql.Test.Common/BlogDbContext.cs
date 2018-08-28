using NetSql.Test.Common.Model;

namespace NetSql.Test.Common
{
public class BlogDbContext : DbContext
{
    public BlogDbContext(IDbContextOptions options) : base(options)
    {
    }

    public IDbSet<Article> Articles { get; set; }
}
}
