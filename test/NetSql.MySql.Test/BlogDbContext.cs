using NetSql.Test.Common.Model;

namespace NetSql.MySql.Test
{
    public class BlogDbContext : NetSqlDbContext
    {
        public IDbSet<Article> Articles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer("Server=10.32.1.183;Initial Catalog=Test;User ID=sa;Password=oldli!@#123;MultipleActiveResultSets=True");
            //optionsBuilder.UseMySql("Server=localhost;Database=Test;Uid=root;Pwd=oldli!@#123;Allow User Variables=True;charset=utf8;");
            optionsBuilder.UseSQLite("Filename=./Database/Test.db");
        }
    }
}
