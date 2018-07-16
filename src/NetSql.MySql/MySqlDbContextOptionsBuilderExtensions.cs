using NetSql.MySql;

namespace NetSql
{
    public static class MySqlDbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UseMySql(this DbContextOptionsBuilder builder, string connectionString)
        {
            builder.Init(new MySqlDbContextOptions(connectionString));
            return builder;
        }
    }
}
