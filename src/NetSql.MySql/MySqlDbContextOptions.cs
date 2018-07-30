using System.Data;
using MySql.Data.MySqlClient;
using NetSql.Internal;
using NetSql.SqlAdapter;
using DbType = NetSql.Enums.DbType;

namespace NetSql.MySql
{
    public class MySqlDbContextOptions : IDbContextOptions
    {
        public string ConnectionString { get; }

        public ISqlAdapter SqlAdapter { get; }

        public IDbConnection DbConnection => new MySqlConnection(ConnectionString);

        public DbType DbType { get; }

        public MySqlDbContextOptions(string connectionString)
        {
            Check.NotNull(connectionString, nameof(connectionString), "数据库连接字符串为空");

            ConnectionString = connectionString;
            DbType = DbType.MySql;
            SqlAdapter = new MySqlAdapter();
        }
    }
}
