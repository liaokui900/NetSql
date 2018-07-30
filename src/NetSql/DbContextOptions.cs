using System.Data;
using System.Data.SqlClient;
using NetSql.Internal;
using NetSql.SqlAdapter;
using DbType = NetSql.Enums.DbType;

namespace NetSql
{
    public class DbContextOptions : IDbContextOptions
    {

        public string ConnectionString { get; }

        public ISqlAdapter SqlAdapter { get; }

        public IDbConnection DbConnection => new SqlConnection(ConnectionString);

        public DbType DbType { get; }

        public DbContextOptions(string connectionString)
        {
            Check.NotNull(connectionString, nameof(connectionString), "数据库连接字符串为空");

            ConnectionString = connectionString;
            DbType = DbType.SqlServer;
            SqlAdapter = new SqlServerAdapter();
        }
    }
}
