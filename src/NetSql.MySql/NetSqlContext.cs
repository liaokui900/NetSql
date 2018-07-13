using System.Data;
using MySql.Data.MySqlClient;

namespace NetSql.MySql
{
    public abstract class NetSqlContext : NetSqlContextAbstract
    {
        protected NetSqlContext(string connectionString) : base(connectionString, new SqlAdapter())
        {

        }
    }
}
