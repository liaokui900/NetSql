using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using NetSql.SqlAdapter;

namespace NetSql.MySql
{
    internal class MySqlDbContextOptions : DbContextOptionsAbstract
    {
        public override IDbConnection DbConnection => new MySqlConnection(ConnectionString);

        public MySqlDbContextOptions(string connectionString) : base(connectionString, new MySqlAdapter(), Enums.DbType.MySql)
        {
        }
    }
}
