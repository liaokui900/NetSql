namespace NetSql
{
    public static class DbContextOptionsBuilderExtensions
    {
        /// <summary>
        /// 使用SqlServer数据库
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static DbContextOptionsBuilder UseSqlServer(this DbContextOptionsBuilder builder, string connectionString)
        {
            builder.Init(new DbContextOptions(connectionString));
            return builder;
        }
    }
}
