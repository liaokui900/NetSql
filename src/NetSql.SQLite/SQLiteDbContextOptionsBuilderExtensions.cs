using NetSql.SQLite;

namespace NetSql
{
    public static class SQLiteDbContextOptionsBuilderExtensions
    {
        /// <summary>
        /// 使用SQLite数据库
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder UseSQLite(this DbContextOptionsBuilder builder, string connectionString)
        {
            builder.Init(new SQLiteDbContextOptions(connectionString));
            return builder;
        }
    }
}
