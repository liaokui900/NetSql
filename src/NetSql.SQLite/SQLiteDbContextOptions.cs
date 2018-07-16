using System.Data;
using Microsoft.Data.Sqlite;

namespace NetSql.SQLite
{
    internal class SQLiteDbContextOptions : DbContextOptionsAbstract
    {
        public SQLiteDbContextOptions(string connectionString) : base(connectionString, new SQLiteAdapter(), Enums.DbType.SQLite)
        {
        }

        public override IDbConnection DbConnection => new SqliteConnection(ConnectionString);
    }
}
