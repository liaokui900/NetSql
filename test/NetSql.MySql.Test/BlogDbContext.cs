using NetSql.SQLite;
using NetSql.Test.Common.Model;

namespace NetSql.MySql.Test
{
    public class BlogDbContext : DbContext
    {
        public IDbSet<Article> Articles { get; set; }

        public BlogDbContext() : base(new SQLiteDbContextOptions("Filename=./Database/Test.db"))
        {
        }
    }
}
