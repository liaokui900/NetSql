using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using NetSql.SqlAdapter;

namespace NetSql
{
    internal class DbContextOptions : DbContextOptionsAbstract
    {
        public DbContextOptions(string connectionString) : base(connectionString, new SqlServerAdapter(), Enums.DbType.SqlServer)
        {
        }

        public override IDbConnection DbConnection => new SqlConnection(ConnectionString);
    }
}
