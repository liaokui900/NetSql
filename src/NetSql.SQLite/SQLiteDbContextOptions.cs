using System.Data;
using Microsoft.Data.Sqlite;
using NetSql.Internal;
using NetSql.SqlAdapter;
using DbType = NetSql.Enums.DbType;

namespace NetSql.SQLite
{
    public class SQLiteDbContextOptions : IDbContextOptions
    {
        public string ConnectionString { get; }

        public ISqlAdapter SqlAdapter { get; }

        public IDbConnection DbConnection => new SqliteConnection(ConnectionString);

        public DbType DbType { get; }

        public SQLiteDbContextOptions(string connectionString)
        {
            Check.NotNull(connectionString, nameof(connectionString), "数据库连接字符串为空");

            ConnectionString = connectionString;
            DbType = DbType.MySql;
            SqlAdapter = new SQLiteAdapter();
        }
    }
}
